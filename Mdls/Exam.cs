using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models;

[Table("Exam")]
public partial class Exam
{
    [Key]
    [Column("ExamID")]
    public int ExamId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string ExamName { get; set; } = null!;

    [Column("SubjectID")]
    public int? SubjectId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string ExamType { get; set; } = null!;

    public DateOnly? ExamStartDate { get; set; }

    public DateOnly? ExamEndDate { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    [Column("ClassID")]
    public int? ClassId { get; set; }

    [Column("TeacherID")]
    public int? TeacherId { get; set; }

    [Column("RoomID")]
    public int? RoomId { get; set; }

    public int? TimeDuration { get; set; }

    public DateOnly? ReExamDate { get; set; }

    public bool? IsReExam { get; set; }

    [ForeignKey("ClassId")]
    [InverseProperty("Exams")]
    public virtual Filter? Class { get; set; }

    [InverseProperty("Exam")]
    public virtual ICollection<ExaminationTeacher> ExaminationTeachers { get; set; } = new List<ExaminationTeacher>();

    [ForeignKey("RoomId")]
    [InverseProperty("Exams")]
    public virtual Room? Room { get; set; }

    [InverseProperty("Exam")]
    public virtual ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();

    [ForeignKey("SubjectId")]
    [InverseProperty("Exams")]
    public virtual Subject? Subject { get; set; }

    [ForeignKey("TeacherId")]
    [InverseProperty("Exams")]
    public virtual Teacher? Teacher { get; set; }
}
