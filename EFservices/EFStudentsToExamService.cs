using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFStudentsToExamService : IStudentsToExams
    {
        private readonly EksamensDBContext _context;
        public EFStudentsToExamService(EksamensDBContext context)
        {
            _context = context;
        }

        
        public IEnumerable<StudentsToExam> GetAll()
            => _context.StudentsToExams
                .Include(se => se.Student)
                .Include(se => se.Exam)
                .AsNoTracking()
                .OrderBy(se => se.ExamId)
                .ToList();

        public IEnumerable<StudentsToExam> GetAll(GenericFilter filter)
            => GetAll(); 

        public void AddItem(StudentsToExam item)
        {
            if (item == null) return;
            _context.StudentsToExams.Add(item);
            _context.SaveChanges();
        }

        public StudentsToExam? GetItemById(int id)
            => _context.StudentsToExams.Find(id);

        public void DeleteItem(int id)
        {
            var entity = GetItemById(id);
            if (entity != null)
            {
                _context.StudentsToExams.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void UpdateItem(StudentsToExam item)
        {
            if (item == null) return;
            var existing = _context.StudentsToExams.Find(item.StudentExamId);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(item);
                _context.SaveChanges();
            }
        }



        // Adds all students from a class to an exam
        public void AddStudentsFromClassToExam(int classId, int examId)
        {
            var studentIds = _context.StudentsToClasses
                .Where(stc => stc.ClassId == classId)
                .Select(stc => stc.StudentId)
                .ToList();

            // add only students not already added to the exam
            var alreadyAddedStudentIds = _context.StudentsToExams
                .Where(se => se.ExamId == examId)
                .Select(se => se.StudentId)
                .ToHashSet();

            foreach (var studentId in studentIds)
            {
                if (!alreadyAddedStudentIds.Contains(studentId))
                {
                    var entry = new StudentsToExam
                    {
                        ExamId = examId,
                        StudentId = studentId
                    };
                    _context.StudentsToExams.Add(entry);
                }
            }
            _context.SaveChanges();
        }

        // Remove all students from exam (if class changes or exam deleted)
        public void RemoveAllFromExam(int examId)
        {
            var items = _context.StudentsToExams.Where(se => se.ExamId == examId).ToList();
            if (items.Any())
            {
                _context.StudentsToExams.RemoveRange(items);
                _context.SaveChanges();
            }
        }

        // Updates students for exam when class changes
        public void SyncStudentsForExamAndClass(int examId, int newClassId)
        {
            RemoveAllFromExam(examId);
            AddStudentsFromClassToExam(newClassId, examId);
        }

        public void AddStudentsToExam(IEnumerable<int> studIds, int examId)
        {
            foreach (var studentId in studIds)
            {
                AddItem(new StudentsToExam
                {
                    StudentId = studentId,
                    ExamId = examId
                });
            }
        }
    }
}
