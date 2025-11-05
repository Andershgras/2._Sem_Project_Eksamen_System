using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class Edit_ExamModel : PageModel
    {
        private readonly ICRUD<Exam> _service;
        private readonly ICRUD<Class> _classService;

        public SelectList ClassList { get; set; }

        [BindProperty]
        public Exam Exam { get; set; }

        [BindProperty]
        public Exam ReExam { get; set; }

        public Edit_ExamModel(ICRUD<Exam> service, ICRUD<Class> classService)
        {
            _service = service;
            _classService = classService;
        }

        public IActionResult OnGet(int id)
        {
            Exam = _service.GetItemById(id);
            if (Exam == null)
                return RedirectToPage("GetEksamner");

            if (Exam.ReExamId != null)
                ReExam = _service.GetItemById(Exam.ReExamId.Value);

            ClassList = new SelectList(_classService.GetAll(), "ClassId", "ClassName");

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ClassList = new SelectList(_classService.GetAll(), "ClassId", "ClassName");
                return Page();
            }
            _service.UpdateItem(Exam);

            if (ReExam != null && ReExam.ExamId > 0)
                _service.UpdateItem(ReExam);

            return RedirectToPage("/Eksamner/GetEksamner");
        }
    }
}