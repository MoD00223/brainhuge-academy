using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BrainHugeAcademy.Data;
using BrainHugeAcademy.Models;
using System.Security.Claims;

namespace BrainHugeAcademy.Controllers;

public class LearnerController : Controller
{
    private readonly ApplicationDbContext _context;

    public LearnerController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Catalog(string? search, string? category, string? level)
    {
        var query = _context.Courses
            .Include(c => c.Modules)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Title.Contains(search) || c.Description.Contains(search));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(c => c.Category == category);

        if (!string.IsNullOrWhiteSpace(level))
            query = query.Where(c => c.Level == level);

        var courses = await query.OrderByDescending(c => c.IsFeatured).ToListAsync();

        ViewBag.Categories = await _context.Courses.Select(c => c.Category).Distinct().ToListAsync();
        ViewBag.Levels = await _context.Courses.Select(c => c.Level).Distinct().ToListAsync();
        ViewBag.CurrentSearch = search;
        ViewBag.CurrentCategory = category;
        ViewBag.CurrentLevel = level;

        return View(courses);
    }

    public async Task<IActionResult> Details(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .Include(c => c.Modules)
                .ThenInclude(m => m.Quizzes)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return NotFound();

        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            ViewBag.IsEnrolled = await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == id);
        }

        return View(course);
    }

    [Authorize]
    public async Task<IActionResult> MyCourses()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var enrollments = await _context.Enrollments
            .Where(e => e.UserId == userId)
            .Include(e => e.Course)
                .ThenInclude(c => c.Modules)
            .Include(e => e.Progress)
            .ToListAsync();

        return View(enrollments);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Enroll(int courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (!await _context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId))
        {
            _context.Enrollments.Add(new Enrollment
            {
                UserId = userId,
                CourseId = courseId
            });
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("MyCourses");
    }

    [Authorize]
    public async Task<IActionResult> Module(int courseId, int moduleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (enrollment == null) return RedirectToAction("Details", new { id = courseId });

        var module = await _context.CourseModules
            .Include(m => m.Lessons.OrderBy(l => l.OrderIndex))
            .Include(m => m.Quizzes)
                .ThenInclude(q => q.Questions)
            .FirstOrDefaultAsync(m => m.Id == moduleId && m.CourseId == courseId);

        if (module == null) return NotFound();

        var progress = await _context.ModuleProgresses
            .FirstOrDefaultAsync(p => p.EnrollmentId == enrollment.Id && p.ModuleId == moduleId);

        ViewBag.Progress = progress;
        ViewBag.Enrollment = enrollment;

        var allModules = await _context.CourseModules
            .Where(m => m.CourseId == courseId)
            .OrderBy(m => m.OrderIndex)
            .Select(m => new { m.Id, m.Title, m.OrderIndex })
            .ToListAsync();

        ViewBag.AllModules = allModules;

        return View(module);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CompleteModule(int enrollmentId, int moduleId, int score, int total)
    {
        var existing = await _context.ModuleProgresses
            .FirstOrDefaultAsync(p => p.EnrollmentId == enrollmentId && p.ModuleId == moduleId);

        if (existing == null)
        {
            _context.ModuleProgresses.Add(new ModuleProgress
            {
                EnrollmentId = enrollmentId,
                ModuleId = moduleId,
                IsCompleted = true,
                QuizScore = score,
                QuizTotal = total,
                CompletedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.IsCompleted = true;
            existing.QuizScore = score;
            existing.QuizTotal = total;
            existing.CompletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("MyCourses");
    }
}
