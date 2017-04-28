using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RazorPagesSample.Data;

namespace RazorPagesSample.Pages.Customers.SeparatePageModels
{
    public class NewModel : PageModel
    {
        private readonly AppDbContext _db;

        public NewModel(AppDbContext db)
        {
            _db = db;
        }
        
        [BindProperty]
        public Customer Customer { get; set; }

        [TempData]
        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _db.Customers.Add(Customer);
            await _db.SaveChangesAsync();

            Message = "New customer created successfully!";

            return RedirectToPage("./Index");
        }
    }
}
