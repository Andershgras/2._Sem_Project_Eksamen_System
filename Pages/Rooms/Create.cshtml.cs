using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Rooms
{
    public class CreateModel : PageModel
    {
        private readonly ICRUDAsync<Room> _service;
        [BindProperty] public Room Room { get; set; } = new();

        public CreateModel(ICRUDAsync<Room> service) => _service = service;

        public void OnGetAsync() { }

        public IActionResult OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            _service.AddItemAsync(Room);
            return RedirectToPage("Index");
        }
    }
}
