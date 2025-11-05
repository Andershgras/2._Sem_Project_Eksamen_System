using _2._Sem_Project_Eksamen_System.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Pages.Hold
{
    public class StudentsToClassesModel : PageModel
    {
        private readonly ICRUD<Class> _Class_service;
        public IEnumerable<Student> Students { get; set; }

        [BindProperty]
        public Class ClassItem { get; set; }

        public StudentsToClassesModel(ICRUD<Class> service)
        {
            _Class_service = service;
            Students = new List<Student>();
        }

        

        public IActionResult OnGet(int id)
        {
            var _class = _Class_service.GetItemById(id);
            if (_class == null) return RedirectToPage("Index");
            ClassItem = _class;

            Students = ClassItem.StudentsToClasses.Select(sc => sc.Student).ToList();

            return Page();
        }
    }
}
