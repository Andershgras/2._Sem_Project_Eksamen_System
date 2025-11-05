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
                // Optionally, you can pre-toggle the ReExam edit box
                // EditReExam = true;
            }
            else
            {
                ReExam = new Exam();
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            ClassList = new SelectList(_classService.GetAll(), "ClassId", "ClassName");

            // Conditionally ignore validation errors from nested ReExam model
            if (!EditReExam)
            {
                foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ReExam.")))
                    ModelState[key].Errors.Clear();
            }

            if (!ModelState.IsValid)
                return Page();

            // Save and link ReExam if section is ON
            if (EditReExam)
            {
                // If creating a new ReExam (no ID set yet)
                if (!Exam.ReExamId.HasValue || Exam.ReExamId.Value == 0)
                {
                    // Copy relevant fields from Exam to ReExam
                    ReExam.ClassId = Exam.ClassId;
                    if (string.IsNullOrWhiteSpace(ReExam.ExamName))
                        ReExam.ExamName = $"ReEksamen-{Exam.ExamName}";
                    // Set flags if desired
                    ReExam.IsReExam = true;
                    if (Exam.IsFinalExam)
                        ReExam.IsFinalExam = true;
                    // You can add other auto-fill logic as needed

                    // Save new ReExam and link it
                    _service.AddItem(ReExam);
                    Exam.ReExamId = ReExam.ExamId;
                }
                else
                {
                    // Update existing linked ReExam
                    _service.UpdateItem(ReExam);
                }
            }
            else
            {
                // Unlink ReExam if unchecked
                Exam.ReExamId = null;
            }

            // Save main Exam (with updated ReExamId if any)
            _service.UpdateItem(Exam);

            // Redirect to Exam details page (or list)
            return RedirectToPage("/Eksamner/Exam_Details", new { id = Exam.ExamId });
        }
    }
}