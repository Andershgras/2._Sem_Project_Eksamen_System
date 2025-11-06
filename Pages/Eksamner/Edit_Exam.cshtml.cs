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

        public bool HasReExam => Exam.ReExamId.HasValue && Exam.ReExamId.Value > 0;

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

            if (HasReExam)
            {
                ReExam = _service.GetItemById(Exam.ReExamId.Value) ?? new Exam();
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

            // Clear validation for all ReExam fields when not editing/creating a ReExam
            if (!EditReExam)
            {
                foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ReExam.")))
                    ModelState[key]?.Errors.Clear();
                    

            }

            // Validate Exam dates
            if (Exam.ExamStartDate > Exam.ExamEndDate)
                ModelState.AddModelError("Exam.ExamStartDate", "Exam start date must not be after end date.");
            if (Exam.DeliveryDate > Exam.ExamStartDate)
                ModelState.AddModelError("Exam.DeliveryDate", "Delivery date must not be after start date.");

            // Handle ReExam create/update logic if editing/creating
            if (EditReExam)
            {
                ReExam.ClassId = Exam.ClassId;
                if (string.IsNullOrWhiteSpace(ReExam.ExamName))
                    ReExam.ExamName = $"ReEksamen-{Exam.ExamName}";

                // Validate ReExam dates
                if (ReExam.ExamStartDate > ReExam.ExamEndDate)
                    ModelState.AddModelError("ReExam.ExamStartDate", "ReExam start date must not be after end date.");
                if (ReExam.DeliveryDate > ReExam.ExamStartDate)
                    ModelState.AddModelError("ReExam.DeliveryDate", "ReExam delivery date must not be after start date.");

                // ReExam must be after main Exam
                if (ReExam.ExamStartDate <= Exam.ExamEndDate)
                    ModelState.AddModelError("ReExam.ExamStartDate", "ReExam start date must be after main exam end date.");
                if (ReExam.ExamEndDate <= Exam.ExamEndDate)
                    ModelState.AddModelError("ReExam.ExamEndDate", "ReExam end date must be after main exam end date.");
                if (ReExam.DeliveryDate <= Exam.DeliveryDate)
                    ModelState.AddModelError("ReExam.DeliveryDate", "ReExam delivery must be after main exam delivery date.");

                ReExam.IsReExam = true;
                if (Exam.IsFinalExam)
                    ReExam.IsFinalExam = true;

                // If no prior ReExam AND EditReExam is true, create a new ReExam and link it to Exam
                if (!HasReExam)
                {
                    _service.AddItem(ReExam); // creates and gets ReExam.ExamId
                    Exam.ReExamId = ReExam.ExamId;
                }
                else
                {
                    _service.UpdateItem(ReExam); // update existing re-exam
                }
            }
            else
            {
                // Unlink ReExam from Exam
                Exam.ReExamId = null;
            }

            // Log ModelState errors for debugging in console (field, state, errors)
            foreach (var entry in ModelState)
            {
                var state = entry.Value.ValidationState;
                var errorCount = entry.Value.Errors.Count;
                Console.WriteLine($"Key: {entry.Key} - State: {state} - Errors: {errorCount}");
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"  Error: {error.ErrorMessage}");
                }
            }

            if (ModelState.ErrorCount > 0)
                return Page();

            _service.UpdateItem(Exam);
            _studentsToExamService.SyncStudentsForExamAndClass(Exam.ExamId, Exam.ClassId);

            return RedirectToPage("GetEksamner");
        }
    }
}