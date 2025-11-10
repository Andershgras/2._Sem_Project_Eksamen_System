using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Teachers
{
    public class EditModel : PageModel
    {
        private readonly ICRUDAsync<Teacher> _service;
        [BindProperty] public Teacher Teacher { get; set; } = new();

        public EditModel(ICRUDAsync<Teacher> service) => _service = service;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var teacher = await _service.GetItemByIdAsync(id);
            if (teacher == null) return RedirectToPage("Index");

            Teacher = teacher;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            await _service.UpdateItemAsync(Teacher);
            return RedirectToPage("Index");
        }
    }
}
