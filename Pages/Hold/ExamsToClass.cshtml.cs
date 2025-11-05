using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Hold
{
    public class ExamsToClassModel : PageModel
    {
        private readonly ICRUD<Class> _classService;
        public Class ClassItem { get; set; }
        public IEnumerable<Exam> Exams { get; set; }

        public ExamsToClassModel(ICRUD<Class> classService)
        {
            _classService = classService;
            Exams = new List<Exam>();
        }

        public IActionResult OnGet(int id)
        {
            var _class = _classService.GetItemById(id); // update GetItemById to include Exams
            if (_class == null) return RedirectToPage("Index");
            ClassItem = _class;
            Exams = _class.Exams ?? new List<Exam>();
            return Page();
        }
    }
}

