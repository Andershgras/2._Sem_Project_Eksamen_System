using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly EksamensDBContext _context;

        public IndexModel(ILogger<IndexModel> logger, EksamensDBContext context)
        {
            _logger = logger;
            _context = context;
        }
        // Dashboard statistics
        public int TotalStudents { get; set; }
        public int TotalExams { get; set; }
        public int UpcomingExams { get; set; }
        public int TodayExams { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalClasses { get; set; }
        public int TotalRooms { get; set; }
        // Lists for displaying data
        public List<Exam> TodayExamsList { get; set; } = new();
        public List<Exam> UpcomingExamsList { get; set; } = new();
        public void OnGet()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var nextWeek = today.AddDays(7);

            // Get counts
            TotalStudents = _context.Students.Count();
            TotalExams = _context.Exams.Count();
            TotalTeachers = _context.Teachers.Count();
            TotalClasses = _context.Classes.Count();
            TotalRooms = _context.Rooms.Count();

            // Get today's exams
            TodayExamsList = _context.Exams
                .Include(e => e.Class)
                .Include(e => e.RoomsToExams)
                    .ThenInclude(r => r.Room)
                .Where(e => e.ExamStartDate == today)
                .OrderBy(e => e.ExamStartDate)
                .Take(5)
                .AsNoTracking()
                .ToList();

            TodayExams = TodayExamsList.Count;

            // Get upcoming exams (next 7 days, excluding today)
            UpcomingExamsList = _context.Exams
                .Include(e => e.Class)
                .Include(e => e.RoomsToExams)
                    .ThenInclude(r => r.Room)
                .Where(e => e.ExamStartDate > today && e.ExamStartDate <= nextWeek)
                .OrderBy(e => e.ExamStartDate)
                .Take(5)
                .AsNoTracking()
                .ToList();

            UpcomingExams = _context.Exams
                .Where(e => e.ExamStartDate >= today)
                .Count();
        }

    }
}
