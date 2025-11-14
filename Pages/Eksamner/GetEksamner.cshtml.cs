using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;


namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class GetEksamnerModel : PageModel
    {
        public IEnumerable<Exam> Eksamner { get; set; }
        public IEnumerable<Exam> UpcomingExams { get; set; }
        public IEnumerable<Exam> PastExams { get; set; }
        public ICRUD<Exam> context { get; set; }


        [BindProperty(SupportsGet = true)]
        public GenericFilter? Filter { get; set; }

        public GetEksamnerModel(ICRUD<Exam> service)
        {
            context = service;
            Eksamner = new List<Exam>();
            UpcomingExams = new List<Exam>();
            PastExams = new List<Exam>();
            Filter = new GenericFilter();
        }

        public void OnGet()
        {
            // Get all exams (filtered or not)
            if (!string.IsNullOrEmpty(Filter?.FilterByName))
            {
                Eksamner = context.GetAll(Filter);
            }
            else
            {
                Eksamner = context.GetAll();
            }

            // Split exams into upcoming and past
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Upcoming exams: ExamEndDate is null, today, or in the future
            // Sorted by nearest date first (nulls at the end)
            UpcomingExams = Eksamner
                .Where(e => e.ExamEndDate == null || e.ExamEndDate >= today)
                .OrderBy(e => e.ExamStartDate ?? DateOnly.MaxValue)
                .ThenBy(e => e.ExamName)
                .ToList();

            // Past exams: ExamEndDate is before today
            // Sorted by most recent first
            PastExams = Eksamner
                .Where(e => e.ExamEndDate.HasValue && e.ExamEndDate < today)
                .OrderByDescending(e => e.ExamEndDate)
                .ThenBy(e => e.ExamName)
                .ToList();
        }
    }
}
