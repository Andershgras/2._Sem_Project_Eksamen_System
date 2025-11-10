using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Interfaces;

namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class Create_ExamModel : PageModel
    {
        private readonly ICRUD<Exam> _examService;
        private readonly ICRUD<Class> _classService;
        //private readonly ICRUD<StudentsToExam> _studentsToExamService;
        private readonly IStudentsToExams _studentsToExamService;
        private readonly ICRUD<Room> _roomService;

        [BindProperty]
        public Exam Exam { get; set; } = new Exam();

        [BindProperty]
        public bool CreateReExam { get; set; } = false;

        [BindProperty]
        public Exam ReExam { get; set; } = new Exam();

        public SelectList ClassList { get; set; } = default!;

        public Create_ExamModel(
            ICRUD<Exam> examService,
            ICRUD<Class> classService,
            IStudentsToExams studentsToExamService,
            ICRUD<Room> roomService

        )
        {
            _examService = examService;
            _classService = classService;
            //_studentsToExamService = studentsToExamService;
            _studentsToExamService = studentsToExamService;
            _roomService = roomService;
        }

        public void OnGet()
        {
            ClassList = new SelectList(_classService.GetAll(), "ClassId", "ClassName");
        }

        public IActionResult OnPost()
        {
            ClassList = new SelectList(_classService.GetAll(), "ClassId", "ClassName");
            
            // Clear validation for all ReExam fields when not creating a ReExam
            if (!CreateReExam)
            {
                foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ReExam.")))
                    ModelState[key]?.Errors.Clear();
            }

            if(Exam.ExamName.Length > 30)
                Exam.ExamName = Exam.ExamName.Substring(0, 30);

            // Validate Exam dates
            if (Exam.ExamStartDate > Exam.ExamEndDate)
                ModelState.AddModelError("Exam.ExamStartDate", "Exam start date must not be after end date.");
            if (Exam.DeliveryDate > Exam.ExamStartDate)
                ModelState.AddModelError("Exam.DeliveryDate", "Delivery date must not be after start date.");

            // Handle ReExam logic if creating
            if (CreateReExam)
            {
                ReExam.ClassId = Exam.ClassId;

                if (string.IsNullOrWhiteSpace(ReExam.ExamName))
                    ReExam.ExamName = $"ReEksamen-{Exam.ExamName}";

                if(ReExam.ExamName.Length > 30)
                    ReExam.ExamName = ReExam.ExamName.Substring(0, 30);

                // Validate ReExam dates
                if (ReExam.ExamStartDate > ReExam.ExamEndDate)
                    ModelState.AddModelError("ReExam.ExamStartDate", "ReExam start date must not be after end date.");
                if (ReExam.DeliveryDate > ReExam.ExamStartDate)
                    ModelState.AddModelError("ReExam.DeliveryDate", "ReExam delivery date must not be after start date.");

                // Validate ReExam dates against Exam dates
                if (ReExam.ExamStartDate <= Exam.ExamEndDate)
                    ModelState.AddModelError("ReExam.ExamStartDate", "ReExam start date must be after main exam end date.");
                if (ReExam.ExamEndDate <= Exam.ExamEndDate)
                    ModelState.AddModelError("ReExam.ExamEndDate", "ReExam end date must be after main exam end date.");
                if (ReExam.DeliveryDate <= Exam.DeliveryDate)
                    ModelState.AddModelError("ReExam.DeliveryDate", "ReExam delivery must be after main exam delivery date.");

                ReExam.IsReExam = true;
                if (Exam.IsFinalExam)
                    ReExam.IsFinalExam = true;
            }
            else
            {
                // Unlink ReExam from Exam (just in case)
                Exam.ReExamId = null;
            }

            // Optionally clear all Class or ExamName validation errors related to ReExam if not creating
                foreach (var key in ModelState.Keys.Where(k =>
                    k == "Exam.Class" || k == "ReExam.Class" || k == "ReExam.ExamName"))
                {
                    ModelState[key]?.Errors.Clear();
                    if (ModelState[key] != null)
                        ModelState[key].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                }
            

            // (Optional Debug Logging, can comment out in production)
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

            if (!ModelState.IsValid)
                return Page();

            try
            {
                if (CreateReExam)
                {
                    _examService.AddItem(ReExam);
                    Exam.ReExamId = ReExam.ExamId;
                }

                _examService.AddItem(Exam);

                _studentsToExamService.AddStudentsFromClassToExam(Exam.ClassId, Exam.ExamId);

                TempData["SuccessMessage"] = "Exam created successfully!";
                return RedirectToPage("GetEksamner");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating exam: {ex.Message}");
                return Page();
            }
        }
    }
}