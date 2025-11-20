using _2._Sem_Project_Eksamen_System.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Pages.Hold
{
    public class StudentsToClassesModel : PageModel
    {
        private readonly ICRUDAsync<Class> _service;
        public IEnumerable<Student> Students { get; set; }

        [BindProperty]
        public Class? ClassItem { get; set; }

        public StudentsToClassesModel(ICRUDAsync<Class> service)
        {
            _service = service;
            Students = new List<Student>();
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var classItem = await _service.GetItemByIdAsync(id);
            if (classItem == null) return RedirectToPage("Index");

            ClassItem = classItem; // Assign the retrieved class to the property with students
            Students = ClassItem.StudentsToClasses
                .Select(sc => sc.Student)
                .Where(s => s != null)
                .ToList()!;

            return Page();
        }
    }
}
