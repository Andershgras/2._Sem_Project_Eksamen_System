using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    /// <summary>
    /// EFCore backed implimentation of ICRUDAsync for class entities
    /// Query classes and related navigation
    /// CRUD class entities
    /// Class specific read Helperes metods
    /// All read operations use AsNoTracking for read only operations
    /// update and save changes
    /// </summary>
    public class EFClassService : ICRUDAsync<Class>
    {
        //DbContextext injected via DI
        EksamensDBContext _context;

        public EFClassService(EksamensDBContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Returns all classes 
        /// Order by classID to provide deterministic ordering for callers
        /// </summary>
        /// <returns></returns>

        public async Task<IEnumerable<Class>> GetAllAsync()
        {
            return await _context.Classes
                .Include(c => c.StudentsToClasses)
                .ThenInclude(sc => sc.Student)
                .AsNoTracking()
                .OrderBy(c => c.ClassId)
                .ToListAsync();
        }
        /// <summary>
        /// Returns class filte by name
        /// <param name="filter"></param>
        ///  /// </summary>
        public async Task<IEnumerable<Class>> GetAllAsync(GenericFilter filter)
        {
            // keeping orignal null handling
            var filterName = filter?.FilterByName?.ToLower() ?? string.Empty;

            return await _context.Classes
                .Where(c => string.IsNullOrEmpty(filterName) || c.ClassName.ToLower().StartsWith(filterName))
                .AsNoTracking()
                .OrderBy(c => c.ClassId)
                .ToListAsync();
        }
        /// <summary>
        /// Adds a class entity and persisit changes
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task AddItemAsync(Class item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            await _context.Classes.AddAsync(item);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Loads a class by id with StudentsTo Class --> Student to exam naviagation properties
        /// return null if not found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Class?> GetItemByIdAsync(int id)
        {
            return await _context.Classes
                .Include(c => c.StudentsToClasses)
                    .ThenInclude(stc => stc.Student)
                .Include(c => c.Exams)
                .FirstOrDefaultAsync(c => c.ClassId == id);
        }
        /// <summary>
        /// Delete item if found
        /// Note: this will cascade-delete or throw if DB constraints prevent deletion.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteItemAsync(int id)
        {
            var item = await GetItemByIdAsync(id);
            if (item != null)
            {
                _context.Classes.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Updates the provided class entitiy
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task UpdateItemAsync(Class item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _context.Classes.Update(item);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Gget exams for a specific class
        /// Returns exams for a specific class,orderd by start date
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>

        public async Task<IEnumerable<Exam>> GetExamsForClassAsync(int classId)
        {
            return await _context.Exams
                .Where(e => e.ClassId == classId)
                .AsNoTracking()
                .OrderBy(e => e.ExamStartDate)
                .ToListAsync();
        }
    }
}
