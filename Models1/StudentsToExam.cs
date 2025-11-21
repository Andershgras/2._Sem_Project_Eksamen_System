using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

public partial class StudentsToExam
{
    // Primary key identifier for the student-exam relationship
    [Key]
    [Column("StudentExamID")]
    public int StudentExamId { get; set; }
    // Foreign key to the associated student
    [Column("StudentID")]
    public int StudentId { get; set; }
    // Foreign key to the associated exam
    [Column("ExamID")]
    public int ExamId { get; set; }
    // Navigation property to the exam entity

    [ForeignKey("ExamId")]
    [InverseProperty("StudentsToExams")]
    public virtual Exam Exam { get; set; } = null!;
    // Navigation property to the student entity
    [ForeignKey("StudentId")]
    [InverseProperty("StudentsToExams")]
    public virtual Student Student { get; set; } = null!;
}
