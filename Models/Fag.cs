using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("Fag")]
public partial class Fag
{
    [Key]
    [Column("FagID")]
    public int FagId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? Navn { get; set; }

    [InverseProperty("Fag")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [ForeignKey("FagId")]
    [InverseProperty("Fags")]
    public virtual ICollection<Hold> Holds { get; set; } = new List<Hold>();
}
