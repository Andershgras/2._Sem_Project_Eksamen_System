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
        private readonly ICRUDAsync<Class> _classService;
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
            ICRUDAsync<Class> classService,
            IStudentsToExams studentsToExamService)
        {
            _service = service;
            _classService = classService;
            _studentsToExamService = studentsToExamService;
        }

        public async Task<IActionResult> OnGet(int id) // Changed to async
        {
            var classes = await _classService.GetAllAsync(); // Use async method
            ClassList = new SelectList(classes, "ClassId", "ClassName");

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

        public async Task<IActionResult> OnPost()
        {
            var classes = await _classService.GetAllAsync(); // Use async method
            ClassList = new SelectList(classes, "ClassId", "ClassName");

            // Clear validation for all ReExam fields when not editing/creating a ReExam
            if (!EditReExam)
            {
                foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ReExam.")))
                    ModelState[key]?.Errors.Clear();
            }

            // Truncate Exam name if too long
            if (Exam.ExamName.Length > 30)
                Exam.ExamName = Exam.ExamName.Substring(0, 30);

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
            Console.WriteLine("OnPost: in Edit_Exam");
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
            Console.WriteLine();

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