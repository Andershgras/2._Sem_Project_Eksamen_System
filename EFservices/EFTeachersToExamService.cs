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
            _context.TeachersToExams.Add(item);
            _context.SaveChanges();
        }

        public void UpdateItem(TeachersToExam item)
        {
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

        public void AddTeachersToExams(int teacherId, int examId)
        {
            if (teacherId <= 0) throw new ArgumentException("teacherId must be greater than zero", nameof(teacherId));
            if (examId <= 0) throw new ArgumentException("examId must be greater than zero", nameof(examId));

            // ensure teacher exists (defensive)
            var teacherExists = _context.Teachers.AsNoTracking().Any(t => t.TeacherId == teacherId);
            if (!teacherExists)
                return; // silently return; caller may validate beforehand as needed

            // skip if mapping already exists
            var alreadyAssigned = _context.TeachersToExams
                .AsNoTracking()
                .Any(tte => tte.TeacherId == teacherId && tte.ExamId == examId);

            if (alreadyAssigned) return;

            var mapping = new TeachersToExam
            {
                TeacherId = teacherId,
                ExamId = examId,
                Role = null
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
    }

}
