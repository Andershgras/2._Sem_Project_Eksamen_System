using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace _2._Sem_Project_Eksamen_System.Pages.Hold
{
    public class CreateModel : PageModel
    {
        private readonly ICRUDAsync<Class> _service;

        [BindProperty]
        public Class ClassItem { get; set; }

        // Separate properties for each part of the class name
        [BindProperty]
        [Required(ErrorMessage = "Education is required")]
        public string Education { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "City is required")]
        [MinLength(2, ErrorMessage = "City must be at least 2 characters")]
        public string City { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Type (Physical/Virtual) is required")]
        public string PhysicalOrVirtual { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Season is required")]
        public string Season { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Year is required")]
        public string Year { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Letter is required")]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Letter must be a single character")]
        [RegularExpression(@"[A-Za-z]", ErrorMessage = "Letter must be A-Z")]
        public string Letter { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Semester is required")]
        [Range(1, 100, ErrorMessage = "Semester must be between 1 and 100")]
        public int Semester { get; set; }

        public CreateModel(ICRUDAsync<Class> service)
        {
            _service = service;
            ClassItem = new Class();
        }

        public void OnGet()
        {
            // Just display the form
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Builds the class name from the individual components
            var cityCode = City[..Math.Min(2, City.Length)].ToUpper();
            var letterUpper = Letter.ToUpper();

            ClassItem.ClassName = $"{Education}-{cityCode}-{PhysicalOrVirtual}-{Season}{Year}{letterUpper}-{Semester}sem";

            try
            {
                await _service.AddItemAsync(ClassItem);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating class: {ex.Message}");
                return Page();
            }
        }
    }
}
