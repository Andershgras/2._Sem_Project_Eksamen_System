using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFStudentService : ICRUDT<Student>
    {
        private readonly EksamensDBContext _context;

        public EFStudentService(EksamensDBContext dBContext)
        {
            _context = dBContext;
        }

        public async Task<IEnumerable<Student>> GetAll()
        {
            return await _context.Students
                .Include(s => s.StudentsToExams)
                    .ThenInclude(se => se.Exam)
                .AsNoTracking()
                .OrderBy(s => s.StudentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetAll(GenericFilter Filter)
        {
            if (Filter == null || string.IsNullOrWhiteSpace(Filter.FilterByName))
            {
                return await GetAll();
            }

            var nameFilter = Filter.FilterByName.ToLower();
            return await _context.Students
                .Where(s => s.StudentName != null && s.StudentName.ToLower().StartsWith(nameFilter))
                .Include(s => s.StudentsToExams)
                    .ThenInclude(se => se.Exam)
                .AsNoTracking()
                .OrderBy(s => s.StudentId)
                .ToListAsync();
        }

        public async Task AddItem(Student item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            await _context.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task<Student?> GetItemById(int id)
        {
            // Use tracking because caller might update the returned entity;
            // if caller only needs read-only, consider AsNoTracking with FirstOrDefaultAsync
            return await _context.Students
                .Include(s => s.StudentsToExams)
                    .ThenInclude(se => se.Exam)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task DeleteItem(int id)
        {
            var studentToDelete = await _context.Students.FindAsync(id);
            if (studentToDelete != null)
            {
                _context.Remove(studentToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateItem(Student item)
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