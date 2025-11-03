using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("Student")]
public partial class Student
{
    [Key]
    [Column("StudentID")]
    public int StudentId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string StudentName { get; set; } = null!;

    [StringLength(250)]
    [Unicode(false)]
    public string? Mail { get; set; }

    [InverseProperty("Student")]
    public virtual ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();

    [InverseProperty("Student")]
    public virtual ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
}
