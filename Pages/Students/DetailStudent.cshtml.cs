using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Pages.Students
{
    public class DetailStudentModel : PageModel
    {
        private readonly ICRUDT<Student> _studentService;

        public Student Student { get; set; } = new Student();

        public DetailStudentModel(ICRUDT<Student> studentService)
        {
            _studentService = studentService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var student = await _studentService.GetItemById(id);

            if (student == null)
            {
                return NotFound();
            }

            Student = student;
            return Page();
        }
    }
}