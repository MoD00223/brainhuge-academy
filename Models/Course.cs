using System.ComponentModel.DataAnnotations;

namespace BrainHugeAcademy.Models;

public class Course
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Level { get; set; } = "Débutant";

    public int DurationHours { get; set; }

    [MaxLength(500)]
    public string IconUrl { get; set; } = string.Empty;

    public bool IsFeatured { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<CourseModule> Modules { get; set; } = new();
    public List<Enrollment> Enrollments { get; set; } = new();
}
