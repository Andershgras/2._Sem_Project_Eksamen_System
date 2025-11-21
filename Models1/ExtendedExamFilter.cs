using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2._Sem_Project_Eksamen_System.Utils;
namespace _2._Sem_Project_Eksamen_System.Models1
{
    public class ExtendedExamFilter : GenericFilter
    {
        public string FilterByClass { get; set; } = string.Empty;
        public string FilterByTeacher {  get; set; } = string.Empty;
        public string FilterByRoom {  get; set; } = string.Empty;
        public string FilterByExaminerName {  get; set; } = string.Empty;
        public int? FilterByExaminerId { get; set; }
        public DateOnly? FilterByStartDate { get; set; }
        public DateOnly? FilterByEndDate { get; set; }
    }
}
