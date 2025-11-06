using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class Edit_ExamModel : PageModel
    {
        private readonly ICRUD<Exam> _service;
        private readonly ICRUD<Class> _classService;
        private readonly IStudentsToExams _studentsToExamService;

        public SelectList ClassList { get; set; } = default!;

        [BindProperty]
        public Exam Exam { get; set; }

        [BindProperty]
        public Exam ReExam { get; set; } = new Exam();

        [BindProperty]
        public bool EditReExam { get; set; } = false;

        public Edit_ExamModel(
            ICRUD<Exam> service,
            ICRUD<Class> classService,
            IStudentsToExams studentsToExamService)
        {
            _service = service;
            _classService = classService;
            _studentsToExamService = studentsToExamService;
        }

        public IActionResult OnGet(int id)
        {
            ClassList = new SelectList(_classService.GetAll(), "ClassId", "ClassName");

            Exam = _service.GetItemById(id);
            if (Exam == null)
                return RedirectToPage("GetEksamner");

            if (Exam.ReExamId.HasValue)
            {
                ReExam = _service.GetItemById(Exam.ReExamId.Value) ?? new Exam();
                EditReExam = true; // auto-toggle section if ReExam exists
            }
            else
            {
                ReExam = new Exam();
                EditReExam = false;
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            ClassList = new SelectList(_classService.GetAll(), "ClassId", "ClassName");

            // Clear validation for all ReExam fields when not editing/creating a ReExam
            if (!EditReExam)
            {
                foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ReExam.")))
                    ModelState[key]?.Errors.Clear();
            }

            //// Now validate the rest
            //if (!ModelState.IsValid)  // Problematic validation causes issues
            //    return Page();

            // Handle ReExam create/update logic if editing/creating
            if (EditReExam)
            {
                // Always set ReExam.ClassId from Exam.ClassId!
                ReExam.ClassId = Exam.ClassId;

                if (string.IsNullOrWhiteSpace(ReExam.ExamName))
                    ReExam.ExamName = $"ReEksamen-{Exam.ExamName}";

                ReExam.IsReExam = true;
                if (Exam.IsFinalExam)
                    ReExam.IsFinalExam = true;

                if (!Exam.ReExamId.HasValue || Exam.ReExamId.Value == 0)
                {
                    _service.AddItem(ReExam);
                    Exam.ReExamId = ReExam.ExamId;
                }
                else
                {
                    _service.UpdateItem(ReExam);
                }
            }
            else
            {
                // Unlink ReExam from Exam
                Exam.ReExamId = null;
            }

            _service.UpdateItem(Exam);
            _studentsToExamService.SyncStudentsForExamAndClass(Exam.ExamId, Exam.ClassId);

            // Redirect to details page
            return RedirectToPage("GetEksamner");
        }
    }
}