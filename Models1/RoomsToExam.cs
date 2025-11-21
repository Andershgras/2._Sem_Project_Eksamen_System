using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

public partial class RoomsToExam
{
    // Primary key identifier for the room-exam relationship
    [Key]
    [Column("RoomExamID")]
    public int RoomExamId { get; set; }
    // Foreign key to the associated room
    [Column("RoomID")]
    public int RoomId { get; set; }
    // Foreign key to the associated room
    [Column("ExamID")]
    public int ExamId { get; set; }
    // Optional role description for the room in this exam (e.g., "main", "backup")
    [StringLength(100)]
    [Unicode(false)]
    public string? Role { get; set; }
    // Navigation property to the exam entity
    [ForeignKey("ExamId")]
    [InverseProperty("RoomsToExams")]
    public virtual Exam Exam { get; set; } = null!;
    // Navigation property to the room entity
    [ForeignKey("RoomId")]
    [InverseProperty("RoomsToExams")]
    public virtual Room Room { get; set; } = null!;
}
