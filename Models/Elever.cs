using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("Elever")]
public partial class Elever
{
    [Key]
    public int ElevId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? ElevNavn { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? Mail { get; set; }

    [ForeignKey("ElevId")]
    [InverseProperty("Elevs")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [ForeignKey("ElevId")]
    [InverseProperty("Elevs")]
    public virtual ICollection<Hold> Holds { get; set; } = new List<Hold>();
}
