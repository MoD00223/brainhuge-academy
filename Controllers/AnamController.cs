using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace BrainHugeAcademy.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AnamController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public AnamController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("session-token")]
    public async Task<IActionResult> GetSessionToken([FromBody] SessionTokenRequest? request = null)
    {
        var apiKey = _configuration["Anam:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
            return BadRequest(new { error = "Clé API Anam non configurée." });

        var systemPrompt = request?.SystemPrompt
            ?? "Vous êtes Nova, un compagnon d'apprentissage IA de classe mondiale pour BrainHuge Academy. Vous êtes chaleureux, encourageant et profondément compétent. Vous adaptez votre style d'enseignement à chaque apprenant. Vous expliquez les concepts en utilisant des analogies claires, des exemples concrets et des explications étape par étape. Vous ne donnez jamais simplement les réponses — vous guidez les apprenants pour les découvrir. Quand quelqu'un est en difficulté, vous prenez du recul et l'aidez à voir le tableau global. Quand il est prêt pour un défi, vous le poussez plus loin. Vous parlez naturellement, comme un ami brillant qui est aussi un enseignant incroyable. Gardez les réponses concises mais percutantes — visez la clarté plutôt que la longueur.";

        var personaConfig = new
        {
            name = "Nova",
            avatarId = _configuration["Anam:AvatarId"] ?? "30fa96d0-26c4-4e55-94a0-517025942e18",
            avatarModel = _configuration["Anam:AvatarModel"] ?? "cara-4",
            voiceId = _configuration["Anam:VoiceId"] ?? "6bfbe25a-979d-40f3-a92b-5394170af54b",
            llmId = _configuration["Anam:LlmId"] ?? "a7cf662c-2ace-4de1-a21e-ef0fbf144bb7",
            systemPrompt = systemPrompt
        };

        var body = JsonSerializer.Serialize(new { personaConfig });

        try
        {
            var client = _httpClientFactory.CreateClient();
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.anam.ai/v1/auth/session-token")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.Add("Authorization", $"Bearer {apiKey}");

            var response = await client.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { error = $"Erreur API Anam : {response.StatusCode}", details = errorContent });
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);

            if (result.TryGetProperty("sessionToken", out var tokenElement))
            {
                return Ok(new { sessionToken = tokenElement.GetString() });
            }

            return Ok(new { raw = json });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Échec de la connexion à Anam : {ex.Message}" });
        }
    }
}

public class SessionTokenRequest
{
    public string? SystemPrompt { get; set; }
    public string? ModuleContext { get; set; }
}