using _2._Sem_Project_Eksamen_System.EFservices;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class Create_ExamModel : PageModel
    {
        private readonly ICRUD<Exam> _examService;
        private readonly ICRUDAsync<Class> _classService;
        private readonly ICRUDAsync<Student> _studentService;
        private readonly IStudentsToExams _studentsToExamService;
        private readonly IStudentsToClasses _studentsToClassesService;
        private readonly ICRUDAsync<Room> _roomService;
        private readonly IRoomsToExams _roomsToExamService;
        private readonly ICRUDAsync<Teacher> _teacherService;
        private readonly ITeachersToExam _teachersToExamsService;

        [BindProperty]
        public Exam Exam { get; set; } = new Exam();

        [BindProperty]
        public bool CreateReExam { get; set; } = false;

        [BindProperty]
        public Exam ReExam { get; set; } = new Exam();

        public SelectList ClassList { get; set; } = default!;
        public SelectList RoomList { get; set; } = default!;
        public SelectList TeacherList { get; set; } = default!;
        public SelectList TeacherListReExam { get; set; } = default!;
        public SelectList StudentList { get; set; } = default!;

        [BindProperty]
        public int? SelectedRoomId { get; set; }

        [BindProperty]
        public List<int> SelectedTeacherIds { get; set; } = new List<int>();

        [BindProperty]
        public List<int> SelectedReExamTeacherIds { get; set; } = new List<int>();

        [BindProperty]
        public List<int> SelectedReExamStudentIds { get; set; } = new List<int>();

        [BindProperty]
        public int NumberOfStudents { get; set; }

        [BindProperty]
        public Dictionary<int, string> TeacherRoles { get; set; } = new Dictionary<int, string>();

        // /////////////////TESTING ROLES ///////////////////////
        public List<SelectListItem> RoleOptions { get; } = new List<SelectListItem>
{
    new SelectListItem { Value = "Examiner", Text = "Examiner" },
    new SelectListItem { Value = "Censor", Text = "Censor" }
};

        public Create_ExamModel(
            ICRUD<Exam> examService,
            ICRUDAsync<Class> classService,
            IStudentsToExams studentsToExamService,
            ICRUDAsync<Room> roomService,
            IRoomsToExams roomsToExamService,
            ICRUDAsync<Teacher> teacherService,
            ITeachersToExam teachersToExamService,
            ICRUDAsync<Student> studentService,
            IStudentsToClasses studentsToClassesService
        )
        {
            _examService = examService;
            _classService = classService;
            _studentsToExamService = studentsToExamService;
            _roomService = roomService;
            _roomsToExamService = roomsToExamService;
            _teacherService = teacherService;
            _teachersToExamsService = teachersToExamService;
            _studentService = studentService;
            _studentsToClassesService = studentsToClassesService;
        }

        public async Task OnGet()
        {
            ClassList = new SelectList(await _classService.GetAllAsync(), "ClassId", "ClassName");
            RoomList = new SelectList(await _roomService.GetAllAsync(), "RoomId", "Name");
            TeacherList = new SelectList(await _teacherService.GetAllAsync(), "TeacherId", "TeacherName");

        }
        // Handles The student menu population based on selected class
    
        public async Task<IActionResult> OnPost()
        {
            // repopulate lists (use correct property names)
            ClassList = new SelectList(await _classService.GetAllAsync(), "ClassId", "ClassName");
            RoomList = new SelectList(await _roomService.GetAllAsync(), "RoomId", "Name");
            TeacherList = new SelectList(await _teacherService.GetAllAsync(), "TeacherId", "TeacherName");


            // Clear validation for all ReExam fields when not creating a ReExam
            if (!CreateReExam)
            {
                foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ReExam.")))
                    ModelState[key]?.Errors.Clear();
            }

            Exam.NumOfStud = ClassList.Count();

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

                // if no teachers selected for ReExam, copy from main Exam
                if (SelectedReExamTeacherIds == null)
                    SelectedReExamTeacherIds = SelectedTeacherIds;
                
                         
                
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

            // Debug Logging
            foreach (var entry in ModelState)
            {
                var state = entry.Value.ValidationState;
                var errorCount = entry.Value.Errors.Count;
                Console.WriteLine($"Key: {entry.Key} - State: {state} - Errors: {errorCount}");
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"  Error: {error.ErrorMessage}");
                }
            }

            if (!ModelState.IsValid)
                return Page();

            // check room availability for the selected room and date range ---
            if (SelectedRoomId.HasValue)
            {
                // Ensure the exam has valid start and end dates
                if (!Exam.ExamStartDate.HasValue || !Exam.ExamEndDate.HasValue)
                {
                    ModelState.AddModelError("Exam.ExamStartDate", "Please provide both start and end dates for the exam to validate room availability.");
                    return Page();
                }

                // Use inclusive overlap: existingStart <= newEnd && existingEnd >= newStart
                var newStart = Exam.ExamStartDate.Value;
                var newEnd = Exam.ExamEndDate.Value;

                var conflicts = _roomsToExamService.GetAll()
                    .Where(rte => rte != null && rte.RoomId == SelectedRoomId.Value && rte.Exam != null)
                    .Where(rte =>
                        rte.Exam.ExamStartDate.HasValue &&
                        rte.Exam.ExamEndDate.HasValue &&
                        rte.Exam.ExamStartDate.Value <= newEnd &&
                        rte.Exam.ExamEndDate.Value >= newStart)
                    .ToList();

                if (conflicts.Any())
                {
                    // Build a helpful error message listing the first conflict (you can expand to list all)
                    var c = conflicts.First();
                    var existingStart = c.Exam.ExamStartDate.HasValue ? c.Exam.ExamStartDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "N/A";
                    var existingEnd = c.Exam.ExamEndDate.HasValue ? c.Exam.ExamEndDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "N/A";
                    var existingName = string.IsNullOrWhiteSpace(c.Exam.ExamName) ? $"Exam ID {c.Exam.ExamId}" : c.Exam.ExamName;

                    ModelState.AddModelError("SelectedRoomId", $"Selected room is already booked for '{existingName}' ({existingStart} — {existingEnd}). Choose another room or change the dates.");
                    return Page();
                }
            }
            

           
            try
            {
                if (CreateReExam)
                {
                    _examService.AddItem(ReExam);
                    Exam.ReExamId = ReExam.ExamId;

                    if (NumberOfStudents > 0 && NumberOfStudents != null) // set number of students for re-exam if specified
                        ReExam.NumOfStud = NumberOfStudents;
                }

                _examService.AddItem(Exam);

                // Map selected Room to Exam (only if selected and exists)
                if (SelectedRoomId.HasValue)
                {

                    // inefefficient but simple existence check loading all rooms the first time
                    // Consider optimizing with a dedicated existence check method in ICRUDAsync<Room> if needed
                    var rooms = await _roomService.GetAllAsync();
                    var roomExists = rooms.Any(r => r.RoomId == SelectedRoomId.Value);

                    if (roomExists)
                    {
                        var mapping = new RoomsToExam
                        {
                            ExamId = Exam.ExamId,
                            RoomId = SelectedRoomId.Value,
                            Role = null
                        };
                    
                        _roomsToExamService.AddItem(mapping);
                    }
                }

                // Persist selected examiners (SelectedTeacherIds)
                if (SelectedTeacherIds != null && SelectedTeacherIds.Count > 0)
                {
                    foreach (var teacherId in SelectedTeacherIds.Distinct())
                    {
                        _teachersToExamsService.AddTeachersToExams(teacherId, Exam.ExamId);
                    }
                    foreach (var teacherId in SelectedReExamTeacherIds.Distinct())
                    {
                        _teachersToExamsService.AddTeachersToExams(teacherId, Exam.ExamId);
                    }
                }
                /////////////////////TESTING ROLES  ///////////////////////
                // Persist selected examiners (SelectedTeacherIds) with roles
                if (SelectedTeacherIds != null && SelectedTeacherIds.Count > 0)
                {
                    foreach (var teacherId in SelectedTeacherIds.Distinct())
                    {
                        // Get the role for this teacher, default to "Examiner" if not specified
                        string role = "Examiner";
                        if (TeacherRoles != null && TeacherRoles.ContainsKey(teacherId))
                        {
                            role = TeacherRoles[teacherId];
                        }

                        _teachersToExamsService.AddTeachersToExams(teacherId, Exam.ExamId, role);
                    }
                }

                // Add all students from the selected class to the exam
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