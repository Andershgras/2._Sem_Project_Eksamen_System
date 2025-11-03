using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Rooms
{
    public class EditModel : PageModel
    {
        private readonly ICRUD<Room> _service;
        [BindProperty] public Room Room { get; set; } = new();

        public EditModel(ICRUD<Room> service) => _service = service;

        public IActionResult OnGet(int id)
        {
            var room = _service.GetItemById(id);
            if (room == null) return RedirectToPage("Index");
            Room = room;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            _service.UpdateItem(Room);
            return RedirectToPage("Index");
        }
    }
}
