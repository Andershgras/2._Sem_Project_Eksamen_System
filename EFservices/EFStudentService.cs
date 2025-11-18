using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    /// <summary>
    /// This service provides asynchronous CRUD operations for Student entities using Entity Framework.
    /// </summary>
    public class EFStudentService : ICRUDAsync<Student>
    {
        /// <summary>
        /// intializes a new instance of the EFStudentService with the provided database context.
        /// </summary>
        private readonly EksamensDBContext _context;

        public EFStudentService(EksamensDBContext dBContext)
        {
            _context = dBContext;
        }
        /// </summary>
        /// Retrieves a student by their unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the student.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the student if found; otherwise, null.</returns>

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students
                // Includes students to class 
                .Include(s => s.StudentsToClasses) 
                 .ThenInclude(sc => sc.Class)    
                .Include(s => s.StudentsToExams)
                 .ThenInclude(se => se.Exam)
                .AsNoTracking()
                .OrderBy(s => s.StudentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetAllAsync(GenericFilter Filter)
        {
            if (Filter is ExtendedStudentFilter extendedFilter)
            {
                var query = _context.Students.AsQueryable();

                // Apply multiple filters
                if (!string.IsNullOrWhiteSpace(extendedFilter.FilterByName))
                {
                    var nameFilter = extendedFilter.FilterByName.ToLower();
                    query = query.Where(s => s.StudentName != null && s.StudentName.ToLower().Contains(nameFilter));
                }

                if (!string.IsNullOrWhiteSpace(extendedFilter.FilterByEmail))
                {
                    var emailFilter = extendedFilter.FilterByEmail.ToLower();
                    query = query.Where(s => s.Email != null && s.Email.ToLower().Contains(emailFilter));
                }

                if (extendedFilter.FilterById.HasValue && extendedFilter.FilterById > 0)
                {
                    query = query.Where(s => s.StudentId == extendedFilter.FilterById.Value);
                }

                // Apply class filter - this is more complex due to the many-to-many relationship
                if (!string.IsNullOrWhiteSpace(extendedFilter.FilterByClass))
                {
                    query = query.Where(s => s.StudentsToClasses.Any(sc =>
                        sc.Class != null && sc.Class.ClassName == extendedFilter.FilterByClass));
                }

                    return await query
                    .Include(s => s.StudentsToClasses)
                     .ThenInclude(sc => sc.Class)
                    .Include(s => s.StudentsToExams)
                      .ThenInclude(se => se.Exam)
                    .AsNoTracking()
                    .OrderBy(s => s.StudentId)
                    .ToListAsync();
            }
            else if (Filter == null || string.IsNullOrWhiteSpace(Filter.FilterByName))
            {
                return await GetAllAsync();
            }
            else
            {
                var nameFilter = Filter.FilterByName.ToLower();
                return await _context.Students
                    .Where(s => s.StudentName != null && s.StudentName.ToLower().Contains(nameFilter))
                    .Include(s => s.StudentsToClasses)
                        .ThenInclude(sc => sc.Class)
                    .Include(s => s.StudentsToExams)
                        .ThenInclude(se => se.Exam)
                    .AsNoTracking()
                    .OrderBy(s => s.StudentId)
                    .ToListAsync();
            }
        }

        public async Task AddItemAsync(Student item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            await _context.AddAsync(item);
            await _context.SaveChangesAsync();
        }
        public async Task<Student?> GetItemByIdAsync(int id)
        {
            // Use tracking because caller might update the returned entity;
            // if caller only needs read-only, consider AsNoTracking with FirstOrDefaultAsync
            return await _context.Students
                .Include(s => s.StudentsToClasses)  // ADD THIS LINE
                    .ThenInclude(sc => sc.Class)    // ADD THIS LINE
                .Include(s => s.StudentsToExams)
                    .ThenInclude(se => se.Exam)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task DeleteItemAsync(int id)
        {
            var studentToDelete = await _context.Students.FindAsync(id);
            if (studentToDelete != null)
            {
                _context.Remove(studentToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateItemAsync(Student item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var existing = await _context.Students.FindAsync(item.StudentId);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Optionally: throw or add new. Here we choose to throw to signal missing entity.
                throw new InvalidOperationException($"Student with id {item.StudentId} not found.");
            }
        }
    }
}