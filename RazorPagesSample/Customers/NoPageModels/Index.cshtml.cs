
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace RazorPages.Customers
{
    public class CustomersPageModel : PageModel
    {
        private readonly AppDbContext _db;

        public CustomersPageModel(AppDbContext db)
        {
            _db = db;
        }

        public async Task OnGetAsync()
        {
            Message = TempData[nameof(Message)]?.ToString();
            Customers = await _db.Customers.AsNoTracking().ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var customer = await _db.Customers.FindAsync(id);

            if (customer != null)
            {
                _db.Customers.Remove(customer);
                await _db.SaveChangesAsync();
            }
            
            TempData[nameof(Message)] = $"Customer {id} deleted successfully";
            //Message = $"Customer {id} deleted successfully";

            // IDEA: Have a convneience method for redirecting to yourself, e.g. Reload(), RedirectToSelf()
            // IDEA: Need overloads of Redirect that take route name/arguments to redirect to manually routed pages
            // IDEA: What can we do to make redirecting to other pages easier, without having to use URL?
            //       e.g. RedirectToPage(pagePath: "Customers/SeparatePageModels/Index.cshtml", routeArgs, new { routeArg = 1 })
            // IDEA: Trailing slash is important when redirecting/navigating to default document, we should automate somehow
            return Redirect("~/customers/separatepagemodels/");
        }

        // HACK: Dummy to provide meta-data for helpers, better way to do this?
        public Customer Customer { get; }

        public IList<Customer> Customers { get; private set; }

        // BUG: TempData attribute doesn't appear to be working
        // IDEA: Allow specifying the key for TempData so that it can be easily mirrored across multiple PageModels,
        //       e.g. when setting the message from the Edit page before redirecting back to the Index list.
        [TempData]
        public string Message { get; set; }

        public bool ShowMessage => !string.IsNullOrEmpty(Message);
    }
}
