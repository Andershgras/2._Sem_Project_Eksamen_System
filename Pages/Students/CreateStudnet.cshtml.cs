using System.Threading.Tasks;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Students
{
    public class CreateStudentModel : PageModel
    {
        private readonly ICRUDT<Student> _service;

        [BindProperty]
        public Student Student { get; set; } = new Student();

        public CreateStudentModel(ICRUDT<Student> service)
        {
            _service = service;
        }

        public void OnGet()
        {
            // show empty form
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _service.AddItem(Student);
            return RedirectToPage("/Eksamner/Students/GetStudent");
        }
        
    }
}