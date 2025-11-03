using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFRoomService : ICRUD<Room>
    {
        private readonly EksamensDBContext _context;

        public EFRoomService(EksamensDBContext context) => _context = context;

        public IEnumerable<Room> GetAll() => _context.Rooms.AsNoTracking().OrderBy(r => r.RoomId);

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

            _context.SaveChanges();
        }

        public void DeleteItem(int id)
        {
            var room = _context.Rooms.Find(id);
            if (room == null) return;
            _context.Rooms.Remove(room);
            _context.SaveChanges();
        }
    }
}
