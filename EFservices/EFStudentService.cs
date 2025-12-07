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
    /// EF Core implementation of async CRUD operations for Student entities.
    /// </summary>
    public class EFStudentService : ICRUDAsync<Student>
    {
        /// <summary>
        /// intializes a new instance of the EFStudentService with the provided database context.
        /// </summary>
        private readonly EksamensDBContext _context;
        // DbContext injection
        public EFStudentService(EksamensDBContext dBContext)
        {
            _context = dBContext;
        }
        //Get all studentds with related classes and exam
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
        // Get students with optional extended filtering (name, email, id, class)
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
                // Filter by email
                if (!string.IsNullOrWhiteSpace(extendedFilter.FilterByEmail))
                {
                    var emailFilter = extendedFilter.FilterByEmail.ToLower();
                    query = query.Where(s => s.Email != null && s.Email.ToLower().Contains(emailFilter));
                }
                //Filter by exact student ID

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
                //No filter applied Get All
                return await GetAllAsync();
            }
            else
            {
                //Simple name filter 
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
        //Add a new student and save made changes
        public async Task AddItemAsync(Student item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            await _context.AddAsync(item);
            await _context.SaveChangesAsync();
        }
        // Get a single studnet by id with related classes and exam
        public async Task<Student?> GetItemByIdAsync(int id)
        {
          
            return await _context.Students
                .Include(s => s.StudentsToClasses) 
                    .ThenInclude(sc => sc.Class)    
                .Include(s => s.StudentsToExams)
                    .ThenInclude(se => se.Exam)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }
        //Delete a student by id if exsists
        public async Task DeleteItemAsync(int id)
        {
            var studentToDelete = await _context.Students.FindAsync(id);
            if (studentToDelete != null)
            {
                _context.Remove(studentToDelete);
                await _context.SaveChangesAsync();
            }
        }
        // Update scalar properties of an existing student; throws an Exception if the student does not exist
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