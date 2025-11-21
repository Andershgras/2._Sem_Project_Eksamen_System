using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

[Table("Room")]
public partial class Room
{
    // Primary key identifier for the room
    [Key]
    [Column("RoomID")]
    public int RoomId { get; set; }
    // Required room name, max 250 characters, non-Unicode
    [Required]
    [StringLength(250)]
    [Unicode(false)]
    public string Name { get; set; } = null!;
    // Optional room capacity with validation range (0-10,000)
    [Range(0, 10000, ErrorMessage = "Capacity must be between 0 & 10000")]
    public int? Capacity { get; set; }
    // Navigation property for exams scheduled in this room
    [InverseProperty("Room")]
    public virtual ICollection<RoomsToExam> RoomsToExams { get; set; } = new List<RoomsToExam>();
}
