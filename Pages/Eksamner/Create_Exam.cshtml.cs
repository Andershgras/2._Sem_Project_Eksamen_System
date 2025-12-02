
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
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

        [BindProperty]
        public Exam Exam { get; set; } = new Exam();
        [BindProperty] 
        public int? ExaminerTeacherId { get; set; }////R/////

        [BindProperty]
        public int? CensorTeacherId { get; set; }

        [BindProperty]
        public List<int> SelectedTeacherIds { get; set; } = new List<int>();

        [BindProperty]
        public List<int> SelectedRoomIds { get; set; } = new List<int>();

        [BindProperty]
        public bool CreateReExam { get; set; } = false;

        [BindProperty]
        public Exam ReExam { get; set; } = new Exam();

        [BindProperty]
        public List<int> SelectedReExamTeacherIds { get; set; } = new List<int>();

        [BindProperty]
        public List<int> SelectedReExamRoomIds { get; set; } = new List<int>();

        [BindProperty]
        public int NumberOfStudents { get; set; }

        [BindProperty]
        public List<GenericMultySelect> ExaminerSelect { get; set; } = new List<GenericMultySelect>();

        [BindProperty]
        public List<GenericMultySelect> RoomSelect { get; set; } = new List<GenericMultySelect>();

        [BindProperty]
        public List<GenericMultySelect> ExaminerSelectReExam { get; set; } = new List<GenericMultySelect>();

        [BindProperty]
        public List<GenericMultySelect> RoomSelectReExam { get; set; } = new List<GenericMultySelect>();

        public SelectList ClassList { get; set; } = default!;
        public MultiSelectList TeacherList { get; set; } = default!;
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
            _teachersToExamsService = teachersToExamService;
            _studentService = studentService;
            _studentsToClassesService = studentsToClassesService;
            _overlapService = overlapService;
        }
        #endregion

        private async Task PopulatePropertiesAsync()
        {
            // Populate rooms for Room selection
            var rooms = (await _roomService.GetAllAsync()).ToList();
            RoomSelect = rooms.Select(r => new GenericMultySelect
            {
                SelectValue = r.RoomId,
                SelectText = r.Name ?? string.Empty,
                IsSelected = SelectedRoomIds != null && SelectedRoomIds.Contains(r.RoomId)
            }).ToList();

            RoomSelectReExam = rooms.Select(r => new GenericMultySelect
            {
                SelectValue = r.RoomId,
                SelectText = r.Name ?? string.Empty,
                IsSelected = SelectedReExamRoomIds != null && SelectedReExamRoomIds.Contains(r.RoomId)
            }).ToList();

            // Populate teachers for Examiner selection
            var teachers = (await _teacherService.GetAllAsync()).ToList();
            ExaminerSelect = teachers.Select(t => new GenericMultySelect
            {
                SelectValue = t.TeacherId,
                SelectText = t.TeacherName ?? string.Empty,
                IsSelected = SelectedTeacherIds != null && SelectedTeacherIds.Contains(t.TeacherId)
            }).ToList();

            ExaminerSelectReExam = teachers.Select(t => new GenericMultySelect
            {
                SelectValue = t.TeacherId,
                SelectText = t.TeacherName ?? string.Empty,
                IsSelected = SelectedReExamTeacherIds != null && SelectedReExamTeacherIds.Contains(t.TeacherId)
            }).ToList();

            ClassList = new SelectList(await _classService.GetAllAsync(), "ClassId", "ClassName");
            TeacherList = new SelectList(await _teacherService.GetAllAsync(), "TeacherId", "TeacherName");
        }

        public async Task OnGet()
        {
            await PopulatePropertiesAsync();
        }

        public async Task<IActionResult> OnPost()
        {
            

            // Clear validation for all ReExam fields when not creating a ReExam
            if (!CreateReExam)
            {
                foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ReExam.")))
                    ModelState[key]?.Errors.Clear();
            }

            ExamValidationCheck();

          
            // Handle ReExam logic if creating
            if (CreateReExam)
            {
                ReExam.ClassId = Exam.ClassId;

                ReExamValidationCheck();

                ReExam.IsReExam = true;

                if (Exam.IsFinalExam)
                    ReExam.IsFinalExam = true;
            }
            else
            {
                Exam.ReExamId = null;
            }

            // Clear specific ModelState errors to re-validate
            foreach (var key in ModelState.Keys.Where(k =>
                k == "Exam.Class" || k == "ReExam.Class" || k == "ReExam.ExamName"))
            {
                ModelState[key]?.Errors.Clear();
                if (ModelState[key] != null)
                    ModelState[key].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }

            OverlapValidationCheck();

            if (!ModelState.IsValid)
            {
                LogModelState("Before Try catch");
                await PopulatePropertiesAsync();
                return Page();
            }

            try
            {
                if (CreateReExam)
                {
                    await _examService.AddItemAsync(ReExam);
                    Exam.ReExamId = ReExam.ExamId;
                    if (NumberOfStudents > 0)
                        ReExam.NumOfStud = NumberOfStudents;
                }

                await _examService.AddItemAsync(Exam);

                // Map selected Room to Exam
                if (!SelectedRoomIds.IsNullOrEmpty())
                {
                    foreach (var roomId in SelectedRoomIds.Distinct())
                    {
                        var mapping = new RoomsToExam
                        {
                            ExamId = Exam.ExamId,
                            RoomId = roomId,
                            Role = null
                        };
                        await _roomsToExamService.AddItemAsync(mapping);
                    }
                }
                // Map selected Rooms to ReExam if creating
                if (!SelectedReExamRoomIds.IsNullOrEmpty() && CreateReExam)
                {
                    foreach (var roomId in SelectedReExamRoomIds.Distinct())
                    {
                        var mapping = new RoomsToExam
                        {
                            ExamId = ReExam.ExamId,
                            RoomId = roomId,
                            Role = null
                        };
                        await _roomsToExamService.AddItemAsync(mapping);
                    }
                }

                // Assign Teachers for Exam with roles
                // 1. Assign Primary Examiner
                if (ExaminerTeacherId.HasValue)
                {
                    await _teachersToExamsService.AddTeachersToExamsAsync(ExaminerTeacherId.Value, Exam.ExamId, "Examiner");
                }

                // 2. Assign Additional Examiners (skip if same as Primary Examiner)
                if (SelectedTeacherIds != null && SelectedTeacherIds.Count > 0)
                {
                    foreach (var teacherId in SelectedTeacherIds)
                    {
                        // Don't duplicate if same as primary examiner
                        if (teacherId != ExaminerTeacherId)
                        {
                            await _teachersToExamsService.AddTeachersToExamsAsync(teacherId, Exam.ExamId, "Examiner");
                        }
                    }
                }

                // 3. Assign Censor with validation
                if (CensorTeacherId.HasValue)
                {
                    // Validate the Censor and Primary Examiner are different
                    if (ExaminerTeacherId.HasValue && ExaminerTeacherId.Value == CensorTeacherId.Value)
                    {
                        ModelState.AddModelError("CensorTeacherId", "The Censor must be different from the Primary Examiner.");
                        LogModelState("In Add Teacher: role Censor to Exam");
                        await PopulatePropertiesAsync();
                        return Page();
                    }
                    await _teachersToExamsService.AddTeachersToExamsAsync(CensorTeacherId.Value, Exam.ExamId, "Censor");
                }

                // 3. Assign Teachers for ReExam if creating
                if (CreateReExam && !SelectedReExamTeacherIds.IsNullOrEmpty())
                {
                    foreach (var teacherId in SelectedReExamTeacherIds.Distinct())
                        await _teachersToExamsService.AddTeachersToExamsAsync(teacherId, ReExam.ExamId, "Examiner");
                }

                await _studentsToExamService.AddStudentsFromClassToExamAsync(Exam.ClassId, Exam.ExamId);

                TempData["SuccessMessage"] = "Exam created successfully!";
                return RedirectToPage("GetEksamner");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating exam: {ex.Message}");
                LogModelState("Exeption");
                await PopulatePropertiesAsync();
                return Page();
            }
        }

        private void OverlapValidationCheck()
        {
            // Room availability for Exam
            if (!SelectedRoomIds.IsNullOrEmpty())
            {
                foreach (var roomId in SelectedRoomIds.Distinct())
                {
                    OverlapResult result = _overlapService.RoomHasOverlap(roomId, Exam.ExamStartDate, Exam.ExamEndDate);
                    if (result != null && result.HasConflict)
                    {
                        ModelState.AddModelError("SelectedRoomIds", result.Message ?? "Something went wrong with Rooms");
                    }
                }
            }

            // Room availability for ReExam
            if (!SelectedReExamRoomIds.IsNullOrEmpty() && CreateReExam)
            {
                foreach (var roomId in SelectedReExamRoomIds.Distinct())
                {
                    OverlapResult result = _overlapService.RoomHasOverlap(roomId, ReExam.ExamStartDate, ReExam.ExamEndDate);
                    if (result != null && result.HasConflict)
                    {
                        ModelState.AddModelError("SelectedReExamRoomIds", result.Message ?? "Something went wrong with ReExam Rooms");
                    }
                }
            }

            if (Exam.ClassId > 0)
            {
                OverlapResult result = _overlapService.ClassHasOverlap(Exam.ClassId, Exam.ExamStartDate, Exam.ExamEndDate);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("Exam.ClassId", result.Message ?? "Something went Wrong");
                }
            }

            if (CreateReExam && ReExam.ClassId > 0)
            {
                OverlapResult result = _overlapService.ClassHasOverlap(ReExam.ClassId, ReExam.ExamStartDate, ReExam.ExamEndDate);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("ReExam.ClassId", result.Message ?? "Something went Wrong");
                }
            }

            if (!SelectedTeacherIds.IsNullOrEmpty())
            {
                foreach (var teacherId in SelectedTeacherIds.Distinct())
                {
                    OverlapResult result = _overlapService.TeacherHasOverlap(teacherId,
                        Exam.ExamStartDate, Exam.ExamEndDate, Exam.IsFinalExam, Exam.IsReExam);
                    if (result != null && result.HasConflict)
                    {
                        ModelState.AddModelError("SelectedTeacherIds", result.Message ?? "Something went wrong");
                    }
                }
            }
            ////////R/////////
            if (ExaminerTeacherId.HasValue)
            {
                OverlapResult result = _overlapService.TeacherHasOverlap(ExaminerTeacherId.Value,
                    Exam.ExamStartDate, Exam.ExamEndDate, Exam.IsFinalExam, Exam.IsReExam);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("ExaminerTeacherId", result.Message ?? "Primary examiner has scheduling conflict");
                }
            }

            if (!SelectedReExamTeacherIds.IsNullOrEmpty() && CreateReExam)
            {
                foreach (var teacherId in SelectedReExamTeacherIds.Distinct())
                {
                    OverlapResult result = _overlapService.TeacherHasOverlap(teacherId,
                        ReExam.ExamStartDate, ReExam.ExamEndDate, ReExam.IsFinalExam, ReExam.IsReExam);
                    if (result != null && result.HasConflict)
                    {
                        ModelState.AddModelError("SelectedReExamTeacherIds", result.Message ?? "Something went wrong with ReExam Teachers");
                    }
                }
            }
        }
        private void ExamValidationCheck()
        {   // check for required fields
            if (Exam.ClassId <= 0)
                ModelState.AddModelError("Exam.ClassId", "A class must be selected for the exam.");
            
            if (SelectedRoomIds.IsNullOrEmpty())
                ModelState.AddModelError("SelectedRoomIds", "A room/place must be selected for the exam.");
            
            if (!ExaminerTeacherId.HasValue || ExaminerTeacherId.Value <= 0)
                ModelState.AddModelError("ExaminerTeacherId", "A primary examiner must be selected for the exam.");
            
            if (Exam.ExamStartDate == default)
                ModelState.AddModelError("Exam.ExamStartDate", "The Exam Start Date is required.");
            
            if (Exam.ExamEndDate == default)
                ModelState.AddModelError("Exam.ExamEndDate", "The Exam End Date is required.");

            // Validate Exam name + reformat if too long
            if (!string.IsNullOrWhiteSpace(Exam.ExamName) && Exam.ExamName.Length > 30)
                Exam.ExamName = Exam.ExamName.Substring(0, 30);

            if (string.IsNullOrEmpty(Exam.ExamName))
                ModelState.AddModelError("Exam.ExamName", "Needs a Title");

            // Validate Exam dates overlap
            if (Exam.ExamStartDate.HasValue && Exam.ExamEndDate.HasValue && Exam.ExamStartDate.Value > Exam.ExamEndDate.Value)
                ModelState.AddModelError("Exam.ExamStartDate", "Exam start date must not be after end date.");
            if (Exam.DeliveryDate.HasValue && Exam.ExamStartDate.HasValue && Exam.DeliveryDate.Value > Exam.ExamStartDate.Value)
                ModelState.AddModelError("Exam.DeliveryDate", "Delivery date must not be after start date.");
        }
        private void ReExamValidationCheck()
        {
            if (string.IsNullOrWhiteSpace(ReExam.ExamName))
                ReExam.ExamName = $"ReEksamen-{Exam.ExamName ?? string.Empty}";

            if (!string.IsNullOrWhiteSpace(ReExam.ExamName) && ReExam.ExamName.Length > 30)
                ReExam.ExamName = ReExam.ExamName.Substring(0, 30);

            // Validate ReExam dates
            if (ReExam.ExamStartDate.HasValue && ReExam.ExamEndDate.HasValue && ReExam.ExamStartDate.Value > ReExam.ExamEndDate.Value)
                ModelState.AddModelError("ReExam.ExamStartDate", "ReExam start date must not be after end date.");
            if (ReExam.DeliveryDate.HasValue && ReExam.ExamStartDate.HasValue && ReExam.DeliveryDate.Value > ReExam.ExamStartDate.Value)
                ModelState.AddModelError("ReExam.DeliveryDate", "ReExam delivery date must not be after start date.");
            if (ReExam.ExamStartDate.HasValue && Exam.ExamEndDate.HasValue && ReExam.ExamStartDate.Value <= Exam.ExamEndDate.Value)
                ModelState.AddModelError("ReExam.ExamStartDate", "ReExam start date must be after main exam end date.");
            if (ReExam.ExamEndDate.HasValue && Exam.ExamEndDate.HasValue && ReExam.ExamEndDate.Value <= Exam.ExamEndDate.Value)
                ModelState.AddModelError("ReExam.ExamEndDate", "ReExam end date must be after main exam end date.");
            if (ReExam.DeliveryDate.HasValue && Exam.DeliveryDate.HasValue && ReExam.DeliveryDate.Value <= Exam.DeliveryDate.Value)
                ModelState.AddModelError("ReExam.DeliveryDate", "ReExam delivery must be after main exam delivery date.");

            if (SelectedReExamTeacherIds.IsNullOrEmpty())
                SelectedReExamTeacherIds = SelectedTeacherIds;

            if (SelectedReExamRoomIds.IsNullOrEmpty())
                SelectedReExamRoomIds = SelectedRoomIds;

        }
        private void LogModelState(string place)
        {
            Console.WriteLine();
            Console.WriteLine($"=== ModelState contents === Logger activated at: {place}");
            Console.WriteLine();

            var prevColor = Console.ForegroundColor;

            try
            {

                if (ModelState.IsValid)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ModelState: VALID");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ModelState: INVALID");
                }
                Console.ForegroundColor = prevColor;
                Console.WriteLine();

                foreach (var entry in ModelState)
                {
                    var key = string.IsNullOrEmpty(entry.Key) ? "(root)" : entry.Key;
                    var validationState = entry.Value.ValidationState;
                    var errors = entry.Value.Errors;

                    if (validationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        Console.ForegroundColor = ConsoleColor.Green;

                    Console.Write($"Key: {key}  - State: {validationState}  - Errors: {errors.Count}");
                    Console.ForegroundColor = prevColor;
                    Console.WriteLine();

                    foreach (var err in errors)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"    {err.ErrorMessage}");
                        Console.ForegroundColor = prevColor;
                    }
                }
            }
            finally
            {
                Console.ForegroundColor = prevColor;
                Console.WriteLine();
                Console.WriteLine("=== End ModelState ===");
                Console.WriteLine();
            }
        }
    }
}