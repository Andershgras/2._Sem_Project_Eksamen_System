using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    // EF Core service implementing async CRUD for Teacher entities
    public class EFTeacherService : ICRUDAsync<Teacher>
    {
        //DbContext Injection
        private readonly EksamensDBContext _context;

        public EFTeacherService(EksamensDBContext context) => _context = context;

        #region Asynchronous Methods
        // Return all teachers (read-only), ordered by primary key
        public async Task<IEnumerable<Teacher>> GetAllAsync() =>
            await _context.Teachers
                .AsNoTracking()
                .OrderBy(t => t.TeacherId)
                .ToListAsync();
        // Return teachers filtered by name or email substring (case-insensitive)
                public async Task<IEnumerable<Teacher>> GetAllAsync(GenericFilter filter)
        {
            var term = (filter?.FilterByName ?? string.Empty).Trim().ToLower();
            var query = _context.Teachers.AsNoTracking();

            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(t => t.TeacherName.ToLower().Contains(term)
                                      || (t.Email != null && t.Email.ToLower().Contains(term)));
            }

            return await query.OrderBy(t => t.TeacherId).ToListAsync();
        }
        //Find a teacher by Primary key
        public async Task<Teacher?> GetItemByIdAsync(int id) =>
            await _context.Teachers.FindAsync(id);
        //Add anew teacher and persist
        public async Task AddItemAsync(Teacher item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            await _context.Teachers.AddAsync(item);
            await _context.SaveChangesAsync();
        }
        // Update simple scalar fields of an existing teacher (Name, Email)
        public async Task UpdateItemAsync(Teacher item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var existing = await _context.Teachers.FindAsync(item.TeacherId);
            if (existing == null) return;

            existing.TeacherName = item.TeacherName;
            existing.Email = item.Email;

            await _context.SaveChangesAsync();
        }
        //Delete a teacher and remove any TeachersToExams join entires first
        public async Task DeleteItemAsync(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.TeachersToExams)
                .FirstOrDefaultAsync(t => t.TeacherId == id);

            if (teacher == null) return;

            if (teacher.TeachersToExams?.Any() == true)
            {
                _context.RemoveRange(teacher.TeachersToExams);
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
