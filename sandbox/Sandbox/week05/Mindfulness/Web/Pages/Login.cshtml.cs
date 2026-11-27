using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mindfulness.Web.Pages;

[AllowAnonymous]
public sealed class LoginModel(SignInManager<IdentityUser> signInManager) : PageModel
{
    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public sealed class LoginInput
    {
        [Required, StringLength(40)]
        public string UserName { get; set; } = "";

        [Required, StringLength(128)]
        public string Password { get; set; } = "";
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var result = await signInManager.PasswordSignInAsync(Input.UserName, Input.Password,
            isPersistent: false, lockoutOnFailure: true);
        if (result.Succeeded) return RedirectToPage("/Index");
        ModelState.AddModelError(string.Empty, result.IsLockedOut
            ? "This account is temporarily locked. Try again later."
            : "Invalid username or password.");
        return Page();
    }
}
