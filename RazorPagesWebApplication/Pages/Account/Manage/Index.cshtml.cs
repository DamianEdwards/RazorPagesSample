using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RazorPagesWebApplication.Data;

namespace RazorPagesWebApplication.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public bool HasPassword { get; set; }

        public IList<UserLoginInfo> Logins { get; set; }

        public string PhoneNumber { get; set; }

        public bool TwoFactor { get; set; }

        public bool BrowserRemembered { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public string StatusMessageClass => StatusMessage.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0 ? "error" : "success";

        public bool ShowStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return RedirectToPage("/Error");
            }

            HasPassword = await _userManager.HasPasswordAsync(user);
            PhoneNumber = await _userManager.GetPhoneNumberAsync(user);
            TwoFactor = await _userManager.GetTwoFactorEnabledAsync(user);
            Logins = await _userManager.GetLoginsAsync(user);
            BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user);

            return Page();
        }
    }
}
