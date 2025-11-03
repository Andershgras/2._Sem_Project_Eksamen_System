using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("Teacher")]
public partial class Teacher
{
    [Key]
    [Column("TeacherID")]
    public int TeacherId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string TeacherName { get; set; } = null!;

    [StringLength(250)]
    [Unicode(false)]
    public string? Mail { get; set; }

    [InverseProperty("Teacher")]
    public virtual ICollection<ExaminationTeacher> ExaminationTeachers { get; set; } = new List<ExaminationTeacher>();

    [InverseProperty("Teacher")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
