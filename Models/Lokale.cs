using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("Lokale")]
public partial class Lokale
{
    [Key]
    public int LokaleId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? Navn { get; set; }

    [InverseProperty("Lokale")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
