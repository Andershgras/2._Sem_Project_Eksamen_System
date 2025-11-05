using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class Edit_ExamModel : PageModel
    {

        private readonly ICRUD<Exam> _service;

        [BindProperty]
        public Exam Exam { get; set; }

        public Edit_ExamModel(ICRUD<Exam> service)
        {
            _service = service;
        }

        public IActionResult OnGet(int id)
        {
            var exam = _service.GetItemById(id);
            if (exam == null) {
                return RedirectToPage("GetEksamner");
            }
            Exam = exam;
            return Page();
        }


        // Fungere ikke
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) {
                return Page();
            }
            _service.UpdateItem(Exam);
            return RedirectToPage("/Eksamner/GetEksamner");
        }
    }
}
