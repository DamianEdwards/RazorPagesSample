using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesSample
{
    public class IndexModel : PageModel
    {
        public string Message { get; private set; } = "Hello from the page model!";

        public void OnGet()
        {
            Message += $" Server time is { DateTime.Now }";
        }
    }
}
