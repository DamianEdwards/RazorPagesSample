using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

            // Tracking issues:
            // https://github.com/aspnet/Mvc/issues/5953
            // https://github.com/aspnet/Mvc/issues/5956
            // https://github.com/aspnet/Mvc/issues/5955
            return Redirect("~/customers/separatepagemodels/");
        }

        // HACK: Dummy to provide meta-data for helpers, better way to do this?
        public Customer Customer { get; }

        public IList<Customer> Customers { get; private set; }

        // BUG: TempData attribute doesn't appear to be working yet
        // Tracking issue: https://github.com/aspnet/Mvc/issues/5954
        [TempData]
        public string Message { get; set; }

        public bool ShowMessage => !string.IsNullOrEmpty(Message);
    }
}
