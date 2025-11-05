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

        // Dropdown for Class
        public SelectList ClassList { get; set; } = default!;

        [BindProperty]
        public Exam Exam { get; set; }

        [BindProperty]
        public Exam ReExam { get; set; } = new Exam();

        [BindProperty]
        public bool EditReExam { get; set; } = false;

        public Edit_ExamModel(ICRUD<Exam> service, ICRUD<Class> classService)
        {
            _service = service;
            _classService = classService;
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
            }
            else
            {
                ReExam = new Exam(); // Not linked
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            ClassList = new SelectList(_classService.GetAll(), "ClassId", "ClassName");

            // Check EditReExam
            if (EditReExam)
            {
                // If there is NO linked ReExam yet
                if (!Exam.ReExamId.HasValue || Exam.ReExamId.Value == 0)
                {
                    // Save new ReExam
                    _service.AddItem(ReExam);
                    // Link to main Exam
                    Exam.ReExamId = ReExam.ExamId;
                }
                else
                {
                    // Edit existing linked ReExam
                    if (ReExam != null && ReExam.ExamId > 0)
                        _service.UpdateItem(ReExam);
                }
            }
            else
            {
                // If unchecked, remove reference to ReExam
                Exam.ReExamId = null;
            }

            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging:
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(modelError.ErrorMessage); // Or use logger/debug 
                }
                return Page();
            }
            // Save main Exam (with updated ReExamId if any)
            _service.UpdateItem(Exam);

            // Redirect to Exam details page
            return RedirectToPage("/Eksamner/Exam_Details", new { id = Exam.ExamId }); // Update to your actual details page
        }
    }
}