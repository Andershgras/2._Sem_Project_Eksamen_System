using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Teachers
{
    public class DeleteModel : PageModel
    {
        private readonly ICRUD<Teacher> _service;
        public Teacher? Teacher { get; set; }

        public DeleteModel(ICRUD<Teacher> service) => _service = service;

        public IActionResult OnGet(int id)
        {
            Teacher = _service.GetItemById(id);
            if (Teacher == null) return RedirectToPage("Index");
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            _service.DeleteItem(id);
            return RedirectToPage("Index");
        }
    }
}
