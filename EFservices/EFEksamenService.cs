using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
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
            return context.Find<Exam>(id);
        }

        public void DeleteItem(int id)
        {
            var examToDelete = GetItemById(id);
            if (examToDelete != null)
            {
                context.Remove(examToDelete);
                context.SaveChanges();
            }
        }

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
