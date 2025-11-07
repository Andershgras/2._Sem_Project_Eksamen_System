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
        public Exam ReExam { get; set; }

        public bool HasReExam => Exam.ReExamId.HasValue;

        [BindProperty]
        public bool EditReExam { get; set; }

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
                ReExam = _service.GetItemById(Exam.ReExamId.Value);
                EditReExam = true;
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
                // Always set ReExam.ClassId from Exam.ClassId!
                ReExam.ClassId = Exam.ClassId;

                if (string.IsNullOrWhiteSpace(ReExam.ExamName))
                    ReExam.ExamName = $"ReEksamen-{Exam.ExamName}";

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

                if (!HasReExam) // Create new ReExam and link it
                {
                    _service.AddItem(ReExam);
                    Exam.ReExamId = ReExam.ExamId;
                    
                }
                else // Update existing ReExam
                {
                    _service.UpdateItem(ReExam);
                }
            }
            else
            {
                // Unlink ReExam from Exam
                Exam.ReExamId = null;
            }

            
            
                foreach (var key in ModelState.Keys.Where(k => k.Equals("Exam.Class") || k.Equals("ReExam.Class") || k.Equals("ReExam.ExamName"))) // Removes all Class related errors så Make sure it is vali
                {
                    ModelState[key]?.Errors.Clear();
                    if (ModelState[key] != null) // nessesary null check to avoid the unvalidated warning
                    ModelState[key].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                }
            

            //Log ModelState errors for debugging in console
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

            Console.WriteLine("-----");
            Console.WriteLine($"{EditReExam} | Exam.ReExamId = {Exam.ReExamId} | ReExamen.ExamenId = {ReExam.ExamId}");

            _service.UpdateItem(Exam);
            _studentsToExamService.SyncStudentsForExamAndClass(Exam.ExamId, Exam.ClassId);

            // Redirect to details page
            return RedirectToPage("GetEksamner");
        }
    }
}