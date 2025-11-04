using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Rooms
{
    public class DeleteModel : PageModel
    {
        private readonly ICRUD<Room> _service;
        public Room? Room { get; set; }

        public DeleteModel(ICRUD<Room> service) => _service = service;

        public IActionResult OnGet(int id)
        {
            Room = _service.GetItemById(id);
            if (Room == null) return RedirectToPage("Index");
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            _service.DeleteItem(id);
            return RedirectToPage("Index");
        }
    }
}
