using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

[Table("Student")]
public partial class Student
{
    // Primary key identifier for the student
    [Key]
    [Column("StudentID")]
    public int StudentId { get; set; }
    // Required student name, max 250 characters, non-Unicode
    [StringLength(250)]
    [Unicode(false)]
    public string StudentName { get; set; } = null!;
    // Optional email address, max 250 characters, non-Unicode
    [StringLength(250)]
    [Unicode(false)]
    public string? Email { get; set; }
    // Navigation property for class enrollments
    [InverseProperty("Student")]
    public virtual ICollection<StudentsToClass> StudentsToClasses { get; set; } = new List<StudentsToClass>();
    // Navigation property for exam registration
    [InverseProperty("Student")]
    public virtual ICollection<StudentsToExam> StudentsToExams { get; set; } = new List<StudentsToExam>();
}
