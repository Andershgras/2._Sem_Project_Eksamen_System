using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFHoldService : ICRUD<Class>
    {
        private readonly EksamensDBContext _context;

        EFHoldService(EksamensDBContext context)
        {
            _context = context;
        }

        public IEnumerable<Class> GetAll()
        {
            var items = _context.Classes
                .Include(c => c.StudentsToClasses)
                    .ThenInclude(sc => sc.Student)
                .AsNoTracking()
                .OrderBy(c => c.ClassId)
                .ToList();

            return items;
        }

        public IEnumerable<Class> GetAll(GenericFilter Filter)
        {
            var items = _context.Classes
                .Where(c => c.ClassName.ToLower().StartsWith(Filter.FilterByName))
                .AsNoTracking()
                .ToList();
           return items;
        }
      
        public void AddItem(Class item)
        {
            _context.Add(item);
            _context.SaveChanges();
        }

     
        public Class? GetItemById(int id)
        {
            return _context.Classes.Find(id);
        }
       
        public void DeleteItem(int id)
        {
            var item = GetItemById(id);
            if (item != null)_context.Classes.Remove(item);
            _context.SaveChanges();
        }
     
        public void UpdateItem(Class item)
        {
            _context.Update(item);
            _context.SaveChanges();
        }
    }
}
