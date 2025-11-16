using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    public interface ICheckOverlap
    {
      /// <summary>
      /// Determines whether a teacher has any overlapping exams within the specified date range and conditions.
      /// </summary>
      /// <param name="teacherId">The unique identifier of the teacher to check for overlaps.</param>
      /// <param name="newStart">The start date of the new exam. Can be <see langword="null"/> if no specific start date is provided.</param>
      /// <param name="newEnd">The end date of the new exam. Can be <see langword="null"/> if no specific end date is provided.</param>
      /// <param name="newIsFinal">A value indicating whether the new exam is a final exam. <see langword="true"/> if it is a final exam;
      /// otherwise, <see langword="false"/>.</param>
      /// <param name="newIsReExam">A value indicating whether the new exam is a re-exam. <see langword="true"/> if it is a re-exam; otherwise,
      /// <see langword="false"/>.</param>
      /// <param name="excludeExamId">The unique identifier of an exam to exclude from the overlap check. Can be <see langword="null"/> if no exam
      /// should be excluded.</param>
      /// <returns>An <see cref="OverlapResult"/> indicating whether an overlap exists and providing details about the overlap,
      /// if any.</returns>
        OverlapResult TeacherHasOverlap(int teacherId, DateOnly? newStart, DateOnly? newEnd, bool newIsFinal, bool newIsReExam, int? excludeExamId = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="newStart"></param>
        /// <param name="newEndl"></param>
        /// <returns></returns>
        OverlapResult ClassHasOverlap(int classId, DateOnly? newStart, DateOnly? newEnd, int? excludeExamId = null);
        /// <summary>
        /// Determines whether the specified room has a scheduling overlap with the given date range.
        /// </summary>
        /// <remarks>If both <paramref name="newStart"/> and <paramref name="newEnd"/> are <see
        /// langword="null"/>, the method will return <see langword="true"/> if the room has any existing
        /// bookings.</remarks>
        /// <param name="roomId">The unique identifier of the room to check for overlaps.</param>
        /// <param name="newStart">The start date of the new booking range. Can be <see langword="null"/> to indicate no start date constraint.</param>
        /// <param name="newEnd">The end date of the new booking range. Can be <see langword="null"/> to indicate no end date constraint.</param>
        /// <returns><see langword="true"/> if the room has an existing booking that overlaps with the specified date range;
        /// otherwise, <see langword="false"/>.</returns>
        OverlapResult RoomHasOverlap(int roomId, DateOnly? newStart, DateOnly? newEnd, int? excludeExamId = null);
    }
}
