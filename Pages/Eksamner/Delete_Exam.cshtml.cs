using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class Delete_ExamModel : PageModel
    {
        private readonly ICRUD<Exam> _examService;
        private readonly EksamensDBContext _context;

        public Exam Exam { get; set; } = new Exam();

        public Delete_ExamModel(ICRUD<Exam> examService, EksamensDBContext context)
        {
            _examService = examService;
            _context = context;
        }

        public IActionResult OnGet(int id)
        {
            Exam = _context.Exams
                .Include(e => e.Class)
                .Include(e => e.ReExam)
                .Include(e => e.StudentsToExams)
                    .ThenInclude(ste => ste.Student)
                .Include(e => e.TeachersToExams)
                .Include(e => e.InverseReExam)
                .FirstOrDefault(e => e.ExamId == id);

            if (Exam == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost(int id)  // Add the id parameter here
        {
            try
            {
                // Verify the exam exists before deletion
                var examExists = _context.Exams.Any(e => e.ExamId == id);
                if (!examExists)
                {
                    ModelState.AddModelError(string.Empty, "Exam not found.");
                    return Page();
                }

                // Call the service to delete the exam
                _examService.DeleteItem(id);

                // Verify the exam was actually deleted
                var examAfterDeletion = _context.Exams.Find(id);
                if (examAfterDeletion == null)
                {
                    TempData["SuccessMessage"] = "Exam deleted successfully!";
                    return RedirectToPage("/Eksamner/GetEksamner");
                }
                else
                {
                    // If we get here, the exam wasn't deleted
                    ModelState.AddModelError(string.Empty, "Exam was not deleted. Please check the service implementation.");
                    // Reload the exam for display
                    Exam = _context.Exams
                        .Include(e => e.Class)
                        .Include(e => e.StudentsToExams)
                        .Include(e => e.TeachersToExams)
                        .FirstOrDefault(e => e.ExamId == id);
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Reload the exam for display if deletion fails
                Exam = _context.Exams
                    .Include(e => e.Class)
                    .Include(e => e.StudentsToExams)
                    .Include(e => e.TeachersToExams)
                    .FirstOrDefault(e => e.ExamId == id);

                
                ModelState.AddModelError(string.Empty, $"Could not delete exam: {ex.Message}");
                TempData["ErrorMessage"] = $"Error deleting exam: {ex.Message}";
                return Page();
            }
        }
    }
}