using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using _2._Sem_Project_Eksamen_System.Utils;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFTeachersToExamService : ITeachersToExam
    {
        private readonly EksamensDBContext _context;

        public EFTeachersToExamService(EksamensDBContext context)
        {
            _context = context;
        }

        public IEnumerable<TeachersToExam> GetAll()
        {
            // Returns all teacher-to-exam assignments, with teacher and exam 
            return _context.TeachersToExams
                .Include(te => te.Teacher)
                .Include(te => te.Exam)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<TeachersToExam> GetAll(GenericFilter filter)
        {
            // Example: filter by role or teacher or exam name if needed; left basic here
            return _context.TeachersToExams // needs implementation in GernericFilter to make this work properly
                .Where(te => te.Role != null && te.Role.ToLower().Contains(filter.FilterByName.ToLower()))
                .AsNoTracking()
                .ToList();
        }

        public TeachersToExam? GetItemById(int id)
        {
            return _context.TeachersToExams
                .Include(te => te.Teacher)
                .Include(te => te.Exam)
                .FirstOrDefault(te => te.TeacherExamId == id);
        }

        public void AddItem(TeachersToExam item)
        {
            // Ensure role has a default value if not provided
            if (string.IsNullOrEmpty(item.Role))
            {
                item.Role = "Examiner";
            }

            _context.TeachersToExams.Add(item);
            _context.SaveChanges();
        }

        public void UpdateItem(TeachersToExam item)
        {
            // Ensure role has a default value if not provided
            if (string.IsNullOrEmpty(item.Role))
            {
                item.Role = "Examiner";
            }

            var existing = _context.TeachersToExams.Find(item.TeacherExamId);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(item);
                _context.SaveChanges();
            }
        }

        public void DeleteItem(int id)
        {
            var toDelete = _context.TeachersToExams.Find(id);
            if (toDelete != null)
            {
                _context.TeachersToExams.Remove(toDelete);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Add or update teacher-to-exam assignment with proper role handling
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="examId"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddTeachersToExams(int teacherId, int examId)
        {
            if (teacherId <= 0) throw new ArgumentException("teacherId must be greater than zero", nameof(teacherId));
            if (examId <= 0) throw new ArgumentException("examId must be greater than zero", nameof(examId));

            // Ensure teacher exists (defensive)
            var teacherExists = _context.Teachers.AsNoTracking().Any(t => t.TeacherId == teacherId);
            if (!teacherExists)
                return;

            // Check if mapping already exists
            var existingMapping = _context.TeachersToExams
                .FirstOrDefault(tte => tte.TeacherId == teacherId && tte.ExamId == examId);

            if (existingMapping != null)
            {
                // UPDATE EXISTING: Always ensure role has a value
                if (string.IsNullOrEmpty(existingMapping.Role))
                {
                    existingMapping.Role = "Examiner";
                    // No need for explicit Update() - Entity Framework tracks changes
                    _context.SaveChanges();
                }
                return;
            }

            // CREATE NEW: Only create new mapping if it doesn't exist
            var mapping = new TeachersToExam
            {
                TeacherId = teacherId,
                ExamId = examId,
                Role = "Examiner" // Default role
            };

            _context.TeachersToExams.Add(mapping);
            _context.SaveChanges();
        }

        /// <summary>
        /// Remove all teacher->exam mappings for the specified exam.
        /// </summary>
        /// <param name="examId">Exam id</param>
        public void RemoveAllFromExam(int examId)
        {
            if (examId <= 0) return;

            var items = _context.TeachersToExams.Where(t => t.ExamId == examId).ToList();
            if (!items.Any()) return;

            _context.TeachersToExams.RemoveRange(items);
            _context.SaveChanges();
        }

        /// <summary>
        /// NEW METHOD: Bulk update roles for existing records
        /// Call this once to fix all existing null/empty roles
        /// </summary>
        public void FixMissingRoles()
        {
            var recordsWithMissingRoles = _context.TeachersToExams
                .Where(tte => string.IsNullOrEmpty(tte.Role))
                .ToList();

            if (recordsWithMissingRoles.Any())
            {
                foreach (var record in recordsWithMissingRoles)
                {
                    record.Role = "Censor";
                }
                _context.SaveChanges();
            }
        }
    }
}