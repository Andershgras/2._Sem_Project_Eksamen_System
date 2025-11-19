using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.EntityFrameworkCore;


namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFRoomsToExamService : IRoomsToExams
    {
        private readonly EksamensDBContext _context;

        public EFRoomsToExamService(EksamensDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomsToExam>> GetAllAsync()
        {
            return await _context.RoomsToExams
                .Include(rte => rte.Room)
                .Include(rte => rte.Exam)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<RoomsToExam>> GetAllAsync(GenericFilter filter)
        {
            var term = (filter?.FilterByName ?? string.Empty).ToLower();
            return await _context.RoomsToExams
                .Include(rte => rte.Room)
                .Include(rte => rte.Exam)
                .Where(rte => rte.Role != null && rte.Role.ToLower().Contains(term))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<RoomsToExam?> GetItemByIdAsync(int id)
        {
            return await _context.RoomsToExams
                .Include(rte => rte.Room)
                .Include(rte => rte.Exam)
                .FirstOrDefaultAsync(rte => rte.RoomExamId == id);
        }

        public async Task AddItemAsync(RoomsToExam item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            await _context.RoomsToExams.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(RoomsToExam item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var existing = await _context.RoomsToExams.FindAsync(item.RoomExamId);
            if (existing == null) return;

            existing.RoomId = item.RoomId;
            existing.ExamId = item.ExamId;
            existing.Role = item.Role;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int id)
        {
            var entity = await _context.RoomsToExams.FindAsync(id);
            if (entity == null) return;
            _context.RoomsToExams.Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Add multiple room assignments to an exam in one call.
        /// Existing assignments are not removed by this method.
        /// </summary>
        public async Task AddRoomsToExamAsync(int examId, IEnumerable<RoomsToExam> assignments)
        {
            if (assignments == null) return;

            var list = assignments
                .Where(a => a != null)
                .Select(a =>
                {
                    a.ExamId = examId;
                    return a;
                })
                .ToList();

            if (!list.Any()) return;

            await _context.RoomsToExams.AddRangeAsync(list);
            await _context.SaveChangesAsync();
        }

        // Check availability: returns true if room is available for the entire requested range
        public async Task<bool> IsRoomAvailableAsync(int roomId, DateOnly requestedStart, DateOnly requestedEnd, int? excludeExamId = null)
        {
            if (requestedEnd < requestedStart)
                throw new ArgumentException("End date must be on or after start date.", nameof(requestedEnd));

            // Find any RoomsToExam that reference the same room and have an Exam with overlapping dates
            var query = _context.RoomsToExams
                .Include(rte => rte.Exam)
                .Where(rte => rte.RoomId == roomId);

            if (excludeExamId.HasValue)
                query = query.Where(rte => rte.ExamId != excludeExamId.Value);

            // Overlap check: existingStart <= requestedEnd && existingEnd >= requestedStart
            var overlaps = await query
                .Where(rte =>
                    rte.Exam.ExamStartDate.HasValue &&
                    rte.Exam.ExamEndDate.HasValue &&
                    rte.Exam.ExamStartDate.Value <= requestedEnd &&
                    rte.Exam.ExamEndDate.Value >= requestedStart)
                .AsNoTracking()
                .AnyAsync();

            return !overlaps;
        }
        public async Task RemoveAllRoomsFromExamAsync(int examId)//Added This method to remove all room assignments from a specific exam
        {
            var existingAssignments = await _context.RoomsToExams
                .Where(rte => rte.ExamId == examId)
                .ToListAsync();

            if (existingAssignments.Any())
            {
                _context.RoomsToExams.RemoveRange(existingAssignments);
                await _context.SaveChangesAsync();
            }
        }
    }
}