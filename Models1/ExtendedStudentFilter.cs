using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2._Sem_Project_Eksamen_System.Utils;

namespace _2._Sem_Project_Eksamen_System.Models1
{
    public class ExtendedStudentFilter : GenericFilter
    {
        private int? _filterById = null;
        public string FilterByEmail { get; set; } = string.Empty;
        public string FilterByClass { get; set; } = string.Empty;
        
        public int? FilterById
        {
            get { return _filterById; }
            set { if (value < 0) _filterById = value * -1;
                    else _filterById = value;
            }
        }
    }
}
