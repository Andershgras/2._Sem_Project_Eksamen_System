using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class Exam_DetailsModel : PageModel
    {
        private readonly ICRUDAsync<Exam> _examService;
        private readonly EksamensDBContext _context;

        public Exam ? Exam { get; set; }

        public Exam_DetailsModel(ICRUDAsync<Exam> examService, EksamensDBContext context)
        {
            _examService = examService;
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            //I  Used the same pattern as our Delete page, but add Teacher inclusion
            Exam = await _context.Exams
                .Include(e => e.Class)
                .Include(e => e.ReExam)
                .Include(e => e.StudentsToExams)
                .ThenInclude(ste => ste.Student)
                .Include(e => e.TeachersToExams) // CRITICAL: Include TeachersToExams
                .ThenInclude(tte => tte.Teacher) // CRITICAL: Include Teacher details
                .Include(e => e.RoomsToExams)
                .ThenInclude(rte => rte.Room)
                .Include(e => e.InverseReExam)
                .FirstOrDefaultAsync(e => e.ExamId == id);

            if (Exam == null)
                return NotFound();

            return Page();
        }
    }
}