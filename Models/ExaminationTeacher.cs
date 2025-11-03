using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("ExaminationTeacher")]
[Index("ExamId", "TeacherId", Name = "UQ_ExaminationTeacher", IsUnique = true)]
public partial class ExaminationTeacher
{
    [Key]
    [Column("ExaminationTeacherID")]
    public int ExaminationTeacherId { get; set; }

    [Column("ExamID")]
    public int ExamId { get; set; }

    [Column("TeacherID")]
    public int TeacherId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Rolle { get; set; }

    [ForeignKey("ExamId")]
    [InverseProperty("ExaminationTeachers")]
    public virtual Exam Exam { get; set; } = null!;

    [ForeignKey("TeacherId")]
    [InverseProperty("ExaminationTeachers")]
    public virtual Teacher Teacher { get; set; } = null!;
}
