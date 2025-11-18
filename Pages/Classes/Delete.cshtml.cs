using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Hold
{
    public class DeleteModel : PageModel
    {
        private readonly ICRUDAsync<Class> _service;
        public Class? Class { get; set; }

        public DeleteModel(ICRUDAsync<Class> service) => _service = service;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Class = await _service.GetItemByIdAsync(id);
            if (Class == null) return RedirectToPage("Index");

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int id)
        {
            await _service.DeleteItemAsync(id);
            return RedirectToPage("Index");
        }
    }
}
