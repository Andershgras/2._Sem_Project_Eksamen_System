using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Hold
{
    public class EditModel : PageModel
    {
        private readonly ICRUD<Class> _service;
        
        [BindProperty] 
        public Class Class { get; set; }

        public EditModel(ICRUD<Class> service)
        {
            _service = service;
            Class = new Class();
        }

        public IActionResult OnGet(int id)
        {
            var _class = _service.GetItemById(id);
            if (_class == null) return RedirectToPage("Index");
            Class = _class;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            _service.UpdateItem(Class);
            return RedirectToPage("Index");
        }
    }
}
