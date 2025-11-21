using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

public partial class StudentsToClass
{
   
    [Key]
    [Column("StudentClassID")]
    public int StudentClassId { get; set; }
    
    [Column("StudentID")]
    public int StudentId { get; set; }
    
    [Column("ClassID")]
    public int ClassId { get; set; }
    
    [ForeignKey("ClassId")]
    [InverseProperty("StudentsToClasses")]
    public virtual Class Class { get; set; } = null!;
    
    [ForeignKey("StudentId")]
    [InverseProperty("StudentsToClasses")]
    public virtual Student Student { get; set; } = null!;
}
