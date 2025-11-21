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
     
        public bool HasConflict { get; set; } = false;
   
        public string? Message { get; set; }

    
        public string? ConflictingEntityType { get; set; }
     
        public int? ConflictingEntityId { get; set; }
     
        public string? ConflictingExamName { get; set; }
     
        public DateOnly? ConflictingExamStart { get; set; }
      
        public DateOnly? ConflictingExamEnd { get; set; }
      
        public int? ConflictingTeacherId { get; set; }
     
        public int? ConflictingRoomId { get; set; }

       
        public static OverlapResult Ok() => new OverlapResult { HasConflict = false };

    
        public static OverlapResult Conflict(string message) => new OverlapResult { HasConflict = true, Message = message };
    }
}
