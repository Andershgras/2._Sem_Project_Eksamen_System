using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("Exam")]
public partial class Exam
{
    [Key]
    public int ExamId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? ExamName { get; set; }

    [Column("FagID")]
    public int? FagId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? ExamType { get; set; }

    public DateTime? ExamDate { get; set; }

    [Column("HoldID")]
    public int? HoldId { get; set; }

    [Column("UnderviserID")]
    public int? UnderviserId { get; set; }

    [Column("LokaleID")]
    public int? LokaleId { get; set; }

    public int? TimeDuration { get; set; }

    public DateTime? ReEksamenDato { get; set; }

    [InverseProperty("Exam")]
    public virtual ICollection<EksaminationsUndervise> EksaminationsUndervises { get; set; } = new List<EksaminationsUndervise>();

    [ForeignKey("FagId")]
    [InverseProperty("Exams")]
    public virtual Fag? Fag { get; set; }

    [ForeignKey("HoldId")]
    [InverseProperty("Exams")]
    public virtual Hold? Hold { get; set; }

    [ForeignKey("LokaleId")]
    [InverseProperty("Exams")]
    public virtual Lokale? Lokale { get; set; }

    [ForeignKey("UnderviserId")]
    [InverseProperty("Exams")]
    public virtual Underviser? Underviser { get; set; }

    [ForeignKey("ExamId")]
    [InverseProperty("Exams")]
    public virtual ICollection<Elever> Elevs { get; set; } = new List<Elever>();
}
