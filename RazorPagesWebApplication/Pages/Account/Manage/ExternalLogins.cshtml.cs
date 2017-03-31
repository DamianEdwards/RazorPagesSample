using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using RazorPagesWebApplication.Models;

namespace RazorPagesWebApplication.Pages.Manage
{
    public class ExternalLoginsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly string _externalCookieScheme;

        public ExternalLoginsModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<IdentityCookieOptions> identityCookieOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
        }

        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationDescription> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.AddLoginSuccess ? "The external login was added."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return Redirect("~/Error");
            }

            CurrentLogins = await _userManager.GetLoginsAsync(user);
            OtherLogins = _signInManager.GetExternalAuthenticationSchemes()
                .Where(auth => CurrentLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider))
                .ToList();
            ShowRemoveButton = user.PasswordHash != null || CurrentLogins.Count > 1;
            return View();
        }

        public async Task<IActionResult> OnPostRemoveLogin(string loginProvider, string providerKey)
        {
            StatusMessage = "An error has occurred.";
            ManageMessageId? message = ManageMessageId.Error;
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    message = ManageMessageId.RemoveLoginSuccess;
                }
            }
            StatusMessage = "The external login was removed.";
            //return Redirect("~/Account/Manage/ExternalLogins");
            return Redirect($"~/Account/Manage/ExternalLogins/?Message={message}");
        }

        public async Task<IActionResult> OnPostLinkLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.RouteUrl(null, new { page = "/Account/Manage/ExternalLogins/LinkLoginCallback" });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(HttpContext.User));
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetLinkLoginCallback()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Redirect("~/Error");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));
            if (info == null)
            {
                return Redirect($"~/Account/Manage/ExternalLogins/?Message={ManageMessageId.Error}");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                return Redirect($"~/Account/Manage/ExternalLogins/?Message={ManageMessageId.Error}");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);

            return Redirect($"~/Account/Manage/ExternalLogins/?Message={ManageMessageId.AddLoginSuccess}");
        }
    }
}
