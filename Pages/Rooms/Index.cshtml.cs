using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Rooms
{
    public class IndexModel : PageModel
    {
        private readonly ICRUD<Room> _service;
        public IEnumerable<Room> Rooms { get; set; } = Enumerable.Empty<Room>();
        [BindProperty(SupportsGet = true)]
        public GenericFilter? Filter { get; set; }
        public IndexModel(ICRUD<Room> service) => _service = service;
        public void OnGet()
        {
            if (Filter is not null && !string.IsNullOrWhiteSpace(Filter.FilterByName)
                && _service is _2._Sem_Project_Eksamen_System.EFservices.EFRoomService rooms)
            {
                Rooms = rooms.Search(Filter!);
            }
            else
            {
                Rooms = _service.GetAll();
            }
        }
    }
}
