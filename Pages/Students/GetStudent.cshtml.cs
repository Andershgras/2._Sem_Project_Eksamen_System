using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Students
{
    public class GetStudentModel : PageModel
    {
        private readonly ICRUDT<Student> _service;

        public IEnumerable<Student> Students { get; private set; } = Enumerable.Empty<Student>();
        public List<string> AvailableClasses { get; private set; } = new List<string>();

        [BindProperty(SupportsGet = true)]
        public ExtendedStudentFilter Filter { get; set; } = new ExtendedStudentFilter();

        public GetStudentModel(ICRUDT<Student> service) => _service = service;

        public async Task<IActionResult> OnGetAsync()
        {
            Students = await _service.GetAll(Filter) ?? Enumerable.Empty<Student>();

            // Populate available classes for dropdown
            await PopulateAvailableClasses();

            return Page();
        }

        private async Task PopulateAvailableClasses()
        {
            var allStudents = await _service.GetAll(new ExtendedStudentFilter());
            AvailableClasses = allStudents
                .Where(s => s.StudentsToClasses != null && s.StudentsToClasses.Any())
                .SelectMany(s => s.StudentsToClasses.Select(sc => sc.Class?.ClassName))
                .Where(className => !string.IsNullOrEmpty(className))
                .Distinct()
                .OrderBy(className => className)
                .ToList();
        }
    }
}