using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Pages.Rooms
{
    public class IndexModel : PageModel
    {
        private readonly ICRUD<Room> _service;
        private readonly EksamensDBContext _db;

        public IEnumerable<Room> Rooms { get; set; } = Enumerable.Empty<Room>();

        [BindProperty(SupportsGet = true)]
        public GenericFilter? Filter { get; set; }

        public IndexModel(ICRUD<Room> service, EksamensDBContext db)
        {
            _service = service;
            _db = db;
        }

        public void OnGet()
        {
            if (Filter is not null && !string.IsNullOrWhiteSpace(Filter.FilterByName)
                && _service is _2._Sem_Project_Eksamen_System.EFservices.EFRoomService rooms)
            {
                Rooms = rooms.Search(Filter!); 
            }
            else
            {
                Rooms = _service.GetAll();
            }
        }
        public PartialViewResult OnGetUpcoming(int roomId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Hent kommende eksamener i dette rum (inkl. Class til visning)
            var upcoming = _db.Exams
                .Include(e => e.RoomsToExams)
                .Include(e => e.Class)
                .AsNoTracking()
                .Where(e =>
                    e.RoomsToExams.Any(re => re.RoomId == roomId) &&
                    e.ExamStartDate != null &&
                    e.ExamStartDate >= today)
                .OrderBy(e => e.ExamStartDate)
                .Take(10)
                .ToList();

            return Partial("_UpcomingExams", upcoming);
        }
    }
}
