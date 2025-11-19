using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Hold
{
    public class DeleteModel : PageModel
    {
        private readonly ICRUDAsync<Class> _service;
        private readonly ICRUDAsync<Exam> _examService;
        public Class? Class { get; set; }
        public List<Exam> Exams { get; set; } = new List<Exam>();

        public DeleteModel(ICRUDAsync<Class> service, ICRUDAsync<Exam> examService) 
        {
            _service = service;
            _examService = examService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Class = await _service.GetItemByIdAsync(id);
            if (Class == null) return RedirectToPage("Index");
            // Load related exams
            Exams = await _examService.GetAllAsync()
                .ContinueWith(t => t.Result.Where(e => e.ClassId == id).ToList());
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Check that no exams remain before attempting delete
            var exams = await _examService.GetAllAsync();
            var examCount = exams.Count(e => e.ClassId == id);
            if (examCount > 0)
            {
                ModelState.AddModelError("", "You must delete all exams for this class before you can delete the class.");
                return await OnGetAsync(id);
            }

            await _service.DeleteItemAsync(id);
            return RedirectToPage("Index");
        }
    }
}
