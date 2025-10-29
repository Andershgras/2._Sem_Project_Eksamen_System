using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("Underviser")]
public partial class Underviser
{
    [Key]
    [Column("UnderviserID")]
    public int UnderviserId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? UnderviserNavn { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? Mail { get; set; }

    [InverseProperty("Underviser")]
    public virtual ICollection<EksaminationsUndervise> EksaminationsUndervises { get; set; } = new List<EksaminationsUndervise>();

    [InverseProperty("Underviser")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
