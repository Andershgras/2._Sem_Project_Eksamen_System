using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFExamService: ICRUDAsync<Exam>
    {
        EksamensDBContext context;
        public EFExamService(EksamensDBContext dBContext)
        {
            this.context = dBContext;
        }

        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await context.Exams
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
                .ToListAsync();
        }
        public async Task<IEnumerable<Exam>> GetAllAsync(GenericFilter Filter)
        {
            return await context.Exams
                .Where(s => s.ExamName.ToLower().StartsWith(Filter.FilterByName))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddItemAsync(Exam item)
        {
            await context.AddAsync(item);
            await context.SaveChangesAsync();
        }

        public async Task<Exam?> GetItemByIdAsync(int id)
        {
            return await context.Exams
                .Include(e => e.Class)
                .Include(e => e.ReExam)
                .Include(e => e.TeachersToExams)
                    .ThenInclude(te => te.Teacher)
                .Include(e => e.RoomsToExams)
                    .ThenInclude(rt => rt.Room)
                .Include(e => e.StudentsToExams)
                    .ThenInclude(st => st.Student)
                .FirstOrDefaultAsync(e => e.ExamId == id);
        }

        public async Task DeleteItemAsync(int id)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // Find the exam with ALL relationships including inverse re-exams
                var examToDelete = await context.Exams
                    .Include(e => e.StudentsToExams)
                    .Include(e => e.TeachersToExams)
                    .Include(e => e.RoomsToExams)
                    .Include(e => e.InverseReExam) // Important: exams that have this as their ReExam
                    .FirstOrDefaultAsync(e => e.ExamId == id);

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
                    await context.SaveChangesAsync();

                    // 6. Finally delete the exam itself
                    context.Exams.Remove(examToDelete);
                    await context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Failed to delete exam: {ex.Message}", ex);
            }
        }
        public async Task UpdateItemAsync(Exam item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var existingExam = await context.Exams.FindAsync(item.ExamId);
            if (existingExam != null)
            {
                context.Entry(existingExam).CurrentValues.SetValues(item);
                await context.SaveChangesAsync();
            }
        }
    }
}
