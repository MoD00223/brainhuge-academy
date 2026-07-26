using System.ComponentModel.DataAnnotations;

namespace BrainHugeAcademy.Models;

public class Lesson
{
    public int Id { get; set; }

    public int ModuleId { get; set; }
    public CourseModule Module { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public int OrderIndex { get; set; }

    [MaxLength(10000)]
    public string Content { get; set; } = string.Empty;

    public string LessonType { get; set; } = "Théorie";
}
