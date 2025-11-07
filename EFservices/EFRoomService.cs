using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFRoomService : ICRUD<Room>
    {
        private readonly EksamensDBContext _context;

        public EFRoomService(EksamensDBContext context) => _context = context;

        public IEnumerable<Room> GetAll() =>
            _context.Rooms.AsNoTracking().OrderBy(r => r.RoomId);

        public IEnumerable<Room> GetAll(GenericFilter filter)
        {
            var name = (filter?.FilterByName ?? string.Empty).ToLower();
            return _context.Rooms
                .Where(r => r.Name.ToLower().StartsWith(name))
                .AsNoTracking()
                .ToList();
        }

        public Room? GetItemById(int id) => _context.Rooms.Find(id);

        public void AddItem(Room item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _context.Rooms.Add(item);
            _context.SaveChanges();
        }

        public void UpdateItem(Room item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var existing = _context.Rooms.Find(item.RoomId);
            if (existing == null) return;

            existing.Name = item.Name;
            existing.Capacity = item.Capacity;

            _context.SaveChanges();
        }

        public void DeleteItem(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null) return;
            _context.Rooms.Remove(room);
            _context.SaveChanges();
        }
        public IEnumerable<Room> Search(GenericFilter filter)
        {
            var term = (filter?.FilterByName ?? string.Empty).Trim().ToLower();

            var query = _context.Rooms.AsNoTracking();

            if (!string.IsNullOrEmpty(term))
                query = query.Where(r => r.Name.ToLower().Contains(term));

            return query.OrderBy(r => r.RoomId).ToList();
        }
    }
}
