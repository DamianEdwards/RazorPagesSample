using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPages
{
    public class IndexPageModel : PageModel
    {
        public void OnGet()
        {
            Message += $" Server time is { DateTime.Now.ToShortTimeString() }";
        }

        public string Message { get; private set; } = "Hello from the page model!";
    }
}
