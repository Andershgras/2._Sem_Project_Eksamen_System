using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Interfaces;

namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class Create_ExamModel : PageModel
    {
        private readonly ICRUD<Exam> _examService;
        private readonly EksamensDBContext _context;

        [BindProperty]
        public Exam Exam { get; set; } = default!;

        public SelectList ClassList { get; set; } = default!;
        public SelectList RoomList { get; set; } = default!;

        public Create_ExamModel(ICRUD<Exam> examService, EksamensDBContext context)
        {
            _examService = examService;
            _context = context;
        }

        public void OnGet()
        {
            // Load dropdown lists
            ClassList = new SelectList(_context.Classes, "ClassId", "ClassName");
            RoomList = new SelectList(_context.Rooms, "RoomId", "Name");
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // Reload lists if validation fails
                ClassList = new SelectList(_context.Classes, "ClassId", "ClassName");
                RoomList = new SelectList(_context.Rooms, "RoomId", "Name");
                return Page();
            }

            try
            {
                _examService.AddItem(Exam);
                TempData["SuccessMessage"] = "Exam created successfully!";
                return RedirectToPage("./GetEksamner");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating exam: {ex.Message}");
                ClassList = new SelectList(_context.Classes, "ClassId", "ClassName");
                RoomList = new SelectList(_context.Rooms, "RoomId", "Name");
                return Page();
            }
        }
    }
}
