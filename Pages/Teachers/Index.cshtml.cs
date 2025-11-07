using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Teachers
{
    public class IndexModel : PageModel
    {
        private readonly ICRUD<Teacher> _service;
        public IEnumerable<Teacher> Teachers { get; set; } = Enumerable.Empty<Teacher>();

        [BindProperty(SupportsGet = true)]
        public GenericFilter? Filter { get; set; }

        public IndexModel(ICRUD<Teacher> service) => _service = service;

        public void OnGet()
        {
            if (Filter is not null && !string.IsNullOrWhiteSpace(Filter.FilterByName)
                && _service is _2._Sem_Project_Eksamen_System.EFservices.EFUnderviserService svc)
            {
                Teachers = svc.GetAll(Filter!);
            }
            else
            {
                Teachers = _service.GetAll();
            }
        }
    }
}
