using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BrainHugeAcademy.Data;

namespace BrainHugeAcademy.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var featuredCourses = await _context.Courses
            .Where(c => c.IsFeatured)
            .Include(c => c.Modules)
            .ToListAsync();

        var stats = new
        {
            Courses = await _context.Courses.CountAsync(),
            Modules = await _context.CourseModules.CountAsync(),
            Learners = await _context.Users.CountAsync(),
            Quizzes = await _context.Quizzes.CountAsync()
        };

        ViewBag.Stats = stats;
        return View(featuredCourses);
    }
}
