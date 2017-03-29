using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RazorPagesWebApplication.Models;

namespace RazorPagesWebApplication.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly string _externalCookieScheme;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private IUrlHelper _url;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            IOptions<IdentityCookieOptions> identityCookieOptions,
            IUrlHelperFactory urlHelperFactory,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _urlHelperFactory = urlHelperFactory;
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

        // Bug: This should be on PageModel 
        public IUrlHelper Url
        {
            get
            {
                if (_url == null)
                {
                    var factory = HttpContext?.RequestServices?.GetRequiredService<IUrlHelperFactory>();
                    _url = factory?.GetUrlHelper(PageContext);
                }

                return _url;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _url = value;
            }
        }

        public async Task OnGet(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);

            ViewData["ReturnUrl"] = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Email, Password, RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return Redirect($"~/Account/SendCode?ReturnUrl={returnUrl}&RememberMe={RememberMe}");
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return Redirect("~/Account/Lockout");
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
                return Redirect("~/");
            }
        }
    }
}
