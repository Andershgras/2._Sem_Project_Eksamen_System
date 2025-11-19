using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    // EFCore Service managing the many-to-many relationship between Students and Classes

    public class EFStudentsToClassesService : IStudentsToClasses
    {
        //DbContext Injection
        private readonly EksamensDBContext _context;

        public EFStudentsToClassesService(EksamensDBContext context)
        {
            _context = context;
        }
        // Return all student-class mappings with related Student and Class loaded
        public async Task<IEnumerable<StudentsToClass>> GetAllAsync()
        {
            return await _context.StudentsToClasses
                .Include(stc => stc.Student)
                .Include(stc => stc.Class)
                .AsNoTracking()
                .OrderBy(stc => stc.ClassId)
                .ToListAsync();
        }
        // Return mappings filtered by student name sub string
        public async Task<IEnumerable<StudentsToClass>> GetAllAsync(GenericFilter filter)
        {
            if (filter == null || string.IsNullOrWhiteSpace(filter.FilterByName))
                return await GetAllAsync();

            var term = filter.FilterByName.Trim().ToLowerInvariant();

            return await _context.StudentsToClasses
                .Include(stc => stc.Student)
                .Include(stc => stc.Class)
                .AsNoTracking()
                .Where(stc => stc.Student != null && (stc.Student.StudentName ?? string.Empty).ToLower().Contains(term))
                .OrderBy(stc => stc.ClassId)
                .ToListAsync();
        }
        // Find a single mapping by primary key with related entities (read-only)
        public async Task<StudentsToClass?> GetItemByIdAsync(int id)
        {
            return await _context.StudentsToClasses
                .Include(stc => stc.Student)
                .Include(stc => stc.Class)
                .AsNoTracking()
                .FirstOrDefaultAsync(stc => stc.StudentClassId == id);
        }
        // Add a mapping if it does notalready exsists
        public async Task AddItemAsync(StudentsToClass item)
        {
            if (item == null) return;

            // Prevent duplicate mapping
            var exists = await _context.StudentsToClasses
                .AsNoTracking()
                .AnyAsync(stc => stc.StudentId == item.StudentId && stc.ClassId == item.ClassId);

            if (exists) return;

            await _context.StudentsToClasses.AddAsync(item);
            await _context.SaveChangesAsync();
        }
        //Update scaler fielsd of an exsisting  mapping
        public async Task UpdateItemAsync(StudentsToClass item)
        {
            if (item == null) return;

            var existing = await _context.StudentsToClasses.FindAsync(item.StudentClassId);
            if (existing == null) return;

            _context.Entry(existing).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
        }
        // delete a mapping by id
        public async Task DeleteItemAsync(int id)
        {
            var entity = await _context.StudentsToClasses.FindAsync(id);
            if (entity == null) return;

            _context.StudentsToClasses.Remove(entity);
            await _context.SaveChangesAsync();
        }
        // Return mappings for a specific class (read-only)
        public async Task<IEnumerable<StudentsToClass>> GetByClassIdAsync(int classId)
        {
            if (classId <= 0) return Enumerable.Empty<StudentsToClass>();

            return await _context.StudentsToClasses
                .Include(stc => stc.Student)
                .Include(stc => stc.Class)
                .AsNoTracking()
                .Where(stc => stc.ClassId == classId)
                .OrderBy(stc => stc.StudentId)
                .ToListAsync();
        }
        // Return mappings for a specific student (read-only)
        public async Task<IEnumerable<StudentsToClass>> GetByStudentIdAsync(int studentId)
        {
            if (studentId <= 0) return Enumerable.Empty<StudentsToClass>();

            return await _context.StudentsToClasses
                .Include(stc => stc.Student)
                .Include(stc => stc.Class)
                .AsNoTracking()
                .Where(stc => stc.StudentId == studentId)
                .OrderBy(stc => stc.ClassId)
                .ToListAsync();
        }
        // Add a student to a class if both exist and mapping is not duplicate

        public async Task AddStudentToClassAsync(int studentId, int classId)
        {
            if (studentId <= 0) throw new ArgumentException("studentId must be greater than zero", nameof(studentId));
            if (classId <= 0) throw new ArgumentException("classId must be greater than zero", nameof(classId));

            var studentExists = await _context.Students.AsNoTracking().AnyAsync(s => s.StudentId == studentId);
            if (!studentExists) return;

            var classExists = await _context.Classes.AsNoTracking().AnyAsync(c => c.ClassId == classId);
            if (!classExists) return;

            var already = await _context.StudentsToClasses
                .AsNoTracking()
                .AnyAsync(stc => stc.StudentId == studentId && stc.ClassId == classId);

            if (already) return;

            var mapping = new StudentsToClass
            {
                StudentId = studentId,
                ClassId = classId
            };

            await _context.StudentsToClasses.AddAsync(mapping);
            await _context.SaveChangesAsync();
        }
        //Remove a specific student  class mapping
        public async Task RemoveStudentFromClassAsync(int studentId, int classId)
        {
            if (studentId <= 0 || classId <= 0) return;

            var mapping = await _context.StudentsToClasses
                .FirstOrDefaultAsync(stc => stc.StudentId == studentId && stc.ClassId == classId);

            if (mapping == null) return;

            _context.StudentsToClasses.Remove(mapping);
            await _context.SaveChangesAsync();
        }
        //Remove all mappings from a class
        public async Task RemoveAllFromClassAsync(int classId)
        {
            if (classId <= 0) return;

            var items = await _context.StudentsToClasses.Where(stc => stc.ClassId == classId).ToListAsync();
            if (!items.Any()) return;

            _context.StudentsToClasses.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
        // Synchronize class membership: add missing mappings and remove extra ones
        public async Task SyncStudentsToClassAsync(int classId, IEnumerable<int> studentIds)
        {
            if (classId <= 0) throw new ArgumentException("classId must be greater than zero", nameof(classId));
            if (studentIds == null) throw new ArgumentNullException(nameof(studentIds));

            var desired = new HashSet<int>(studentIds.Where(id => id > 0));
            var existing = await _context.StudentsToClasses
                .Where(stc => stc.ClassId == classId)
                .ToListAsync();

            var existingIds = existing.Select(e => e.StudentId).ToHashSet();

            // removes mapping not in desired set
            var toRemove = existing.Where(e => !desired.Contains(e.StudentId)).ToList();
            if (toRemove.Any())
                _context.StudentsToClasses.RemoveRange(toRemove);

            // adds mappings that are desired but missing
            var toAddIds = desired.Except(existingIds).ToList();
            if (toAddIds.Any())
            {
                var newMappings = toAddIds.Select(sid => new StudentsToClass
                {
                    StudentId = sid,
                    ClassId = classId
                }).ToList();

                await _context.StudentsToClasses.AddRangeAsync(newMappings);
            }

            await _context.SaveChangesAsync();
        }
        // Return students in a class (read-only). Fixed to return an empty enumerable when invalid id.
        public async Task<IEnumerable<Student>> GetStudentsFromClass(int classId)
        {
            if (classId <= 0) return []; // return empty collection

            var students = await _context.Students
                .AsNoTracking()
                .Where(s => s.StudentsToClasses.Any(stc => stc.ClassId == classId))
                .OrderBy(s => s.StudentName)
                .ToListAsync();

            return students;
        }
    }
}