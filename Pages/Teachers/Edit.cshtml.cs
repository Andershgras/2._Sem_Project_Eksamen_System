using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Teachers
{
    public class EditModel : PageModel
    {
        private readonly ICRUD<Teacher> _service;
        [BindProperty] public Teacher Teacher { get; set; } = new();

        public EditModel(ICRUD<Teacher> service) => _service = service;

        public IActionResult OnGet(int id)
        {
            var t = _service.GetItemById(id);
            if (t == null) return RedirectToPage("Index");
            Teacher = t;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            _service.UpdateItem(Teacher);
            return RedirectToPage("Index");
        }
    }
}
