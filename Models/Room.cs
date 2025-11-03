using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("Room")]
public partial class Room
{
    [Key]
    [Column("RoomID")]
    public int RoomId { get; set; }

    [Required]
    [StringLength(250)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [InverseProperty("Room")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
