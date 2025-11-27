using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    //EFCore service implimeting Async CRUD for Room entities
    public class EFRoomService : ICRUDAsync<Room>
    {
        private readonly EksamensDBContext _context; //Db injection

        public EFRoomService(EksamensDBContext context) => _context = context;

        /// <summary>
        /// Returns all rooms, read-only and ordered by RoomId
        /// </summary>
        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms
                .AsNoTracking()
                .OrderBy(r => r.RoomId)
                .ToListAsync();
        }
        //Find a single room by primary key
        public async Task<Room?> GetItemByIdAsync(int id)
        {
            return await _context.Rooms.FindAsync(id);
        }
        //Add a new room and persisit
        public async Task AddItemAsync(Room item)
        {
            ArgumentNullException.ThrowIfNull(item);
            _context.Rooms.Add(item);
            await _context.SaveChangesAsync();
        }
        // Update simple scalar properties of an existing room (Name, Capacity)
        // Uses FindAsync to load tracked entity and updates only those fields
        public async Task UpdateItemAsync(Room item)
        {
            ArgumentNullException.ThrowIfNull(item);
            var existing = await _context.Rooms.FindAsync(item.RoomId);
            if (existing == null) return;

            existing.Name = item.Name;
            existing.Capacity = item.Capacity;

            await _context.SaveChangesAsync();
        }
        // Remove a room by Id if it exsist
        public async Task DeleteItemAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return;
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
        //Return rooms filtered by name
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
