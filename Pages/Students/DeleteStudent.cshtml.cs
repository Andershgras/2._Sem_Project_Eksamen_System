using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Pages.Students
{
    public class DeleteStudentModel : PageModel
    {
        private readonly ICRUDAsync<Student> _studentService;

        [BindProperty]
        public Student Student { get; set; } = new Student();//To fetch the complete student data before deletion

        [BindProperty]
        public int StudentId { get; set; }//Added this just to Delete by Id

        public DeleteStudentModel(ICRUDAsync<Student> studentService)
        {
            _studentService = studentService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var student = await _studentService.GetItemByIdAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            StudentId = student.StudentId;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _studentService.DeleteItemAsync(Student.StudentId);
                //Here we added Success Message
                TempData["SuccessMessage"] = "Student is Deleted Successfully.";
                return RedirectToPage("/Students/GetStudent");
            }
            catch
            {
                // Handle any errors (e.g., database constraints)
                ModelState.AddModelError(string.Empty, "Kunne ikke slette studenten. Tjek om studenten har tilknyttede eksaminer.");
                return Page();
            }
        }
    }
}