using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Hold
{
    public class DeleteModel : PageModel
    {
        private readonly ICRUD<Class> _service;
        public Class? Class { get; set; }

        public DeleteModel(ICRUD<Class> service) => _service = service;

        public IActionResult OnGet(int id)
        {
            Class = _service.GetItemById(id);
            if (Class == null) return RedirectToPage("Index");
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            _service.DeleteItem(id);
            return RedirectToPage("Index");
        }
    }
}
