using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _2._Sem_Project_Eksamen_System.Models1;
// Maps this entity to the "Exam" database table

[Table("Exam")]
public partial class Exam
{
    //Maps property to exam ID
    [Key]
    [Column("ExamID")]
    public int ExamId { get; set; }
    //Optional FK to another Exam
    [Column("ReExamID")]
    public int? ReExamId { get; set; }
    // Title or short name of the exam

   [StringLength(250)]
    [Unicode(false)] 
    public string ExamName { get; set; } = null!;
    // FK to class ID
    [Column("ClassID")]
    public int ClassId { get; set; }
    // Optional censor/examiner type
    [StringLength(50)]
    [Unicode(false)]
    public string? CensorType { get; set; }
    // Optional exam format (e.g., oral, written)
    [StringLength(50)]
    [Unicode(false)]
    public string? Format { get; set; }
    // Optional exam Ivigilation is there or not
    public bool ExamPatrol { get; set; }
    //Optional start date of exam
    public DateOnly? ExamStartDate { get; set; }
    // Optional end date
    public DateOnly? ExamEndDate { get; set; }
    // Dilvery Dare
    public DateOnly? DeliveryDate { get; set; }
    // Indicates this exam is a re-exam
    public bool IsReExam { get; set; }
    // Indicates this exam is a final exam
    [BindNever]

    public bool IsFinalExam { get; set; }
    // Long description or instructions
   [Column(TypeName = "text")]
    public string? Description { get; set; }

    [Column("NumOfStud")]
    // Nulable Number
    public int? NumOfStud { get; set; }
    // Required class association, linked via ClassId foreign key
    [ForeignKey("ClassId")]
    [InverseProperty("Exams")]
    [BindNever]
    public virtual Class Class { get; set; } = null!;
    // Collection of re-exams that reference this exam as parent
    [InverseProperty("ReExam")]
    public virtual ICollection<Exam> InverseReExam { get; set; } = new List<Exam>();
    // Optional parent re-exam reference for exam retakes
    [ForeignKey("ReExamId")]
    [InverseProperty("InverseReExam")]
    public virtual Exam? ReExam { get; set; }
    // Junction table for exam room assignments
    [InverseProperty("Exam")]
    public virtual ICollection<RoomsToExam> RoomsToExams { get; set; } = new List<RoomsToExam>();
    // Junction table for student exam registrations
    [InverseProperty("Exam")]
    public virtual ICollection<StudentsToExam> StudentsToExams { get; set; } = new List<StudentsToExam>();
    // Junction table for teacher exam proctoring assignments
    [InverseProperty("Exam")]
    public virtual ICollection<TeachersToExam> TeachersToExams { get; set; } = new List<TeachersToExam>();
}
