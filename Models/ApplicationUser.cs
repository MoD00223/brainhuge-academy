using Microsoft.AspNetCore.Identity;

namespace BrainHugeAcademy.Models;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public List<Enrollment> Enrollments { get; set; } = new();
}
