using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

[Table("Exam")]
public partial class Exam
{
    [Key]
    [Column("ExamID")]
    public int ExamId { get; set; }

    [Column("ReExamID")]
    public int? ReExamId { get; set; }

    [StringLength(250)]
    [Unicode(false)] 
    public string ExamName { get; set; } = null!;

    [Column("ClassID")]
    public int ClassId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? CensorType { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Format { get; set; }

    public bool ExamPatrol { get; set; }

    public DateOnly? ExamStartDate { get; set; }

    public DateOnly? ExamEndDate { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public bool IsReExam { get; set; }

    public bool IsFinalExam { get; set; }

    [Column(TypeName = "text")]
    public string? Description { get; set; }


    [ForeignKey("ClassId")]
    [InverseProperty("Exams")]
    [BindNever]
    public virtual Class Class { get; set; } = null!;

    [InverseProperty("ReExam")]
    public virtual ICollection<Exam> InverseReExam { get; set; } = new List<Exam>();

    [ForeignKey("ReExamId")]
    [InverseProperty("InverseReExam")]
    public virtual Exam? ReExam { get; set; }

    [InverseProperty("Exam")]
    public virtual ICollection<RoomsToExam> RoomsToExams { get; set; } = new List<RoomsToExam>();

    [InverseProperty("Exam")]
    public virtual ICollection<StudentsToExam> StudentsToExams { get; set; } = new List<StudentsToExam>();

    [InverseProperty("Exam")]
    public virtual ICollection<TeachersToExam> TeachersToExams { get; set; } = new List<TeachersToExam>();
}
