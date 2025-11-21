using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Rooms
{
    public class EditModel : PageModel
    {
        private readonly ICRUDAsync<Room> _service;
        [BindProperty] public Room Room { get; set; } = new();

        public EditModel(ICRUDAsync<Room> service) => _service = service;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var room = await _service.GetItemByIdAsync(id);
            if (room == null) return RedirectToPage("Index");
            Room = room;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync() 
        {
            await _service.UpdateItemAsync(Room);
            return RedirectToPage("Index");
        }
    }
}
