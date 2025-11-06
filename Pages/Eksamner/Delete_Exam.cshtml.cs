using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class Delete_ExamModel : PageModel
    {
        private readonly ICRUD<Exam> _examService;

        public Exam Exam { get; set; } = new Exam();

        public Delete_ExamModel(ICRUD<Exam> examService)
        {
            _examService = examService;
        }

        public IActionResult OnGet(int id)
        {
            Exam = _examService.GetItemById(id);

            if (Exam == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                _examService.DeleteItem(Exam.ExamId);
                TempData["SuccessMessage"] = "Exam deleted successfully!";
                return RedirectToPage("/Eksamner/GetEksamner");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Could not delete exam: {ex.Message}");
                return Page();
            }
        }
    }
}