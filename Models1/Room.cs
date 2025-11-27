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

    [Required(ErrorMessage = "Room name is required")]
    [StringLength(250, ErrorMessage = "Room name cannot exceed 250 characters")]
    [Unicode(false)]
    public string Name { get; set; } = null!;
    [Range(0, 10000, ErrorMessage = "Capacity must be between 0 & 10000")]
    public int? Capacity { get; set; }

    [InverseProperty("Room")]
    public virtual ICollection<RoomsToExam> RoomsToExams { get; set; } = new List<RoomsToExam>();
}
