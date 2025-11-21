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
    private string _className = null!;

    [StringLength(250)]
    [Unicode(false)]
    [RegularExpression(
        @"^[A-Za-z]+-[A-Z]{2}-[FV]-[VS]\d{2}[A-Z]-([1-9][0-9]?|100)sem$",
        ErrorMessage = "ClassName skal følge formatet: Uddannelse-BY-F/V-SeasonÅrBogstav-nsem (fx: Data-RO-F-V25B-2sem).")]
    public string ClassName
    {
        get => _className;
        set => _className = ClassNameNormalizer.Normalize(value);
    }

    [InverseProperty("Class")]
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    [InverseProperty("Class")]
    public virtual ICollection<StudentsToClass> StudentsToClasses { get; set; } = new List<StudentsToClass>();
}
