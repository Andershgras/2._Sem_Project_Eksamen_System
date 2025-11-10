using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFRoomService : ICRUDAsync<Room>
    {
        private readonly EksamensDBContext _context;

        public EFRoomService(EksamensDBContext context) => _context = context;

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms.AsNoTracking().OrderBy(r => r.RoomId).ToListAsync();
        }

        public async Task<Room?> GetItemByIdAsync(int id)
        {
            return await _context.Rooms.FindAsync(id);
        }
        public async Task AddItemAsync(Room item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _context.Rooms.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(Room item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var existing = await _context.Rooms.FindAsync(item.RoomId);
            if (existing == null) return;

            existing.Name = item.Name;
            existing.Capacity = item.Capacity;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return;
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Room>> GetAllAsync(GenericFilter filter)
        {
            var term = (filter?.FilterByName ?? string.Empty).Trim().ToLower();

            var query = _context.Rooms.AsNoTracking();

            if (!string.IsNullOrEmpty(term))
                query = query.Where(r => r.Name.ToLower().Contains(term));

            return await query.OrderBy(r => r.RoomId).ToListAsync();
        }
    }
}
