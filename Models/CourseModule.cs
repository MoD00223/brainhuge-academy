using System.ComponentModel.DataAnnotations;

namespace BrainHugeAcademy.Models;

public class CourseModule
{
    public int Id { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public int OrderIndex { get; set; }

    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;

    public List<Lesson> Lessons { get; set; } = new();
    public List<Quiz> Quizzes { get; set; } = new();
}
