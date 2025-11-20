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
        private readonly ICRUDAsync<Exam> _examService;
        private readonly ICRUDAsync<Class> _classService;
        private readonly ICRUDAsync<Teacher> _teacherService;
        private readonly ICRUDAsync<Room> _roomService;

        public List<string> AvailableClasses { get; private set; } = new List<string>();
        public List<string> AvailableTeachers { get; private set; } = new List<string>();
        public List<string> AvailableRooms { get; private set; } = new List<string>();
        public List<string> AvailableFormats { get; private set; } = new List<string>();

        public IEnumerable<Exam> Eksamner { get; set; }

        [BindProperty(SupportsGet = true)]
        public ExtendedExamFilter Filter { get; set; } = new ExtendedExamFilter();

        public GetEksamnerModel(ICRUDAsync<Exam> examService, ICRUDAsync<Class> classService, ICRUDAsync<Teacher> teacherService, ICRUDAsync<Room> roomService)
        {
            _examService = examService;
            _classService = classService;
            _teacherService = teacherService;
            _roomService = roomService;
            Eksamner = new List<Exam>();
        }

        public async Task OnGetAsync()
        {
            if (HasActiveFilter())
            {
                Eksamner = await _examService.GetAllAsync(Filter);
            }
            else
            {
                Eksamner = await _examService.GetAllAsync();
            }
            await PopulateDropDowns();
        }
        private bool HasActiveFilter()
        {
            return !string.IsNullOrEmpty(Filter.FilterByName)
                || !string.IsNullOrEmpty(Filter.FilterByClass)
                || !string.IsNullOrEmpty(Filter.FilterByTeacher)
                || !string.IsNullOrEmpty(Filter.FilterByRoom)
                || !string.IsNullOrEmpty(Filter.FilterByExaminerName)
                ||
                Filter.FilterByExaminerId.HasValue ||
                Filter.FilterByStartDate.HasValue ||
                Filter.FilterByEndDate.HasValue;
        }
        private async Task PopulateDropDowns()
        {
            var allClasses = await _classService.GetAllAsync();
            AvailableClasses = allClasses
                .Where(c => !string.IsNullOrEmpty(c.ClassName))
                .Select(c => c.ClassName)
                .Distinct()
                .OrderBy(name => name)
                .ToList();

            var allTeachers = await _teacherService.GetAllAsync();
            AvailableTeachers = allTeachers
               .Where(t => !string.IsNullOrEmpty(t.TeacherName))
               .Select(t => t.TeacherName)
               .Distinct()
               .OrderBy(name => name)
               .ToList();

            var allRooms = await _roomService.GetAllAsync();
            AvailableRooms = allRooms
                .Where(r => !string.IsNullOrEmpty(r.Name))
                .Select(r => r.Name)
                .Distinct()
                .OrderBy(name => name)
                .ToList();

            var allExams = await _examService.GetAllAsync();

            AvailableFormats = allExams
                .Where(e => !string.IsNullOrEmpty(e.Format))
                .Select(e => e.Format)
                .Distinct()
                .OrderBy(format => format)
                .ToList();
        }
    }
}
