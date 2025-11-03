using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("Class")]
public partial class Filter
{
    [Key]
    [Column("ClassID")]
    public int ClassId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string ClassName { get; set; } = null!;

    [InverseProperty("Class")]
    public virtual ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();

    [InverseProperty("Class")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [InverseProperty("Class")]
    public virtual ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
}
