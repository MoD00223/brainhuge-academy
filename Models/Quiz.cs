using System.ComponentModel.DataAnnotations;

namespace BrainHugeAcademy.Models;

public class Quiz
{
    public int Id { get; set; }

    public int ModuleId { get; set; }
    public CourseModule Module { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public List<QuizQuestion> Questions { get; set; } = new();
}
