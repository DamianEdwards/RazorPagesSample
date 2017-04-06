using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RazorPagesWebApplication.Data;

namespace RazorPagesWebApplication.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly string _externalCookieScheme;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            IOptions<IdentityCookieOptions> identityCookieOptions,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _logger = logger;
        }

        [Required]
        [EmailAddress]
        [ModelBinder]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [ModelBinder]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        [ModelBinder]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public async Task OnGet(string returnUrl = null)
        {
            var errorMessage = (string)TempData["ErrorMessage"];
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Email, Password, RememberMe, lockoutOnFailure: true);
                // BUG: This shouldn't allow login if email still requires verification
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("/Account/SendCode", new { returnUrl, RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return RedirectToPage("/Account/Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
    }
}
