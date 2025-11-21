using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

public partial class TeachersToExam
{
    // Primary key identifier for the teacher-exam relationship
    [Key]
    [Column("TeacherExamID")]
    public int TeacherExamId { get; set; }
    // Foreign key to the associated teacher
    [Column("TeacherID")]
    public int TeacherId { get; set; }
    // Foreign key to the associated exam
    [Column("ExamID")]
    public int ExamId { get; set; }
    // Optional role description for the teacher in this exam (e.g., "proctor", "supervisor")
    [StringLength(100)]
    [Unicode(false)]
    public string? Role { get; set; }
    // Navigation property tot the exam
    [ForeignKey("ExamId")]
    [InverseProperty("TeachersToExams")]
    public virtual Exam Exam { get; set; } = null!;
    //navigation property to the teacher entity
    [ForeignKey("TeacherId")]
    [InverseProperty("TeachersToExams")]
    public virtual Teacher Teacher { get; set; } = null!;
}
