using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("ClassSubject")]
[Index("ClassId", "SubjectId", Name = "UQ_ClassSubject", IsUnique = true)]
public partial class ClassSubject
{
    [Key]
    [Column("ClassSubjectID")]
    public int ClassSubjectId { get; set; }

    [Column("ClassID")]
    public int ClassId { get; set; }

    [Column("SubjectID")]
    public int SubjectId { get; set; }

    [ForeignKey("ClassId")]
    [InverseProperty("ClassSubjects")]
    public virtual Filter Class { get; set; } = null!;

    [ForeignKey("SubjectId")]
    [InverseProperty("ClassSubjects")]
    public virtual Subject Subject { get; set; } = null!;
}
