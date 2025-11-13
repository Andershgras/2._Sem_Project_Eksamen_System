using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Pages.Teachers
{
    public class IndexModel : PageModel
    {
        private readonly ICRUDAsync<Teacher> _service;
        private readonly EksamensDBContext _context;

        public IEnumerable<Teacher> Teachers { get; set; } = Enumerable.Empty<Teacher>();

        [BindProperty(SupportsGet = true)]
        public GenericFilter? Filter { get; set; }

        public IndexModel(ICRUDAsync<Teacher> service, EksamensDBContext context)
        {
            _service = service;
            _context = context;
        }

        public async Task OnGetAsync()
        {
            if (Filter is not null && !string.IsNullOrWhiteSpace(Filter.FilterByName))
            {
                Teachers = await _service.GetAllAsync(Filter);
            }
            else
            {
                Teachers = await _service.GetAllAsync();
            }
        }

        public async Task<IEnumerable<Exam>> GetUpcomingExamsForTeacher(int teacherId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.TeachersToExams
                .Include(tte => tte.Exam)
                    .ThenInclude(e => e.Class)
                .Where(tte => tte.TeacherId == teacherId)
                .Select(tte => tte.Exam)
                .Where(e => e.ExamStartDate == null || e.ExamStartDate >= today)
                .OrderBy(e => e.ExamStartDate)
                .ToListAsync();
        }
    }
}
