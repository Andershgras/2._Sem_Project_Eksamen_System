using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    public interface IRoomsToExams : ICRUDAsync<RoomsToExam>
    {
        /// <summary>
        /// Add multiple room assignments to an exam.
        /// Each provided RoomsToExam should have RoomId, Role set. ExamId may be set by the caller or inside the implementation.
        /// </summary>
        /// <param name="examId">Target exam id</param>
        /// <param name="assignments">Collection of RoomsToExam assignments (RoomId + Role)</param>
        Task AddRoomsToExamAsync(int examId, IEnumerable<RoomsToExam> assignments);

        /// <summary>
        /// Determines whether a room is available for booking within the specified date range.
        /// </summary>
        /// <remarks>This method checks for any existing bookings that overlap with the specified date
        /// range.  If <paramref name="excludeExamId"/> is provided, the method will ignore any booking associated with
        /// that exam.</remarks>
        /// <param name="roomId">The unique identifier of the room to check for availability.</param>
        /// <param name="requestedStart">The start date of the requested booking period.</param>
        /// <param name="requestedEnd">The end date of the requested booking period. Must be greater than or equal to <paramref
        /// name="requestedStart"/>.</param>
        /// <param name="excludeExamId">An optional exam identifier to exclude from the availability check.  Use this parameter when checking
        /// availability for an existing booking to avoid conflicts with itself.</param>
        /// <returns><see langword="true"/> if the room is available for the specified date range; otherwise, <see
        /// langword="false"/>.</returns>
        Task<bool> IsRoomAvailableAsync(int roomId, DateOnly requestedStart, DateOnly requestedEnd, int? excludeExamId = null);

        /// <summary>
        /// Remove all room assignments from a specific exam
        /// </summary>
        /// <param name="examId">Target exam id</param>
        Task RemoveAllRoomsFromExamAsync(int examId);
    }    
}
