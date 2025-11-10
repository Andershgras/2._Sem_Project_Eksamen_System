using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Teachers
{
    public class DeleteModel : PageModel
    {
        private readonly ICRUDAsync<Teacher> _service;
        public Teacher? Teacher { get; set; }

        public DeleteModel(ICRUDAsync<Teacher> service) => _service = service;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Teacher = await _service.GetItemByIdAsync(id);
            if (Teacher == null) return RedirectToPage("Index");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await _service.DeleteItemAsync(id);
            return RedirectToPage("Index");
        }
    }
}
