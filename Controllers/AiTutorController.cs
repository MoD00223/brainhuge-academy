using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BrainHugeAcademy.Data;
using BrainHugeAcademy.Models;

namespace BrainHugeAcademy.Controllers;

public class AiTutorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AiTutorController(
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [Authorize]
    public async Task<IActionResult> Chat(int? moduleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        CourseModule? module = null;
        if (moduleId.HasValue)
        {
            module = await _context.CourseModules
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.Id == moduleId.Value);
        }

        var history = await _context.AiConversations
            .Where(c => c.UserId == userId && c.ModuleId == moduleId)
            .OrderBy(c => c.Timestamp)
            .Take(50)
            .ToListAsync();

        ViewBag.Module = module;
        return View(history);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> SendMessage(string message, int? moduleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var module = moduleId.HasValue
            ? await _context.CourseModules
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.Id == moduleId.Value)
            : null;

        var moduleContext = module != null
            ? $"\n\nContexte : L'apprenant étudie '{module.Title}' dans le cours '{module.Course.Title}'. Contenu du module :\n{module.Content}"
            : "";

        var history = await _context.AiConversations
            .Where(c => c.UserId == userId && c.ModuleId == moduleId)
            .OrderBy(c => c.Timestamp)
            .Take(30)
            .ToListAsync();

        var response = await CallClaudeApiAsync(message, moduleContext, history);

        _context.AiConversations.Add(new AiConversation
        {
            UserId = userId,
            ModuleId = moduleId,
            UserMessage = message,
            AiResponse = response,
            Timestamp = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        return RedirectToAction("Chat", new { moduleId });
    }

    private async Task<string> CallClaudeApiAsync(string userMessage, string moduleContext, List<AiConversation> history)
    {
        var apiKey = _configuration["Anthropic:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            return "La clé API Anthropic n'est pas configurée. Veuillez la renseigner dans appsettings.json (Anthropic:ApiKey).";

        var systemPrompt = $"""
            Tu es Nova, un tuteur IA pédagogique bienveillant pour BrainHuge Academy.
            Tu aides les apprenants à comprendre les concepts de leur cours en profondeur.

            Ton style :
            - Réponds toujours en français.
            - Sois chaleureux, encourageant et pédagogue.
            - Utilise des analogies issues du monde réel pour rendre les concepts concrets.
            - Structure tes réponses en étapes claires quand c'est pertinent.
            - Termine par une question de vérification pour t'assurer que l'apprenant a compris.
            - Ne donne jamais directement la réponse à un quiz ou exercice : guide l'apprenant vers la découverte.
            - Si tu ne connais pas la réponse, dis-le honnêtement.
            - Garde tes réponses concises mais complètes.
            {moduleContext}
            """;

        var messages = new List<object>();
        foreach (var h in history)
        {
            messages.Add(new { role = "user", content = h.UserMessage });
            messages.Add(new { role = "assistant", content = h.AiResponse });
        }
        messages.Add(new { role = "user", content = userMessage });

        var requestBody = new
        {
            model = "claude-sonnet-5",
            max_tokens = 1024,
            system = systemPrompt,
            messages
        };

        try
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages")
            {
                Content = JsonContent.Create(requestBody, options: new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                })
            };
            request.Headers.Add("x-api-key", apiKey);
            request.Headers.Add("anthropic-version", "2023-06-01");

            var httpResponse = await client.SendAsync(request);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorBody = await httpResponse.Content.ReadAsStringAsync();
                return $"Erreur API Claude ({httpResponse.StatusCode}) : {errorBody}";
            }

            var json = await httpResponse.Content.ReadAsStringAsync();
            Console.WriteLine("[Nova] Raw API response: " + json);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("content", out var contentArray) &&
                contentArray.GetArrayLength() > 0)
            {
                foreach (var block in contentArray.EnumerateArray())
                {
                    if (block.TryGetProperty("type", out var typeProp) &&
                        typeProp.GetString() == "text" &&
                        block.TryGetProperty("text", out var textElement))
                    {
                        var result = textElement.GetString();
                        if (!string.IsNullOrWhiteSpace(result))
                            return result;
                    }
                }

                Console.WriteLine("[Nova] Content array exists but no text block found. First block: " + contentArray[0].GetRawText());
            }

            Console.WriteLine("[Nova] Parsing failed. Root properties: " + string.Join(", ", root.EnumerateObject().Select(p => p.Name)));
            return $"Format de réponse inattendu de l'API Claude. Réponse brute : {json.Substring(0, Math.Min(json.Length, 500))}";
        }
        catch (HttpRequestException ex)
        {
            return $"Erreur de connexion à l'API Claude : {ex.Message}";
        }
        catch (TaskCanceledException)
        {
            return "L'appel à l'API Claude a expiré. Veuillez réessayer.";
        }
        catch (Exception ex)
        {
            return $"Erreur inattendue : {ex.Message}";
        }
    }
}
