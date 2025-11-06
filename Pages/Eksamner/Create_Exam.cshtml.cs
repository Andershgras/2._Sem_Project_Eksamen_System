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
            IStudentsToExams studentsToExamService

        )
        {
            _examService = examService;
            _classService = classService;
            //_studentsToExamService = studentsToExamService;
            _studentsToExamService = studentsToExamService;
        }

        public void OnGet()
        {
            ClassList = new SelectList(_classService.GetAll(), "ClassId", "ClassName");
        }

        public IActionResult OnPost()
        {
            ClassList = new SelectList(_classService.GetAll(), "ClassId", "ClassName");

            try
            {
                // If ReExam is to be created
                if (CreateReExam)
                {
                    // Auto-fill name if not manually changed
                    if (string.IsNullOrWhiteSpace(ReExam.ExamName))
                        ReExam.ExamName = $"ReEksamen-{Exam.ExamName}";

                        ReExam.ClassId = Exam.ClassId;
                        ReExam.IsReExam = true;

                    if (Exam.IsFinalExam)
                        ReExam.IsFinalExam = true;

                    // Validate dates (ReExam dates >= Exam dates)
                    if (ReExam.ExamStartDate < Exam.ExamEndDate)
                    {
                        ModelState.AddModelError("ReExam.ExamStartDate", "ReExam start date cannot be before main Exam end date.");
                        return Page();
                    }
                    if (ReExam.ExamEndDate < Exam.ExamEndDate)
                    {
                        ModelState.AddModelError("ReExam.ExamEndDate", "ReExam end date cannot be before main Exam end date.");
                        return Page();
                    }
                    if (ReExam.DeliveryDate < Exam.DeliveryDate)
                    {
                        ModelState.AddModelError("ReExam.DeliveryDate", "ReExam delivery date cannot be before main Exam delivery date.");
                        return Page();
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
                    // Save ReExam first (so we get ReExamId)
                    _examService.AddItem(ReExam);

                    // Link main Exam to ReExam
                    Exam.ReExamId = ReExam.ExamId;
                }

                _examService.AddItem(Exam);

                // Add all students from the selected class to the newly created exam
                _studentsToExamService.AddStudentsFromClassToExam(Exam.ClassId, Exam.ExamId);


                TempData["SuccessMessage"] = "Exam created successfully!";
                return RedirectToPage("./GetEksamner");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating exam: {ex.Message}");
                return Page();
            }
        }
    }
}