using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models;


namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class GetEksamnerModel : PageModel
    {
        public IEnumerable<Exam> Eksamner { get; set; }
        public ICRUD<Exam> context { get; set; }


        [BindProperty(SupportsGet = true)]
        public GenericFilter ? Filter { get; set; }

        public GetEksamnerModel(ICRUD<Exam> service)
        {
            context = service;
            Eksamner = new List<Exam>();
            Filter = new GenericFilter();
        }

        public void OnGet()
        {
            if (!string.IsNullOrEmpty(Filter.FilterByName))
            {
                Eksamner = context.GetAll(Filter);
            }
            else
                Eksamner = context.GetAll();
        }
    }
}
