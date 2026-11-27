using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mindfulness.Web.Pages;

[AllowAnonymous]
public sealed class RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : PageModel
{
    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public sealed class RegisterInput
    {
        [Required, StringLength(40, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9._-]+$")]
        public string UserName { get; set; } = "";

        [Required, StringLength(128, MinimumLength = 12)]
        public string Password { get; set; } = "";

        [Required, Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = "";
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        IdentityUser user = new IdentityUser { UserName = Input.UserName };
        IdentityResult result = await userManager.CreateAsync(user, Input.Password);
        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToPage("/Index");
        }

        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }
}
