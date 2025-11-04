using Microsoft.AspNetCore.Mvc;

namespace _2._Sem_Project_Eksamen_System.Models
{
    public class GenericFilter
    {
        private string? _filterByName;

        [BindProperty(SupportsGet = true)]
        public string? FilterByName 
        {
            get { return _filterByName; }
            set { _filterByName = value.ToLower(); } 
        }
        

    }
}
