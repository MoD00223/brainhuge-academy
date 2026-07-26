using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BrainHugeAcademy.Models;
using System.ComponentModel.DataAnnotations;

namespace BrainHugeAcademy.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            DisplayName = model.DisplayName
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
            return LocalRedirect(returnUrl ?? "/");

        ModelState.AddModelError(string.Empty, "Tentative de connexion invalide.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Le nom d'affichage est requis."), Display(Name = "Nom d'affichage"), MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'e-mail est requis."), EmailAddress(ErrorMessage = "Adresse e-mail invalide.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis."), DataType(DataType.Password), MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmation du mot de passe est requise."), DataType(DataType.Password), Display(Name = "Confirmer le mot de passe"), Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required(ErrorMessage = "L'e-mail est requis."), EmailAddress(ErrorMessage = "Adresse e-mail invalide.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis."), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Se souvenir de moi ?")]
    public bool RememberMe { get; set; }
}