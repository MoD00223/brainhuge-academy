using System.ComponentModel.DataAnnotations;

namespace BrainHugeAcademy.Models;

public class AiConversation
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int? ModuleId { get; set; }

    [MaxLength(5000)]
    public string UserMessage { get; set; } = string.Empty;

    [MaxLength(5000)]
    public string AiResponse { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
