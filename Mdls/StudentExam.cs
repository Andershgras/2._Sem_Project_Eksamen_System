using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("StudentExam")]
[Index("StudentId", "ExamId", Name = "UQ_StudentExam", IsUnique = true)]
public partial class StudentExam
{
    [Key]
    [Column("StudentExamID")]
    public int StudentExamId { get; set; }

    [Column("StudentID")]
    public int StudentId { get; set; }

    [Column("ExamID")]
    public int ExamId { get; set; }

    [ForeignKey("ExamId")]
    [InverseProperty("StudentExams")]
    public virtual Exam Exam { get; set; } = null!;

    [ForeignKey("StudentId")]
    [InverseProperty("StudentExams")]
    public virtual Student Student { get; set; } = null!;
}
