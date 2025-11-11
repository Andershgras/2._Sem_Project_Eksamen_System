using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Threading.Tasks;

namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class Create_ExamModel : PageModel
    {
        private readonly ICRUD<Exam> _examService;
        private readonly ICRUD<Class> _classService;
        private readonly IStudentsToExams _studentsToExamService;
        private readonly ICRUDAsync<Room> _roomService;
        private readonly IRoomsToExams _roomsToExamService;
        private readonly ICRUDAsync<Teacher> _teacherService;
        //private readonly ITeachersToExams _teachersToExamsService;

        [BindProperty]
        public Exam Exam { get; set; } = new Exam();

        [BindProperty]
        public bool CreateReExam { get; set; } = false;

        [BindProperty]
        public Exam ReExam { get; set; } = new Exam();

        public SelectList ClassList { get; set; } = default!;
        public SelectList RoomList { get; set; } = default!;

        // ====================================================================
        // CHANGE #1: CHANGED FROM SINGLE ROOM TO MULTIPLE ROOMS SUPPORT
        // ====================================================================
        // OLD: public int? SelectedRoomId { get; set; } - Only supported one room
        // NEW: Supports multiple room selection by changing to List<int>
        // This allows users to assign multiple rooms to a single exam
        // ====================================================================
        [BindProperty]
        public List<int> SelectedRoomIds { get; set; } = new List<int>();

        public Create_ExamModel(
            ICRUD<Exam> examService,
            ICRUD<Class> classService,
            IStudentsToExams studentsToExamService,
            ICRUDAsync<Room> roomService,
            IRoomsToExams roomsToExamService,
            ICRUDAsync<Teacher> teacherService
            
        )
        {
            _examService = examService;
            _classService = classService;
            _studentsToExamService = studentsToExamService;
            _roomService = roomService;
            _roomsToExamService = roomsToExamService;
            _teacherService = teacherService;
        }

        public async Task OnGet()
        {
            ClassList = new SelectList(_classService.GetAll(), "ClassId", "ClassName");
            RoomList = new SelectList(await _roomService.GetAllAsync(), "RoomId", "Name");
        }

        public async Task<IActionResult> OnPost()
        {
            // repopulate lists (use correct property names)
            ClassList = new SelectList(_classService.GetAll(), "ClassId", "ClassName");
            RoomList = new SelectList(await _roomService.GetAllAsync(), "RoomId", "Name");

            // Clear validation for all ReExam fields when not creating a ReExam
            if (!CreateReExam)
            {
                foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ReExam.")))
                    ModelState[key]?.Errors.Clear();
            }

            // Guard Exam.ExamName null and trim to max length
            if (!string.IsNullOrWhiteSpace(Exam.ExamName) && Exam.ExamName.Length > 30)
                Exam.ExamName = Exam.ExamName.Substring(0, 30);

            // Validate Exam dates (only when values present)
            if (Exam.ExamStartDate.HasValue && Exam.ExamEndDate.HasValue && Exam.ExamStartDate.Value > Exam.ExamEndDate.Value)
                ModelState.AddModelError("Exam.ExamStartDate", "Exam start date must not be after end date.");
            if (Exam.DeliveryDate.HasValue && Exam.ExamStartDate.HasValue && Exam.DeliveryDate.Value > Exam.ExamStartDate.Value)
                ModelState.AddModelError("Exam.DeliveryDate", "Delivery date must not be after start date.");

            // Handle ReExam logic if creating
            if (CreateReExam)
            {
                ReExam.ClassId = Exam.ClassId;

                if (string.IsNullOrWhiteSpace(ReExam.ExamName))
                    ReExam.ExamName = $"ReEksamen-{Exam.ExamName ?? string.Empty}";

                if (!string.IsNullOrWhiteSpace(ReExam.ExamName) && ReExam.ExamName.Length > 30)
                    ReExam.ExamName = ReExam.ExamName.Substring(0, 30);

                // Validate ReExam dates (only when values present)
                if (ReExam.ExamStartDate.HasValue && ReExam.ExamEndDate.HasValue && ReExam.ExamStartDate.Value > ReExam.ExamEndDate.Value)
                    ModelState.AddModelError("ReExam.ExamStartDate", "ReExam start date must not be after end date.");
                if (ReExam.DeliveryDate.HasValue && ReExam.ExamStartDate.HasValue && ReExam.DeliveryDate.Value > ReExam.ExamStartDate.Value)
                    ModelState.AddModelError("ReExam.DeliveryDate", "ReExam delivery date must not be after start date.");

                // Validate ReExam dates against Exam dates (only when values present)
                if (ReExam.ExamStartDate.HasValue && Exam.ExamEndDate.HasValue && ReExam.ExamStartDate.Value <= Exam.ExamEndDate.Value)
                    ModelState.AddModelError("ReExam.ExamStartDate", "ReExam start date must be after main exam end date.");
                if (ReExam.ExamEndDate.HasValue && Exam.ExamEndDate.HasValue && ReExam.ExamEndDate.Value <= Exam.ExamEndDate.Value)
                    ModelState.AddModelError("ReExam.ExamEndDate", "ReExam end date must be after main exam end date.");
                if (ReExam.DeliveryDate.HasValue && Exam.DeliveryDate.HasValue && ReExam.DeliveryDate.Value <= Exam.DeliveryDate.Value)
                    ModelState.AddModelError("ReExam.DeliveryDate", "ReExam delivery must be after main exam delivery date.");

                ReExam.IsReExam = true;
                if (Exam.IsFinalExam)
                    ReExam.IsFinalExam = true;
            }
            else
            {
                // Unlink ReExam from Exam (just in case)
                Exam.ReExamId = null;
            }

            // clear all Class or ExamName validation errors related to ReExam if not creating
            foreach (var key in ModelState.Keys.Where(k =>
                k == "Exam.Class" || k == "ReExam.Class" || k == "ReExam.ExamName"))
            {
                ModelState[key]?.Errors.Clear();
                if (ModelState[key] != null)
                    ModelState[key].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }

            // ====================================================================
            // CHANGE #2: UPDATED ROOM AVAILABILITY CHECK FOR MULTIPLE ROOMS
            // ====================================================================
            // OLD: Checked availability for single room only
            // NEW: Now iterates through ALL selected rooms and checks each one individually
            // This ensures no room conflicts across all selected rooms for the exam dates
            // ====================================================================
            if (SelectedRoomIds != null && SelectedRoomIds.Any())
            {
                // Ensure the exam has valid start and end dates
                if (!Exam.ExamStartDate.HasValue || !Exam.ExamEndDate.HasValue)
                {
                    ModelState.AddModelError("Exam.ExamStartDate", "Please provide both start and end dates for the exam to validate room availability.");
                    return Page();
                }

                var newStart = Exam.ExamStartDate.Value;
                var newEnd = Exam.ExamEndDate.Value;

                // ====================================================================
                // CHANGE #3: ENHANCED ERROR MESSAGES WITH SPECIFIC ROOM CONFLICTS
                // ====================================================================
                // OLD: Generic error message for room conflicts
                // NEW: Collects specific conflict details for each room and provides detailed error messages
                // This helps users understand exactly which rooms are unavailable and why
                // ====================================================================
                var conflictingRooms = new List<string>();

                // Check each selected room for scheduling conflicts
                foreach (var roomId in SelectedRoomIds)
                {
                    var conflicts = _roomsToExamService.GetAll()
                        .Where(rte => rte != null && rte.RoomId == roomId && rte.Exam != null)
                        .Where(rte =>
                            rte.Exam.ExamStartDate.HasValue &&
                            rte.Exam.ExamEndDate.HasValue &&
                            rte.Exam.ExamStartDate.Value <= newEnd &&
                            rte.Exam.ExamEndDate.Value >= newStart)
                        .ToList();

                    if (conflicts.Any())
                    {
                        var c = conflicts.First();
                        var existingStart = c.Exam.ExamStartDate.HasValue ? c.Exam.ExamStartDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "N/A";
                        var existingEnd = c.Exam.ExamEndDate.HasValue ? c.Exam.ExamEndDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "N/A";
                        var existingName = string.IsNullOrWhiteSpace(c.Exam.ExamName) ? $"Exam ID {c.Exam.ExamId}" : c.Exam.ExamName;

                        // Get room name for better error message
                        var rooms = await _roomService.GetAllAsync();
                        var roomName = rooms.FirstOrDefault(r => r.RoomId == roomId)?.Name ?? $"Room ID {roomId}";

                        // Build detailed conflict information for each problematic room
                        conflictingRooms.Add($"{roomName} (booked for '{existingName}' from {existingStart} to {existingEnd})");
                    }
                }

                if (conflictingRooms.Any())
                {
                    ModelState.AddModelError("SelectedRoomIds", $"The following rooms are already booked for the selected dates: {string.Join("; ", conflictingRooms)}. Please choose different rooms or change the dates.");
                    return Page();
                }
            }

            if (!ModelState.IsValid)
                return Page();

            // You may want to wrap the following in a transaction to avoid partial persistence on error.
            try
            {
                if (CreateReExam)
                {
                    _examService.AddItem(ReExam);
                    Exam.ReExamId = ReExam.ExamId;
                }

                _examService.AddItem(Exam);

                // ====================================================================
                // CHANGE #4: UPDATED ROOM ASSIGNMENT LOGIC FOR MULTIPLE ROOMS
                // ====================================================================
                // OLD: Assigned only one room to the exam
                // NEW: Now iterates through ALL selected rooms and creates room-to-exam mappings for each
                // This creates multiple entries in the RoomsToExam junction table
                // ====================================================================
                if (SelectedRoomIds != null && SelectedRoomIds.Any())
                {
                    var rooms = await _roomService.GetAllAsync();

                    // Create room assignment for each selected room
                    foreach (var roomId in SelectedRoomIds)
                    {
                        var roomExists = rooms.Any(r => r.RoomId == roomId);
                        if (roomExists)
                        {
                            var mapping = new RoomsToExam
                            {
                                ExamId = Exam.ExamId,
                                RoomId = roomId,
                                Role = null
                            };
                            _roomsToExamService.AddItem(mapping);
                        }
                    }
                }

                _studentsToExamService.AddStudentsFromClassToExam(Exam.ClassId, Exam.ExamId);

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