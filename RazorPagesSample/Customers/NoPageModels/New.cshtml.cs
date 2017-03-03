using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPages.Customers
{
    public class NewCustomerPageModel : PageModel
    {
        private readonly AppDbContext _db;

        public NewCustomerPageModel(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnPostAsync(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            PageContext.TempData[nameof(CustomersPageModel.Message)] = "New customer created successfully!";

            return Redirect("~/customers/separatepagemodels/");
        }

        public Customer Customer { get; }
    }
}
