using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

public partial class RoomsToExam
{
    [Key]
    [Column("RoomExamID")]
    public int RoomExamId { get; set; }

    [Column("RoomID")]
    public int RoomId { get; set; }

    [Column("ExamID")]
    public int ExamId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Role { get; set; }

    [ForeignKey("ExamId")]
    [InverseProperty("RoomsToExams")]
    public virtual Exam Exam { get; set; } = null!;

    [ForeignKey("RoomId")]
    [InverseProperty("RoomsToExams")]
    public virtual Room Room { get; set; } = null!;
}
