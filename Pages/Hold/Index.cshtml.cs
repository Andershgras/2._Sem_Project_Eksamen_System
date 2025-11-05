using _2._Sem_Project_Eksamen_System.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Pages.NewFolder
{
    public class IndexModel : PageModel
    {
        private readonly ICRUD<Class> _service;
        public IEnumerable<Class> HoldList { get; set; }

        [BindProperty(SupportsGet = true)]
        public GenericFilter Filter { get; set; }

        [BindProperty(SupportsGet = true)]
        public GenericFilter Filter1 { get; set; }

        public IndexModel(ICRUD<Class> service)
        {
            _service = service;
            Filter = new GenericFilter();
            HoldList = new List<Class>();
        }
        public void OnGet()
        {
            if (!string.IsNullOrEmpty(Filter.FilterByName))
            {
                HoldList = _service.GetAll(Filter);
            }
            else
            {
                HoldList = _service.GetAll();
            }
        }
    }
}
