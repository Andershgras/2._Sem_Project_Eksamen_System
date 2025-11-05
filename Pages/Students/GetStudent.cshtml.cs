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

        [BindProperty(SupportsGet = true)]
        public GenericFilter Filter { get; set; } = new GenericFilter();

        public GetStudentModel(ICRUDT<Student> service)
        {
            _service = service;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Students = await _service.GetAll(Filter);
            if (Students == null)
            {
                Students = Enumerable.Empty<Student>();
            }

            return Page();
        }
    }
}