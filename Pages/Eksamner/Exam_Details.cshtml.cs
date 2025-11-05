using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Interfaces;

namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class Exam_DetailsModel : PageModel
    {
        private readonly ICRUD<Exam> _examService;

        public Exam Exam { get; set; }

        public Exam_DetailsModel(ICRUD<Exam> examService)
        {
            _examService = examService;
        }

        public IActionResult OnGet(int id)
        {
            Exam = _examService.GetItemById(id);
            // Ideally, your service returns Exam with Class, ReExam, TeachersToExams (with Teacher loaded!)
            if (Exam == null)
                return NotFound();
            return Page();
        }
    }
}