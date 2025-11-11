using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFHoldService : ICRUDAsync<Class>
    {
        EksamensDBContext _context;

        public EFHoldService(EksamensDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Class>> GetAllAsync()
        {
            return await _context.Classes
                .Include(c => c.StudentsToClasses)
                    .ThenInclude(sc => sc.Student)
                .AsNoTracking()
                .OrderBy(c => c.ClassId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Class>> GetAllAsync(GenericFilter filter)
        {
            var filterName = filter?.FilterByName?.ToLower() ?? string.Empty;

            return await _context.Classes
                .Where(c => string.IsNullOrEmpty(filterName) || c.ClassName.ToLower().StartsWith(filterName))
                .AsNoTracking()
                .OrderBy(c => c.ClassId)
                .ToListAsync();
        }
        public async Task AddItemAsync(Class item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            await _context.Classes.AddAsync(item);
            await _context.SaveChangesAsync();
        }
        public async Task<Class?> GetItemByIdAsync(int id)
        {
            return await _context.Classes
                .Include(c => c.StudentsToClasses)
                    .ThenInclude(stc => stc.Student)
                .Include(c => c.Exams)
                .FirstOrDefaultAsync(c => c.ClassId == id);
        }
        public async Task DeleteItemAsync(int id)
        {
            var item = await GetItemByIdAsync(id);
            if (item != null)
            {
                _context.Classes.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateItemAsync(Class item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _context.Classes.Update(item);
            await _context.SaveChangesAsync();
        }
    }
}
