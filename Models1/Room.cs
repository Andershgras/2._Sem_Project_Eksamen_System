using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

[Table("Room")]
public partial class Room
{
    [Key]
    [Column("RoomID")]
    public int RoomId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    public int? Capacity { get; set; }

    [InverseProperty("Room")]
    public virtual ICollection<RoomsToExam> RoomsToExams { get; set; } = new List<RoomsToExam>();
}
