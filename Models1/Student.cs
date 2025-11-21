using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

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
    public string? Email { get; set; }
  
    [InverseProperty("Student")]
    public virtual ICollection<StudentsToClass> StudentsToClasses { get; set; } = new List<StudentsToClass>();
    
    [InverseProperty("Student")]
    public virtual ICollection<StudentsToExam> StudentsToExams { get; set; } = new List<StudentsToExam>();
}
