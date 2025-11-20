using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Hold
{
    public class ExamsToClassModel : PageModel
    {
        private readonly ICRUDAsync<Class> _service;
        public Class? ClassItem { get; set; }
        public IEnumerable<Exam> Exams { get; set; }

        public ExamsToClassModel(ICRUDAsync<Class> service)
        {
            _service = service;
            Exams = new List<Exam>();
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var classItem = await _service.GetItemByIdAsync(id);
            if (classItem == null) return RedirectToPage("Index");

            ClassItem = classItem;
            Exams = ClassItem.Exams ?? new List<Exam>(); // Ensure Exams is not null

            return Page();
        }
    }
}

