using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace _2._Sem_Project_Eksamen_System.EFservices
{
    

    public class EFExamService: ICRUDAsync<Exam>
    {
        //DbContext injected
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
        // Return exam filtered by name 
        public async Task<IEnumerable<Exam>> GetAllAsync(GenericFilter Filter)
        {
            return await context.Exams
                .Where(s => s.ExamName.ToLower().StartsWith(Filter.FilterByName))
                .AsNoTracking()
                .ToListAsync();
        }
        //Add a new exam and persisit
        public async Task AddItemAsync(Exam item)
        {
            await context.AddAsync(item);
            await context.SaveChangesAsync();
        }
        // Get a single exam with all related elements like teachers, rooms, students and re-exam
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
        //Delete an exam and also clean up all relationship entries within a transaction
        public async Task DeleteItemAsync(int id)
        {   // Start a transaction to ensure all related deletions occur properly
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // Find and load the exam with ALL relationships including inverse re-exams
                var examToDelete = await context.Exams
                    .Include(e => e.StudentsToExams)
                    .Include(e => e.TeachersToExams)
                    .Include(e => e.RoomsToExams)
                    .Include(e => e.InverseReExam) // Important: exams that have this as their ReExam
                    .FirstOrDefaultAsync(e => e.ExamId == id);

                if (examToDelete != null)
                {
                    // Remove StudentsToExams relationships
                    if (examToDelete.StudentsToExams.Any())
                    {
                        context.StudentsToExams.RemoveRange(examToDelete.StudentsToExams);
                    }

                    // Remove TeachersToExams relationships
                    if (examToDelete.TeachersToExams.Any())
                    {
                        context.TeachersToExams.RemoveRange(examToDelete.TeachersToExams);
                    }

                    // Remove RoomsToExams relationships
                    if (examToDelete.RoomsToExams.Any())
                    {
                        context.RoomsToExams.RemoveRange(examToDelete.RoomsToExams);
                    }

                    // Handle inverse re-exams (exams that point to this exam as their ReExam)
                    if (examToDelete.InverseReExam.Any())
                    {
                        foreach (var inverseExam in examToDelete.InverseReExam.ToList())
                        {
                            inverseExam.ReExamId = null; // Remove the reference
                        }
                    }

                    
                    if (examToDelete.ReExamId.HasValue)
                    {
                        // Removes the relationship between this exam and its ReExam
                        examToDelete.ReExamId = null;
                    }

                    // Save all relationship changes first
                    await context.SaveChangesAsync();

                    // Finally delete the exam itself
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
        //Update an exsisting exam apply newly updated values and save them
        public async Task UpdateItemAsync(Exam item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var existingExam = await context.Exams.FindAsync(item.ExamId);
            if (existingExam != null)
            {
                //Replace scaler properties with values from the provided item 
                // and Navigation properties remains intact
                context.Entry(existingExam).CurrentValues.SetValues(item);
                await context.SaveChangesAsync();
            }
        }
    }
}
