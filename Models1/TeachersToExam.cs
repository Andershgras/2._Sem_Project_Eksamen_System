using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

public partial class TeachersToExam
{
   
    [Key]
    [Column("TeacherExamID")]
    public int TeacherExamId { get; set; }
 
    [Column("TeacherID")]
    public int TeacherId { get; set; }
    
    [Column("ExamID")]
    public int ExamId { get; set; }
    
    [StringLength(100)]
    [Unicode(false)]
    public string? Role { get; set; }
    
    [ForeignKey("ExamId")]
    [InverseProperty("TeachersToExams")]
    public virtual Exam Exam { get; set; } = null!;
    
    [ForeignKey("TeacherId")]
    [InverseProperty("TeachersToExams")]
    public virtual Teacher Teacher { get; set; } = null!;
}
