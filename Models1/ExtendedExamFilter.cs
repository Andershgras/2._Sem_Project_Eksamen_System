using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2._Sem_Project_Eksamen_System.Utils;
namespace _2._Sem_Project_Eksamen_System.Models1
{
    public class ExtendedExamFilter : GenericFilter
    {
        // Filter exams by class name or identifier
        public string FilterByClass { get; set; } = string.Empty;
        // Filter exams by Teacher name or identifier
        public string FilterByTeacher {  get; set; } = string.Empty;
        // Filter exams by Room number or identifier
        public string FilterByRoom {  get; set; } = string.Empty;
        // Filter exams by Examiner s name or identifier
        public string FilterByExaminerName {  get; set; } = string.Empty;
        // Filter exams by Examiner ID identifier
        public int? FilterByExaminerId { get; set; }
        // Filter exams by Start Date or identifier
        public DateOnly? FilterByStartDate { get; set; }
        // Filter exams by End date or identifier
        public DateOnly? FilterByEndDate { get; set; }
       
    }
}
