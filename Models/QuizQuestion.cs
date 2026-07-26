using System.ComponentModel.DataAnnotations;

namespace BrainHugeAcademy.Models;

public class QuizQuestion
{
    public int Id { get; set; }

    public int QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    [Required, MaxLength(1000)]
    public string QuestionText { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string OptionA { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string OptionB { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string OptionC { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string OptionD { get; set; } = string.Empty;

    [Required, MaxLength(1)]
    public string CorrectOption { get; set; } = "A";

    [MaxLength(1000)]
    public string Explanation { get; set; } = string.Empty;
}
