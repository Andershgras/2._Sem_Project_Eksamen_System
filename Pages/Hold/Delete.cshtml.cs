using _2._Sem_Project_Eksamen_System.EFservices;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2._Sem_Project_Eksamen_System.Pages.Hold
{
    public class DeleteModel : PageModel
    {
        private readonly ICRUDAsync<Class> _service;

        public Class? Class { get; set; }
        public IEnumerable<Exam> AssociatedExams { get; set; } = Enumerable.Empty<Exam>();
        public bool CanDelete { get; set; } = true;
        public string? ErrorMessage { get; set; }

        public DeleteModel(ICRUDAsync<Class> service)
        {
            _service = service;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Class = await _service.GetItemByIdAsync(id);
            if (Class == null) return RedirectToPage("Index");

            // Check if the class has any associated exams
            // The GetItemByIdAsync already includes Exams, so we can use that
            AssociatedExams = Class.Exams ?? Enumerable.Empty<Exam>();

            if (AssociatedExams.Any())
            {
                CanDelete = false;
                ErrorMessage = "This class cannot be deleted because it is assigned to one or more exams.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Double-check on POST to ensure security
            Class = await _service.GetItemByIdAsync(id);

            if (Class == null)
            {
                return RedirectToPage("Index");
            }

            var associatedExams = Class.Exams ?? Enumerable.Empty<Exam>();

            if (associatedExams.Any())
            {
                // Reload the page with error message
                AssociatedExams = associatedExams;
                CanDelete = false;
                ErrorMessage = "This class cannot be deleted because it is assigned to one or more exams.";
                return Page();
            }

            await _service.DeleteItemAsync(id);
            return RedirectToPage("Index");
        }
    }
}
