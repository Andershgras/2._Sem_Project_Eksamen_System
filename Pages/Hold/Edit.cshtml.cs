using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Hold
{
    public class EditModel : PageModel
    {
        private readonly ICRUDAsync<Class> _service;

        [BindProperty] 
        public Class Class { get; set; }

        public EditModel(ICRUDAsync<Class> service)
        {
            _service = service;
            Class = new Class();
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var classItem = await _service.GetItemByIdAsync(id);
            if (classItem == null) return RedirectToPage("Index");

            Class = classItem;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            await _service.UpdateItemAsync(Class);
            return RedirectToPage("Index");
        }
    }
}
