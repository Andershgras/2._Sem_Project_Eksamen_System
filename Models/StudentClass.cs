using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("StudentClass")]
[Index("ClassId", "StudentId", Name = "UQ_StudentClass", IsUnique = true)]
public partial class StudentClass
{
    [Key]
    [Column("StudentClassID")]
    public int StudentClassId { get; set; }

    [Column("ClassID")]
    public int ClassId { get; set; }

    [Column("StudentID")]
    public int StudentId { get; set; }

    [ForeignKey("ClassId")]
    [InverseProperty("StudentClasses")]
    public virtual Filter Class { get; set; } = null!;

    [ForeignKey("StudentId")]
    [InverseProperty("StudentClasses")]
    public virtual Student Student { get; set; } = null!;
}
