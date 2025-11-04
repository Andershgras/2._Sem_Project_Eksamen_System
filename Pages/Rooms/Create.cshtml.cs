using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Rooms
{
    public class CreateModel : PageModel
    {
        private readonly ICRUD<Room> _service;
        [BindProperty] public Room Room { get; set; } = new();

        public CreateModel(ICRUD<Room> service) => _service = service;

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            _service.AddItem(Room);
            return RedirectToPage("Index");
        }
    }
}
