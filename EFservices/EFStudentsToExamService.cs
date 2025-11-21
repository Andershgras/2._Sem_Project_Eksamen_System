using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFStudentsToExamService : IStudentsToExams
    {
        //DbContext Injection
        private readonly EksamensDBContext _context;
        public EFStudentsToExamService(EksamensDBContext context)
        {
            _context = context;
        }

        // Return all student-exam mappings with related Student and Exam loaded (read-only)
        public async Task<IEnumerable<StudentsToExam>> GetAllAsync()
            => await _context.StudentsToExams
                .Include(se => se.Student)
                .Include(se => se.Exam)
                .AsNoTracking()
                .OrderBy(se => se.ExamId)
                .ToListAsync();
        // Return mappings filtered by student name substring
        public async Task<IEnumerable<StudentsToExam>> GetAllAsync(GenericFilter filter)
        {
            var term = (filter?.FilterByName ?? string.Empty).ToLower();
            return await _context.StudentsToExams
                .Include(se => se.Student)
                .Include(se => se.Exam)
                .Where(se => se.Student != null && se.Student.StudentName.ToLower().Contains(term))
                .AsNoTracking()
                .OrderBy(se => se.ExamId)
                .ToListAsync();
        }
            
        // Add asingle student to exam mapping
        public async Task AddItemAsync(StudentsToExam item)
        {
            if (item == null) return;
            await _context.StudentsToExams.AddAsync(item);
            await _context.SaveChangesAsync();
        }
        //Find a mapping by primary key
        public async Task<StudentsToExam?> GetItemByIdAsync(int id)
            => await _context.StudentsToExams.FindAsync(id);
        // Delete a mapping if present
        public async Task DeleteItemAsync(int id)
        {
            var entity = await GetItemByIdAsync(id);
            if (entity != null)
            {
                _context.StudentsToExams.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        // Update scalar fields of an existing mapping
        public async Task UpdateItemAsync(StudentsToExam item)
        {
            if (item == null) return;
            var existing = await _context.StudentsToExams.FindAsync(item.StudentExamId);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
            }
        }

        // Adds all students from a class to an exam
        public async Task AddStudentsFromClassToExamAsync(int classId, int examId)
        {
            var studentIds = await _context.StudentsToClasses
                .Where(stc => stc.ClassId == classId)
                .Select(stc => stc.StudentId)
                .ToListAsync();

            // add only students not already added to the exam
            var alreadyAddedStudentIds = await _context.StudentsToExams
                .Where(se => se.ExamId == examId)
                .Select(se => se.StudentId)
                .ToHashSetAsync();

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
            await _context.SaveChangesAsync();
        }

        // Remove all students from exam (if class changes or exam deleted)
        public async Task RemoveAllFromExamAsync(int examId)
        {
            var items = await _context.StudentsToExams.Where(se => se.ExamId == examId).ToListAsync();
            if (items.Any())
            {
                _context.StudentsToExams.RemoveRange(items);
                await _context.SaveChangesAsync();
            }
        }

        // Updates students for exam when class changes
        public async Task SyncStudentsForExamAndClassAsync(int examId, int newClassId)
        {
            await RemoveAllFromExamAsync(examId);
            await AddStudentsFromClassToExamAsync(newClassId, examId);
        }
        // Add multiple student ids to an exam (calls AddItemAsync for each)
        public async Task AddStudentsToExamAsync(IEnumerable<int> studIds, int examId)
        {
            foreach (var studentId in studIds)
            {
                await AddItemAsync(new StudentsToExam {
                    StudentId = studentId,
                    ExamId = examId
                });
            }
        }
    }
}
