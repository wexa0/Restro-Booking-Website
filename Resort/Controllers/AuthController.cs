using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Resort.Domain.Users;
using Resort.Filters;
using Resort.Models.Auth;
using System.Security.Claims;

namespace Resort.Controllers;

[AllowAnonymous]
public class AuthController : Controller
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher<User> _hasher;

    public AuthController(IUserRepository users, IPasswordHasher<User> hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    // ============ LOGIN ============
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var dbUser = await _users.FindByEmailAsync(model.Email);
        if (dbUser is null)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        var verify = _hasher.VerifyHashedPassword(dbUser, dbUser.PasswordHash, model.Password);
        if (verify == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        // Claims (اسم العرض = FullName)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, dbUser.Id),
            new Claim(ClaimTypes.Name, dbUser.FullName),      // ✅ FullName هنا
            new Claim(ClaimTypes.Email, dbUser.Email),
            new Claim("full_name", dbUser.FullName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var props = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    // ============ REGISTER ============
    [HttpGet]
    public IActionResult Register()
        => View(new RegisterModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PasswordPolicy]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var existing = await _users.FindByEmailAsync(model.Email);
        if (existing is not null)
        {
            ModelState.AddModelError(nameof(RegisterModel.Email), "Email is already registered.");
            return View(model);
        }

        var newUser = new User
        {
            FullName = model.FullName.Trim(),
            Email = model.Email.Trim()
        };

        newUser.PasswordHash = _hasher.HashPassword(newUser, model.Password);
        await _users.AddAsync(newUser);

        // (اختياري) تسجيل دخول مباشرة بعد التسجيل
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, newUser.Id),
            new Claim(ClaimTypes.Name, newUser.FullName),     // ✅ FullName هنا
            new Claim(ClaimTypes.Email, newUser.Email),
            new Claim("full_name", newUser.FullName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return RedirectToAction("Index", "Home");
    }

    // ============ LOGOUT ============
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Welcome", "Landing");
    }

    [HttpGet]
    public IActionResult AccessDenied()
        => View();
}
