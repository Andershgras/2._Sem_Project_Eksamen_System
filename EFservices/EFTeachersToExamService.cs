using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using _2._Sem_Project_Eksamen_System.Utils;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    // Service managing Teacher <-> Exam join entries via EF Core
    public class EFTeachersToExamService : ITeachersToExam
    {
        // DbCOntext injection
        private readonly EksamensDBContext _context;

        public EFTeachersToExamService(EksamensDBContext context)
        {
            _context = context;
        }
        // Return all teacher-exam mappings with related Teacher and Exam loaded (read-only)
        public async Task<IEnumerable<TeachersToExam>> GetAllAsync()
        {
            return await _context.TeachersToExams
                .Include(te => te.Teacher)
                .Include(te => te.Exam)
                .AsNoTracking()
                .ToListAsync();
        }
        // Return mappings filtered by role substring (case-insensitive)
        public async Task<IEnumerable<TeachersToExam>> GetAllAsync(GenericFilter filter)
        {
            return await _context.TeachersToExams
                .Where(te => te.Role != null && te.Role.ToLower().Contains(filter.FilterByName.ToLower()))
                .AsNoTracking()
                .ToListAsync();
        }
        // Find a single mapping by primary key with related entities
        public async Task<TeachersToExam?> GetItemByIdAsync(int id)
        {
            return await _context.TeachersToExams
                .Include(te => te.Teacher)
                .Include(te => te.Exam)
                .FirstOrDefaultAsync(te => te.TeacherExamId == id);
        }
        // Add a mapping; ensure Role has a default value if none provided
        public async Task AddItemAsync(TeachersToExam item)
        {
            // Ensure role has a default value if not provided
            if (string.IsNullOrEmpty(item.Role))
            {
                item.Role = "Examiner";
            }

            await _context.TeachersToExams.AddAsync(item);
            await _context.SaveChangesAsync();
        }
        // Update an existing mapping; ensure Role default 
        public async Task UpdateItemAsync(TeachersToExam item)
        {
            // Ensure role has a default value if not provided
            if (string.IsNullOrEmpty(item.Role))
            {
                item.Role = "Examiner";
            }

            var existing = await _context.TeachersToExams.FindAsync(item.TeacherExamId);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
            }
        }
        // Delete a mapping by id if it exists
        public async Task DeleteItemAsync(int id)
        {
            var toDelete = await _context.TeachersToExams.FindAsync(id);
            if (toDelete != null)
            {
                _context.TeachersToExams.Remove(toDelete);
                await _context.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Add or update teacher-to-exam assignment with proper role handling
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="examId"></param>
        /// <param name="role">Optional role - defaults to "Examiner" if not provided</param>
        /// <exception cref="ArgumentException"></exception>
        public async Task AddTeachersToExamsAsync(int teacherId, int examId, string? role = null)
        {
            if (teacherId <= 0) throw new ArgumentException("teacherId must be greater than zero", nameof(teacherId));
            if (examId <= 0) throw new ArgumentException("examId must be greater than zero", nameof(examId));

            // Ensure teacher exists (defensive)
            var teacherExists = await _context.Teachers.AsNoTracking().AnyAsync(t => t.TeacherId == teacherId);
            if (!teacherExists)
                return;

            // Set default role if not provided
            string finalRole = string.IsNullOrEmpty(role) ? "Examiner" : role;

            // Check if mapping already exists
            var existingMapping = await _context.TeachersToExams
                .FirstOrDefaultAsync(tte => tte.TeacherId == teacherId && tte.ExamId == examId);

            if (existingMapping != null)
            {
                // UPDATE EXISTING: Update the role
                existingMapping.Role = finalRole;
                await _context.SaveChangesAsync();
                return;
            }

            // CREATE NEW
            var mapping = new TeachersToExam
            {
                TeacherId = teacherId,
                ExamId = examId,
                Role = finalRole
            };

            await _context.TeachersToExams.AddAsync(mapping);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<TeachersToExam>> GetTeachersByExamIdAsync(int examId)
        {
            return await _context.TeachersToExams
                .Where(tte => tte.ExamId == examId)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Remove all teacher->exam mappings for the specified exam.
        /// </summary>
        /// <param name="examId">Exam id</param>
        public async Task RemoveAllFromExamAsync(int examId)
        {
            if (examId <= 0) return;

            var items = await _context.TeachersToExams
                .Where(t => t.ExamId == examId)
                .ToListAsync();

            if (!items.Any()) return;

            _context.TeachersToExams.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
        
        public async Task FixMissingRolesAsync()
        {
            var recordsWithMissingRoles = await _context.TeachersToExams
                .Where(tte => string.IsNullOrEmpty(tte.Role))
                .ToListAsync();

            if (recordsWithMissingRoles.Any())
            {
                foreach (var record in recordsWithMissingRoles)
                {
                    record.Role = "Censor";
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Teacher>> GetTeachersByExamIdAndRoleAsync(int examId, string role)
        {
            var teachers = await _context.TeachersToExams
               .Where(tte => tte.ExamId == examId && tte.Role == role)
               .Include(tte => tte.Teacher)
               .Select(tte => tte.Teacher!)
               .AsNoTracking()
               .ToListAsync();

            return teachers;
        }
    }
}