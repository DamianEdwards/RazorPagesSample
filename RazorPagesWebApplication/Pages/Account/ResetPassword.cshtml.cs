using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebApplication.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        public IActionResult OnGet(string code = null)
        {
            return code == null ? Redirect("~/Error") : View();
        }
    }
}
