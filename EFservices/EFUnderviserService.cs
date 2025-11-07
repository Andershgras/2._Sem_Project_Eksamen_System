using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFUnderviserService : ICRUD<Teacher>
    {
        private readonly EksamensDBContext _context;

        public EFUnderviserService(EksamensDBContext context) => _context = context;

        public IEnumerable<Teacher> GetAll() =>
            _context.Teachers.AsNoTracking().OrderBy(t => t.TeacherId).ToList();
        public IEnumerable<Teacher> GetAll(GenericFilter filter)
        {
            var term = (filter?.FilterByName ?? string.Empty).Trim().ToLower();
            var q = _context.Teachers.AsNoTracking();
            if (!string.IsNullOrEmpty(term))
                q = q.Where(t => t.TeacherName.ToLower().Contains(term)
                              || (t.Email != null && t.Email.ToLower().Contains(term)));
            return q.OrderBy(t => t.TeacherId).ToList();
        }

        public Teacher? GetItemById(int id) => _context.Teachers.Find(id);

        public void AddItem(Teacher item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _context.Teachers.Add(item);
            _context.SaveChanges();
        }

        public void UpdateItem(Teacher item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var existing = _context.Teachers.Find(item.TeacherId);
            if (existing == null) return;

            existing.TeacherName = item.TeacherName;
            existing.Email = item.Email;

            _context.SaveChanges();
        }

        public void DeleteItem(int id)
        {
            var teacher = _context.Teachers
                .Include(t => t.TeachersToExams)
                .FirstOrDefault(t => t.TeacherId == id);
            if (teacher == null) return;

            if (teacher.TeachersToExams?.Any() == true)
                _context.RemoveRange(teacher.TeachersToExams); // undgå FK-konflikt mod join-tabel

            _context.Teachers.Remove(teacher);
            _context.SaveChanges();
        }
    }
}
