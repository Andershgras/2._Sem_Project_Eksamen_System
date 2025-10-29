using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[PrimaryKey("ExamId", "UnderviserId")]
[Table("EksaminationsUndervise")]
public partial class EksaminationsUndervise
{
    [Key]
    [Column("ExamID")]
    public int ExamId { get; set; }

    [Key]
    [Column("UnderviserID")]
    public int UnderviserId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? Rolle { get; set; }

    [ForeignKey("ExamId")]
    [InverseProperty("EksaminationsUndervises")]
    public virtual Exam Exam { get; set; } = null!;

    [ForeignKey("UnderviserId")]
    [InverseProperty("EksaminationsUndervises")]
    public virtual Underviser Underviser { get; set; } = null!;
}
