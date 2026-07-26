namespace BrainHugeAcademy.Models;

public class ModuleProgress
{
    public int Id { get; set; }

    public int EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; } = null!;

    public int ModuleId { get; set; }
    public CourseModule Module { get; set; } = null!;

    public bool IsCompleted { get; set; }

    public int QuizScore { get; set; }
    public int QuizTotal { get; set; }

    public DateTime? CompletedAt { get; set; }
}
