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
        private readonly ICRUD<Class> _classService;
        private readonly ICRUD<Room> _roomService;

        [BindProperty]
        public Exam Exam { get; set; } = new Exam();

        public SelectList ClassList { get; set; } = default!;
        public SelectList RoomList { get; set; } = default!;

        public Create_ExamModel(
            ICRUD<Exam> examService,
            ICRUD<Class> classService,
            ICRUD<Room> roomService
        )
        {
            _examService = examService;
            _classService = classService;
            _roomService = roomService;
        }

        public void OnGet()
        {
            // Populate dropdowns for classes and rooms
            ClassList = new SelectList(_classService.GetAll(), "ClassId", "ClassName");
            RoomList = new SelectList(_roomService.GetAll(), "RoomId", "Name");
        }

        public IActionResult OnPost()
        {
            // Repopulate dropdowns for redisplay on errors
            ClassList = new SelectList(_classService.GetAll(), "ClassId", "ClassName");
            RoomList = new SelectList(_roomService.GetAll(), "RoomId", "Name");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _examService.AddItem(Exam);
                TempData["SuccessMessage"] = "Exam created successfully!";
                return RedirectToPage("GetEksamner");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating exam: {ex.Message}");
                return Page();
            }
        }
    }
}