using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Hold
{
    public class CreateModel : PageModel
    {
        private readonly ICRUD<Class> _Class_service;

        [BindProperty]
        public Class ClassItem { get; set; }

        public CreateModel(ICRUD<Class> service)
        {
            _Class_service = service;
            ClassItem = new Class();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _Class_service.AddItem(ClassItem);
            return RedirectToPage("/Hold/Index");
        }

        public void OnGet()
        {
            
        }
    }
}
