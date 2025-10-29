using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("Hold")]
public partial class Hold
{
    [Key]
    public int HoldId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? HoldNavn { get; set; }

    [InverseProperty("Hold")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [ForeignKey("HoldId")]
    [InverseProperty("Holds")]
    public virtual ICollection<Elever> Elevs { get; set; } = new List<Elever>();

    [ForeignKey("HoldId")]
    [InverseProperty("Holds")]
    public virtual ICollection<Fag> Fags { get; set; } = new List<Fag>();
}
