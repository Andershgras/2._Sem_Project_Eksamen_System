namespace _2._Sem_Project_Eksamen_System.Models1
{
    /// <summary>
    /// Represents the result of a scheduling overlap check, indicating whether a conflict exists and providing details
    /// about the conflicting entity, if applicable.
    /// </summary>
    /// <remarks>This class is used to encapsulate the outcome of a scheduling conflict check. If a conflict
    /// is detected, the relevant details such as the type of the conflicting entity, its identifier, and additional
    /// context (e.g., exam name, time range, teacher, or room) are provided.</remarks>
    public class OverlapResult
    {
        // Flag indicating if a scheduling conflict exists
        public bool HasConflict { get; set; } = false;
        // Print Descriptive mesage
        public string? Message { get; set; }

        // fx. "Exam", "Class", "Room", etc.
        public string? ConflictingEntityType { get; set; }
        // Id Of the conflictiong entity 
        public int? ConflictingEntityId { get; set; }
        //Name of the conflictiong exam
        public string? ConflictingExamName { get; set; }
        //  start Date of the conflictiong exam
        public DateOnly? ConflictingExamStart { get; set; }
        //  End Date of the conflictiong exam
        public DateOnly? ConflictingExamEnd { get; set; }
        // Id of the teacher involved on that conflict or overlap
        public int? ConflictingTeacherId { get; set; }
        //Id of the room involved in conflict
        public int? ConflictingRoomId { get; set; }

        // Convenience factory for no-conflict
        public static OverlapResult Ok() => new OverlapResult { HasConflict = false };

        // Convenience factory for conflict
        public static OverlapResult Conflict(string message) => new OverlapResult { HasConflict = true, Message = message };
    }
}
