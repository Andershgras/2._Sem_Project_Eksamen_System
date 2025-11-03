using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Rooms
{
    public class IndexModel : PageModel
    {
        private readonly ICRUD<Room> _service;
        public IEnumerable<Room> Rooms { get; set; } = Enumerable.Empty<Room>();

        public IndexModel(ICRUD<Room> service) => _service = service;

        public void OnGet() => Rooms = _service.GetAll();
    }
}
