using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using _2._Sem_Project_Eksamen_System.Utils;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFTeachersToExamService : ICRUD<TeachersToExam>
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
    }
}