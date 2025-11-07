using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2._Sem_Project_Eksamen_System.Utils;

namespace _2._Sem_Project_Eksamen_System.Models1
{
    public class ExtendedStudentFilter : GenericFilter
    {
        public string FilterByEmail { get; set; } = string.Empty;
        public string FilterByClass { get; set; } = string.Empty;
        public int? FilterById { get; set; }
    }
}
