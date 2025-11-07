using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFEksamenService: ICRUD<Exam>
    {
        EksamensDBContext context;
        public EFEksamenService(EksamensDBContext dBContext)
        {
            this.context = dBContext;
        }

        public IEnumerable<Exam> GetAll()
        {
            return context.Exams
           .Include(e => e.Class)
           .Include(e => e.StudentsToExams)
                .ThenInclude(se => se.Student)
           .Include(e => e.RoomsToExams)
                .ThenInclude(re => re.Room)
           .Include(e => e.TeachersToExams)
               .ThenInclude(et => et.Teacher)
           .Include(e => e.ReExam)
           .AsNoTracking()
           .OrderBy(e => e.ExamId)
           .ToList();
        }

        public IEnumerable<Exam> GetAll(GenericFilter Filter)
        {
            return context.Exams.Where(s => s.ExamName.ToLower().StartsWith(Filter.FilterByName)).AsNoTracking().ToList();
        }

        public void AddItem(Exam item)
        {
            context.Add(item);
            context.SaveChanges();
        }

        public Exam? GetItemById(int id)
        {
            return context.Exams
            .Include(e => e.Class)
            .Include(e => e.ReExam)
            .Include(e => e.TeachersToExams)
                .ThenInclude(te => te.Teacher)
            .Include(e => e.RoomsToExams)
                .ThenInclude(rt => rt.Room)
            .Include(e => e.StudentsToExams)
          .ThenInclude(st => st.Student)
          .FirstOrDefault(e => e.ExamId == id);
        }

        public void DeleteItem(int id)//TEST
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                // Find the exam with ALL relationships including inverse re-exams
                var examToDelete = context.Exams
                    .Include(e => e.StudentsToExams)
                    .Include(e => e.TeachersToExams)
                    .Include(e => e.RoomsToExams)
                    .Include(e => e.InverseReExam) // Important: exams that have this as their ReExam
                    .FirstOrDefault(e => e.ExamId == id);

                if (examToDelete != null)
                {
                    // 1. Remove StudentsToExams relationships
                    if (examToDelete.StudentsToExams.Any())
                    {
                        context.StudentsToExams.RemoveRange(examToDelete.StudentsToExams);
                    }

                    // 2. Remove TeachersToExams relationships
                    if (examToDelete.TeachersToExams.Any())
                    {
                        context.TeachersToExams.RemoveRange(examToDelete.TeachersToExams);
                    }

                    // 3. Remove RoomsToExams relationships
                    if (examToDelete.RoomsToExams.Any())
                    {
                        context.RoomsToExams.RemoveRange(examToDelete.RoomsToExams);
                    }

                    // 4. Handle inverse re-exams (exams that point to this exam as their ReExam)
                    if (examToDelete.InverseReExam.Any())
                    {
                        foreach (var inverseExam in examToDelete.InverseReExam.ToList())
                        {
                            inverseExam.ReExamId = null; // Remove the reference
                        }
                    }

                    // 5. If this exam has a ReExam, we need to decide what to do with it
                    // Option A: Also delete the ReExam (cascade delete)
                    // Option B: Keep the ReExam but remove the relationship
                    if (examToDelete.ReExamId.HasValue)
                    {
                        // For now, let's just remove the relationship
                        examToDelete.ReExamId = null;
                    }

                    // Save all relationship changes first
                    context.SaveChanges();

                    // 6. Finally delete the exam itself
                    context.Exams.Remove(examToDelete);
                    context.SaveChanges();

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Failed to delete exam: {ex.Message}", ex);
            }
        }
        //EndTest
        public void UpdateItem(Exam item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var existingExam = context.Exams.Find(item.ExamId);
            if (existingExam != null)
            {
                context.Entry(existingExam).CurrentValues.SetValues(item);
                context.SaveChanges();
            }
           
        }


    }
}
