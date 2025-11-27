using _2._Sem_Project_Eksamen_System.EFservices;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using _2._Sem_Project_Eksamen_System.Utils;

namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class Create_ExamModel : PageModel
    {
        #region Services
        private readonly ICRUDAsync<Exam> _examService;
        private readonly ICRUDAsync<Class> _classService;
        private readonly ICRUDAsync<Student> _studentService;
        private readonly IStudentsToExams _studentsToExamService;
        private readonly IStudentsToClasses _studentsToClassesService;
        private readonly ICRUDAsync<Room> _roomService;
        private readonly IRoomsToExams _roomsToExamService;
        private readonly ICRUDAsync<Teacher> _teacherService;
        private readonly ITeachersToExam _teachersToExamsService;
        private readonly ICheckOverlap _overlapService;
        #endregion
        #region Properties

        // Bind properties for form Exam creation

        [BindProperty]
        public Exam Exam { get; set; } = new Exam();

        [BindProperty]
        public int? CensorTeacherId { get; set; }

        [BindProperty]
        public int? SelectedRoomId { get; set; }

        [BindProperty]
        public List<int> SelectedTeacherIds { get; set; } = new List<int>();

        // ReExam related properties --------------------------------------------

        [BindProperty]
        public bool CreateReExam { get; set; } = false;

        [BindProperty]
        public Exam ReExam { get; set; } = new Exam();

        [BindProperty]
        public List<int> SelectedReExamTeacherIds { get; set; } = new List<int>();

        //[BindProperty]
        //public List<int> SelectedReExamStudentIds { get; set; } = new List<int>();  Not used currently 

        [BindProperty]
        public int NumberOfStudents { get; set; }

        // property for Selectors ------------------------------------------------

        [BindProperty]
        public List<GenericMultySelect> ExaminerSelect { get; set; } = new List<GenericMultySelect>();
        public SelectList ClassList { get; set; } = default!;
        public SelectList RoomList { get; set; } = default!;
        public MultiSelectList TeacherList { get; set; } = default!;
        public SelectList TeacherListReExam { get; set; } = default!;
        public SelectList StudentList { get; set; } = default!;
        #endregion 

        #region Constructor
        public Create_ExamModel(
            ICRUDAsync<Exam> examService,
            ICRUDAsync<Class> classService,
            IStudentsToExams studentsToExamService,
            ICRUDAsync<Room> roomService,
            IRoomsToExams roomsToExamService,
            ICRUDAsync<Teacher> teacherService,
            ITeachersToExam teachersToExamService,
            ICRUDAsync<Student> studentService,
            IStudentsToClasses studentsToClassesService,
            ICheckOverlap overlapService
        )
        {
            _examService = examService;
            _classService = classService;
            _studentsToExamService = studentsToExamService;
            _roomService = roomService;
            _roomsToExamService = roomsToExamService;
            _teacherService = teacherService;
            _teachersToExamsService = teachersToExamService; // Now they match
            _studentService = studentService;
            _studentsToClassesService = studentsToClassesService;
            _overlapService = overlapService;
        }
        #endregion
        private async Task PopulateExaminerSelectAsync()
        {
            var teachers = (await _teacherService.GetAllAsync()).ToList();
            ExaminerSelect = teachers.Select(t => new GenericMultySelect
            {
                SelectValue = t.TeacherId,
                SelectText = t.TeacherName ?? string.Empty,
                IsSelected = SelectedTeacherIds != null && SelectedTeacherIds.Contains(t.TeacherId)
            }).ToList();
        }

        public async Task OnGet()
        {
            ClassList = new SelectList(await _classService.GetAllAsync(), "ClassId", "ClassName");
            RoomList = new SelectList(await _roomService.GetAllAsync(), "RoomId", "Name");
            TeacherList = new SelectList(await _teacherService.GetAllAsync(), "TeacherId", "TeacherName");
            await PopulateExaminerSelectAsync();
        }
        // Handles The student menu population based on selected class
    
        public async Task<IActionResult> OnPost()
        {
                   // repopulate lists (use correct property names)
            ClassList = new SelectList(await _classService.GetAllAsync(), "ClassId", "ClassName");

            var rooms = await _roomService.GetAllAsync(); // Load all rooms once
            RoomList = new SelectList(rooms, "RoomId", "Name");

            TeacherList = new SelectList(await _teacherService.GetAllAsync(), "TeacherId", "TeacherName");
            await PopulateExaminerSelectAsync();

            SelectedTeacherIds = ExaminerSelect.Where(x => x.IsSelected).Select(x => x.SelectValue).ToList();


            // Clear validation for all ReExam fields when not creating a ReExam
            if (!CreateReExam)
            {
                foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ReExam.")))
                    ModelState[key]?.Errors.Clear();
            }
       
            if (Exam.ClassId <= 0)
            {
                ModelState.AddModelError("Exam.ClassId", "A class must be selected for the exam.");
            }
            if (!SelectedRoomId.HasValue || SelectedRoomId.Value <= 0)
            {
                ModelState.AddModelError("SelectedRoomId", "A room/place must be selected for the exam.");
            }
            if (Exam.ExamStartDate == default)
            {
                ModelState.AddModelError("Exam.ExamStartDate", "The Exam Start Date is required.");
            }
            if (Exam.ExamEndDate == default)
            {
                ModelState.AddModelError("Exam.ExamEndDate", "The Exam End Date is required.");
            }

            Exam.NumOfStud = ClassList.Count();

            // Guard Exam.ExamName null and trim to max length
            if (!string.IsNullOrWhiteSpace(Exam.ExamName) && Exam.ExamName.Length > 30)
                Exam.ExamName = Exam.ExamName.Substring(0, 30);
            
            if (string.IsNullOrEmpty(Exam.ExamName))
                ModelState.AddModelError("Exam.ExamName", "Needs a Tittle");
            

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

            // ModelState validation checks -----------------------------------
            if (SelectedRoomId.HasValue)// check room availability for the selected room and date range ---
            {
                OverlapResult result = _overlapService.RoomHasOverlap(SelectedRoomId.Value, Exam.ExamStartDate, Exam.ExamEndDate);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("SelectedRoomId", result.Message ?? "Something went wrong with Rooms");
                    return Page();
                }
            }

            if(CreateReExam && SelectedRoomId.HasValue) // check room availability for ReExam if creating
            {
                OverlapResult result = _overlapService.RoomHasOverlap(SelectedRoomId.Value, ReExam.ExamStartDate, ReExam.ExamEndDate);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("SelectedRoomId", result.Message ?? "Something went Wrong");
                    return Page();
                }
            }

            if (Exam.ClassId > 0) // check class availability for Exam
            {
                OverlapResult result = _overlapService.ClassHasOverlap(Exam.ClassId, Exam.ExamStartDate, Exam.ExamEndDate);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("Exam.ClassId", result.Message ?? "Something went Wrong");
                    return Page();
                }
            }

            if (CreateReExam && ReExam.ClassId > 0) // check class availability for ReExam if creating
            {
                OverlapResult result = _overlapService.ClassHasOverlap(ReExam.ClassId, ReExam.ExamStartDate, ReExam.ExamEndDate);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("ReExam.ClassId", result.Message ?? "Something went Wrong");
                    return Page();
                }
            }

            if (!SelectedTeacherIds.IsNullOrEmpty()) // check teacher availability for exam
            {
                foreach (var teacherId in SelectedTeacherIds.Distinct())
                {
                    OverlapResult result = _overlapService.TeacherHasOverlap(teacherId, 
                        Exam.ExamStartDate, Exam.ExamEndDate, Exam.IsFinalExam, Exam.IsReExam);
                    if (result != null && result.HasConflict)
                    {
                        ModelState.AddModelError("SelectedTeacherIds", result.Message ?? "Something went wrong");
                        return Page();
                    }
                }
               
            }
            // validatetions end -------------------------------------------------

            if (!ModelState.IsValid)
                return Page();

            // Create Exam and related entities -------------------------------
            try
            {
                if (CreateReExam)
                {
                    await _examService.AddItemAsync(ReExam);
                    Exam.ReExamId = ReExam.ExamId;

                    if (NumberOfStudents > 0) // set number of students for re-exam if specified
                        ReExam.NumOfStud = NumberOfStudents;
                }

                await _examService.AddItemAsync(Exam);

                // Map selected Room to Exam (only if selected and exists)
                if (SelectedRoomId.HasValue)
                {
                    // Ensure the room exists
                    if (rooms.Any(r => r.RoomId == SelectedRoomId.Value))
                    {
                        var mapping = new RoomsToExam
                        {
                            ExamId = Exam.ExamId,
                            RoomId = SelectedRoomId.Value,
                            Role = null
                        };
                    
                        await _roomsToExamService.AddItemAsync(mapping);
                    }
                   
                }

                // Persist selected examiners (SelectedTeacherIds) with roles
                // --- TEACHER ASSIGNMENT LOGIC ---

                // 1. Assign Examiner using the new property
                if (SelectedTeacherIds.Count > 0){
                    foreach (var teacherId in SelectedTeacherIds)
                    {
                        await _teachersToExamsService.AddTeachersToExamsAsync(teacherId, Exam.ExamId, "Examiner");
                    }
                }

                // 2. Assign Censor
                if (CensorTeacherId.HasValue)
                {
                    // validate the Censor and Examiner are different
                    if (SelectedTeacherIds.Contains(CensorTeacherId.Value))
                    {
                        ModelState.AddModelError("CensorTeacherId", "The Censor must be different from the Examiner.");
                        return Page();
                    }

                    await _teachersToExamsService.AddTeachersToExamsAsync(CensorTeacherId.Value, Exam.ExamId, "Censor");
                }

                // 3. Assign Teachers for ReExam if creating
                if (CreateReExam && SelectedReExamTeacherIds != null && SelectedReExamTeacherIds.Count > 0)
                {
                    foreach (var teacherId in SelectedReExamTeacherIds.Distinct())
                        await _teachersToExamsService.AddTeachersToExamsAsync(teacherId, ReExam.ExamId);
                }

                // Add all students from the selected class to the exam
                await _studentsToExamService.AddStudentsFromClassToExamAsync(Exam.ClassId, Exam.ExamId);

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