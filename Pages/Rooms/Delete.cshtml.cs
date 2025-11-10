using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Rooms
{
    public class DeleteModel : PageModel
    {
        private readonly ICRUDAsync<Room> _service;
        public Room? Room { get; set; }

        public DeleteModel(ICRUDAsync<Room> service) => _service = service;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Room = await _service.GetItemByIdAsync(id);
            if (Room == null) return RedirectToPage("Index");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await _service.DeleteItemAsync(id);
            return RedirectToPage("Index");
        }
    }
}
