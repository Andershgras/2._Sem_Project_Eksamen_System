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
        //Filter by e mail
        public string FilterByEmail { get; set; } = string.Empty;
        // Filter student by class
        public string FilterByClass { get; set; } = string.Empty;
        // Filter student by specific sutdent ID
        public int? FilterById { get; set; }
        
        public int? FilterById
        {
            get { return _filterById; }
            set { if (value < 0) _filterById = value * -1;
                    else _filterById = value;
            }
        }

        
    }
}
