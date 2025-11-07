using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.EntityFrameworkCore;


namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFRoomsToExamService : IRoomsToExams
    {
        private readonly EksamensDBContext _context;

        public EFRoomsToExamService(EksamensDBContext context)
        {
            _context = context;
        }

        public IEnumerable<RoomsToExam> GetAll()
        {
            return _context.RoomsToExams
                .Include(rte => rte.Room)
                .Include(rte => rte.Exam)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<RoomsToExam> GetAll(GenericFilter filter)
        {
            var term = (filter?.FilterByName ?? string.Empty).ToLower();
            return _context.RoomsToExams
                .Include(rte => rte.Room)
                .Include(rte => rte.Exam)
                .Where(rte => rte.Role != null && rte.Role.ToLower().Contains(term))
                .AsNoTracking()
                .ToList();
        }

        public RoomsToExam? GetItemById(int id)
        {
            return _context.RoomsToExams
                .Include(rte => rte.Room)
                .Include(rte => rte.Exam)
                .FirstOrDefault(rte => rte.RoomExamId == id);
        }

        public void AddItem(RoomsToExam item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _context.RoomsToExams.Add(item);
            _context.SaveChanges();
        }

        public void UpdateItem(RoomsToExam item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var existing = _context.RoomsToExams.Find(item.RoomExamId);
            if (existing == null) return;

            existing.RoomId = item.RoomId;
            existing.ExamId = item.ExamId;
            existing.Role = item.Role;

            _context.SaveChanges();
        }

        public void DeleteItem(int id)
        {
            var entity = _context.RoomsToExams.Find(id);
            if (entity == null) return;
            _context.RoomsToExams.Remove(entity);
            _context.SaveChanges();
        }

        /// <summary>
        /// Add multiple room assignments to an exam in one call.
        /// Existing assignments are not removed by this method.
        /// </summary>
        public void AddRoomsToExam(int examId, IEnumerable<RoomsToExam> assignments)
        {
            if (assignments == null) return;

            var list = assignments
                .Where(a => a != null)
                .Select(a =>
                {
                    a.ExamId = examId;
                    return a;
                })
                .ToList();

            if (!list.Any()) return;

            _context.RoomsToExams.AddRange(list);
            _context.SaveChanges();
        }
    }
}