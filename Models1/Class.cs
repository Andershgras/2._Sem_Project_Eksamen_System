using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Models1;

[Table("Class")]
public partial class Class
{
    [Key]
    [Column("ClassID")]
    public int ClassId { get; set; }

    // Example: Data-RO-F-V25B-2sem Format validation
    [RegularExpression(
    @"^[A-Za-z]+-[A-Z]{2}-[FV]-[VS]\d{2}[A-Z]-([1-9][0-9]?|100)sem$",
    ErrorMessage = "ExamName must be in the format: Education-City-F/V-SeasonYearClass-nsem (e.g., Data-RO-F-V25B-2sem).")]
    [StringLength(250)]
    [Unicode(false)]
    public string ClassName { get; set; } = null!;

    [InverseProperty("Class")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [InverseProperty("Class")]
    public virtual ICollection<StudentsToClass> StudentsToClasses { get; set; } = new List<StudentsToClass>();
}
