using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

public partial class StudentsToClass
{
    // Primary key identifier for the student-class relationship
    [Key]
    [Column("StudentClassID")]
    public int StudentClassId { get; set; }
    // Foreign key to the associated student
    [Column("StudentID")]
    public int StudentId { get; set; }
    // Foreign key to the associated class
    [Column("ClassID")]
    public int ClassId { get; set; }
    // Navigation property to the class entity
    [ForeignKey("ClassId")]
    [InverseProperty("StudentsToClasses")]
    public virtual Class Class { get; set; } = null!;
    // Navigation property to the student entity
    [ForeignKey("StudentId")]
    [InverseProperty("StudentsToClasses")]
    public virtual Student Student { get; set; } = null!;
}
