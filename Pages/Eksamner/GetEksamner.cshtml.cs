using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;


namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class GetEksamnerModel : PageModel
    {
        public IEnumerable<Exam> Eksamner { get; set; }
        public ICRUDAsync<Exam> context { get; set; }


        [BindProperty(SupportsGet = true)]
        public GenericFilter? Filter { get; set; }

        public GetEksamnerModel(ICRUDAsync<Exam> service)
        {
            context = service;
            Eksamner = new List<Exam>();
            Filter = new GenericFilter();
        }

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(Filter?.FilterByName))
            {
                Eksamner = await context.GetAllAsync(Filter);
            }
            else
                Eksamner = await context.GetAllAsync();
        }
    }
}
