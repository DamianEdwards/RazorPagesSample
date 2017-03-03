using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace RazorPages.Customers
{
    public class EditCustomerPageModel : PageModel
    {
        private readonly AppDbContext _db;

        public EditCustomerPageModel(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Customer = await _db.Customers.FindAsync(id);

            if (Customer == null)
            {
                TempData[nameof(CustomersPageModel.Message)] = $"Customer {id} not found!";
                return Redirect("~/customers/separatepagemodels/");
            }

            return View();
        }

        public async Task<IActionResult> OnPostAsync(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _db.Attach(customer).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
                TempData[nameof(CustomersPageModel.Message)] = "Customer updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData[nameof(CustomersPageModel.Message)] = $"Customer {customer.Id} not found!";
            }

            return Redirect("~/customers/separatepagemodels/");
        }

        public Customer Customer { get; private set; }
    }
}
