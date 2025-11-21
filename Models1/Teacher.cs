using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

[Table("Teacher")]
public partial class Teacher
{
    // Primary key identifier for the teacher
    [Key]
    [Column("TeacherID")]
    public int TeacherId { get; set; }
    // Required teacher name, max 250 characters, non-Unicode
    [StringLength(250)]
    [Unicode(false)]
    public string TeacherName { get; set; } = null!;
    // Optional email address, max 250 characters, non-Unicode
    [StringLength(250)]
    [Unicode(false)]
    public string? Email { get; set; }
    // Navigation property for the associated exams
    [InverseProperty("Teacher")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
    [InverseProperty("Teacher")]
    public virtual ICollection<TeachersToExam> TeachersToExams { get; set; } = new List<TeachersToExam>();
}
