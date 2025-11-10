using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Teachers
{
    public class IndexModel : PageModel
    {
        private readonly ICRUDAsync<Teacher> _service;
        public IEnumerable<Teacher> Teachers { get; set; } = Enumerable.Empty<Teacher>();

        [BindProperty(SupportsGet = true)]
        public GenericFilter? Filter { get; set; }

        public IndexModel(ICRUDAsync<Teacher> service) => _service = service;

        public async Task OnGetAsync()
        {
            if (Filter is not null && !string.IsNullOrWhiteSpace(Filter.FilterByName))
            {
                Teachers = await _service.GetAllAsync(Filter);
            }
            else
            {
                Teachers = await _service.GetAllAsync();
            }
        }
    }
}
