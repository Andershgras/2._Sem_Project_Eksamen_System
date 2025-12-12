using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Pages.Students
{
    /// <summary>
    /// Displays detailed information for a single student.
    /// This is a read-only view showing all student data including related entities.
    /// StudentService.GetItemByIdAsync includes related Classes and Exams.
    /// </summary>
    public class DetailStudentModel : PageModel
    {
        private readonly ICRUDAsync<Student> _studentService;
        /// <summary>
        /// The student to display. Will be null until loaded by OnGetAsync.
        /// Related entities (Classes, Exams) should be included by the service.
        /// </summary>

        public Student? Student { get; set; }

        public DetailStudentModel(ICRUDAsync<Student> studentService)
        {
            _studentService = studentService;
        }
        /// <summary>
        /// Loads and displays a student by ID.
        /// </summary>
        /// <param name="id">Student ID (must be positive integer)</param>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            if(id <= 0)
                return BadRequest("Student ID must be greater than zero.");
           
            var student = await _studentService.GetItemByIdAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            Student = student;
            return Page();
        }
    }
}