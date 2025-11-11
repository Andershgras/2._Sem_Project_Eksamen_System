using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Hold
{
    public class CreateModel : PageModel
    {
        private readonly ICRUDAsync<Class> _service;

        [BindProperty]
        public Class ClassItem { get; set; }

        public CreateModel(ICRUDAsync<Class> service)
        {
            _service = service;
            ClassItem = new Class();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _service.AddItemAsync(ClassItem);
            return RedirectToPage("/Hold/Index");
        }

        public void OnGet()
        {
            
        }
    }
}
