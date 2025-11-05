using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Teachers
{
    public class CreateModel : PageModel
    {
        private readonly ICRUD<Teacher> _service;
        [BindProperty] public Teacher Teacher { get; set; } = new();

        public CreateModel(ICRUD<Teacher> service) => _service = service;

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            _service.AddItem(Teacher);
            return RedirectToPage("Index");
        }
    }
}
