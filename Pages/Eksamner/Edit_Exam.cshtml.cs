
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using _2._Sem_Project_Eksamen_System.Utils;

namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class Edit_ExamModel : PageModel
    {
        private readonly ICRUDAsync<Exam> _service;
        private readonly ICRUDAsync<Class> _classService;
        private readonly ICRUDAsync<Teacher> _teacherService;
        private readonly ICRUDAsync<Room> _roomService;
        private readonly IStudentsToExams _studentsToExamService;
        private readonly ITeachersToExam _teachersToExamService;
        private readonly IRoomsToExams _roomsToExamService;
        private readonly ICheckOverlap _overlapsService;

        public SelectList ClassList { get; set; } = default!;
        public SelectList TeacherList { get; set; } = default!;
        public SelectList RoomList { get; set; } = default!;

        [BindProperty]
        public List<GenericMultySelect> ExaminerSelect { get; set; } = new List<GenericMultySelect>();
        [BindProperty]
        public List<GenericMultySelect> RoomSelect { get; set; } = new List<GenericMultySelect>();

        [BindProperty]
        public List<GenericMultySelect> ReExaminerSelect { get; set; } = new List<GenericMultySelect>();
        [BindProperty]
        public List<GenericMultySelect> ReRoomSelect { get; set; } = new List<GenericMultySelect>();

        [BindProperty]
        public Exam Exam { get; set; }

        [BindProperty]
        public Exam ReExam { get; set; }

        [BindProperty]
        public List<int> SelectedTeacherIds { get; set; } = new List<int>();

        [BindProperty]
        public List<int> SelectedRoomIds { get; set; } = new List<int>();

        [BindProperty]
        public int? CensorTeacherId { get; set; }

        [BindProperty]
        public List<int> SelectedReExamTeacherIds { get; set; } = new List<int>();

        [BindProperty]
        public List<int> SelectedReExamRoomIds { get; set; } = new List<int>();

        public bool HasReExam => Exam?.ReExamId.HasValue ?? false;

        [BindProperty]
        public bool EditReExam { get; set; }

        public Edit_ExamModel(
            ICRUDAsync<Exam> service,
            ICRUDAsync<Class> classService,
            ICRUDAsync<Teacher> teacherService,
            ICRUDAsync<Room> roomService,
            IStudentsToExams studentsToExamService,
            ITeachersToExam teachersToExamService,
            IRoomsToExams roomsToExamService,
            ICheckOverlap overlapsService)
        {
            _service = service;
            _classService = classService;
            _teacherService = teacherService;
            _roomService = roomService;
            _studentsToExamService = studentsToExamService;
            _teachersToExamService = teachersToExamService;
            _roomsToExamService = roomsToExamService;
            _overlapsService = overlapsService;
        }

        private async Task PopulateMultiSelectsAsync(int examId, int? reExamId)
        {
            ClassList = new SelectList(await _classService.GetAllAsync(), "ClassId", "ClassName");
            TeacherList = new SelectList(await _teacherService.GetAllAsync(), "TeacherId", "TeacherName");
           

            var teachers = (await _teacherService.GetAllAsync()).ToList();
            var rooms = (await _roomService.GetAllAsync()).ToList();

            // Main Exam
            var examinerTeachers = await _teachersToExamService.GetTeachersByExamIdAndRoleAsync(examId, "Examiner");
            SelectedTeacherIds = examinerTeachers.Select(t => t.TeacherId).ToList();

            ExaminerSelect = teachers.Select(t => new GenericMultySelect
            {
                SelectValue = t.TeacherId,
                SelectText = t.TeacherName ?? string.Empty,
                IsSelected = SelectedTeacherIds.Contains(t.TeacherId)
            }).ToList();

            var examRooms = await _roomsToExamService.GetRoomsByExamIdAsync(examId);
            SelectedRoomIds = examRooms.Select(r => r.RoomId).ToList();

            RoomSelect = rooms.Select(r => new GenericMultySelect
            {
                SelectValue = r.RoomId,
                SelectText = r.Name ?? string.Empty,
                IsSelected = SelectedRoomIds.Contains(r.RoomId)
            }).ToList();

            // ReExam (if exists, otherwise blank)
            if (HasReExam && EditReExam)
            {
                var reExaminerTeachers = await _teachersToExamService.GetTeachersByExamIdAndRoleAsync(reExamId.Value, "Examiner");
                SelectedReExamTeacherIds = reExaminerTeachers.Select(t => t.TeacherId).ToList();

                ReExaminerSelect = teachers.Select(t => new GenericMultySelect
                {
                    SelectValue = t.TeacherId,
                    SelectText = t.TeacherName ?? string.Empty,
                    IsSelected = SelectedReExamTeacherIds.Contains(t.TeacherId)
                }).ToList();

                var reExamRooms = await _roomsToExamService.GetRoomsByExamIdAsync(reExamId.Value);
                SelectedReExamRoomIds = reExamRooms.Select(r => r.RoomId).ToList();

                ReRoomSelect = rooms.Select(r => new GenericMultySelect
                {
                    SelectValue = r.RoomId,
                    SelectText = r.Name ?? string.Empty,
                    IsSelected = SelectedReExamRoomIds.Contains(r.RoomId)
                }).ToList();
            }
            else
            {
                ReExaminerSelect = teachers.Select(t => new GenericMultySelect
                {
                    SelectValue = t.TeacherId,
                    SelectText = t.TeacherName ?? string.Empty,
                }).ToList();

                ReRoomSelect = rooms.Select(r => new GenericMultySelect
                {
                    SelectValue = r.RoomId,
                    SelectText = r.Name ?? string.Empty,
                }).ToList();
            }
        }

        public async Task<IActionResult> OnGet(int id)
        {
            Exam = await _service.GetItemByIdAsync(id);
            if (Exam == null)
                return RedirectToPage("GetEksamner");

            await PopulateMultiSelectsAsync(Exam.ExamId, Exam.ReExamId);

            var censor = Exam.TeachersToExams?.FirstOrDefault(t => t.Role == "Censor");
            if (censor != null)
            {
                CensorTeacherId = censor.TeacherId;
            }

            if (Exam.RoomsToExams != null && Exam.RoomsToExams.Any())
            {
                SelectedRoomIds = Exam.RoomsToExams.Select(x => x.RoomId).ToList();
            }
            if (HasReExam)
            {
                ReExam = await _service.GetItemByIdAsync(Exam.ReExamId.Value);
                EditReExam = true;
            }
            else
            {
                ReExam = new Exam();
            }

            return Page();
        }

        public async Task<IActionResult> OnPost(int id)
        {
            await PopulateMultiSelectsAsync(id, Exam.ReExamId);

            // Bind properties from checkboxes
            SelectedTeacherIds = ExaminerSelect.Where(x => x.IsSelected).Select(x => x.SelectValue).ToList();
            SelectedRoomIds = RoomSelect.Where(x => x.IsSelected).Select(x => x.SelectValue).ToList();
            SelectedReExamTeacherIds = ReExaminerSelect.Where(x => x.IsSelected).Select(x => x.SelectValue).ToList();
            SelectedReExamRoomIds = ReRoomSelect.Where(x => x.IsSelected).Select(x => x.SelectValue).ToList();

            //Clear validation for all ReExam fields when not editing/creating a ReExam
            if (!EditReExam)
            {
                foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ReExam.")))
                    ModelState[key]?.Errors.Clear();
            }

            if (Exam.ExamName.Length > 30)
                Exam.ExamName = Exam.ExamName.Substring(0, 30);

            // Validate Main Exam dates
            if (Exam.ExamStartDate > Exam.ExamEndDate)
                ModelState.AddModelError("Exam.ExamStartDate", "Exam start date must not be after end date.");
            if (Exam.DeliveryDate > Exam.ExamStartDate)
                ModelState.AddModelError("Exam.DeliveryDate", "Delivery date must not be after start date.");

            // Edit/Update ReExam logic and validations
            if (EditReExam)
            {
                ReExam.ClassId = Exam.ClassId;
                if (string.IsNullOrWhiteSpace(ReExam.ExamName))
                    ReExam.ExamName = $"ReEksamen-{Exam.ExamName}";
                if (ReExam.ExamName.Length > 30)
                    ReExam.ExamName = ReExam.ExamName.Substring(0, 30);

                if (ReExam.ExamStartDate > ReExam.ExamEndDate)
                    ModelState.AddModelError("ReExam.ExamStartDate", "ReExam start date must not be after end date.");
                if (ReExam.DeliveryDate > ReExam.ExamStartDate)
                    ModelState.AddModelError("ReExam.DeliveryDate", "ReExam delivery date must not be after start date.");

                if (ReExam.ExamStartDate <= Exam.ExamEndDate)
                    ModelState.AddModelError("ReExam.ExamStartDate", "ReExam start date must be after main exam end date.");
                if (ReExam.ExamEndDate <= Exam.ExamEndDate)
                    ModelState.AddModelError("ReExam.ExamEndDate", "ReExam end date must be after main exam end date.");
                if (ReExam.DeliveryDate <= Exam.DeliveryDate)
                    ModelState.AddModelError("ReExam.DeliveryDate", "ReExam delivery must be after main exam delivery date.");

                ReExam.IsReExam = true;
                if (Exam.IsFinalExam)
                    ReExam.IsFinalExam = true;

                if (!HasReExam)
                    Exam.ReExamId = null;
            }
            else
            {
                Exam.ReExamId = null;
            }
            // Clear validation errors for class and name fields to avoid blocking save
            foreach (var key in ModelState.Keys.Where(k => k.Equals("Exam.Class") || k.Equals("ReExam.Class") || k.Equals("ReExam.ExamName")))
            {
                ModelState[key]?.Errors.Clear();
                if (ModelState[key] != null)
                    ModelState[key].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }

            // --------------------------------validation checks--------------------------------------------------------------

            // Teacher validation
            if (CensorTeacherId.HasValue && SelectedTeacherIds.Contains(CensorTeacherId.Value))
                ModelState.AddModelError("CensorTeacherId", "Censor cannot be the same as Examiner.");

            if (SelectedTeacherIds.Count > 0)
            {
                foreach (var teacherId in SelectedTeacherIds.Distinct())
                {
                    OverlapResult result = _overlapsService.TeacherHasOverlap(
                        teacherId, Exam.ExamStartDate, Exam.ExamEndDate, Exam.IsFinalExam, Exam.IsReExam, Exam.ExamId);
                    if (result != null && result.HasConflict)
                        ModelState.AddModelError("SelectedTeacherIds", $"Examiner: {result.Message}");
                }
            }
            if (CensorTeacherId.HasValue)
            {
                OverlapResult result = _overlapsService.TeacherHasOverlap(
                    CensorTeacherId.Value, Exam.ExamStartDate, Exam.ExamEndDate, Exam.IsFinalExam, Exam.IsReExam, Exam.ExamId);
                if (result != null && result.HasConflict)
                    ModelState.AddModelError("CensorTeacherId", $"Censor: {result.Message}");
            }

            // Teachers for ReExam
            if (EditReExam && ReExam.ExamId > 0)
            {
                if (SelectedReExamTeacherIds.Count > 0)
                {
                    foreach (var teacherId in SelectedReExamTeacherIds.Distinct())
                    {
                        OverlapResult result = _overlapsService.TeacherHasOverlap(
                            teacherId, ReExam.ExamStartDate, ReExam.ExamEndDate, ReExam.IsFinalExam, ReExam.IsReExam, ReExam.ExamId);
                        if (result != null && result.HasConflict)
                            ModelState.AddModelError("SelectedReExamTeacherIds", $"ReExam Examiner: {result.Message}");
                    }
                }
            }

            // Room validation
            if (SelectedRoomIds.Count > 0)
            {
                foreach (var roomId in SelectedRoomIds.Distinct())
                {
                    OverlapResult result = _overlapsService.RoomHasOverlap(
                        roomId, Exam.ExamStartDate, Exam.ExamEndDate, Exam.ExamId);
                    if (result != null && result.HasConflict)
                        ModelState.AddModelError("SelectedRoomIds", result.Message ?? "Something went wrong with Rooms");
                }
            }
            if (EditReExam && ReExam.ExamId > 0 && SelectedReExamRoomIds.Count > 0)
            {
                foreach (var roomId in SelectedReExamRoomIds.Distinct())
                {
                    OverlapResult result = _overlapsService.RoomHasOverlap(
                        roomId, ReExam.ExamStartDate, ReExam.ExamEndDate, ReExam.ExamId);
                    if (result != null && result.HasConflict)
                        ModelState.AddModelError("SelectedReExamRoomIds", $"ReExam Room: {result.Message}");
                }
            }

            // Class validation (main and reexam)
            if (Exam.ClassId > 0)
            {
                OverlapResult result = _overlapsService.ClassHasOverlap(
                    Exam.ClassId, Exam.ExamStartDate, Exam.ExamEndDate, Exam.ExamId);
                if (result != null && result.HasConflict)
                    ModelState.AddModelError("Exam.ClassId", result.Message ?? "Something went Wrong");
            }
            if (EditReExam && ReExam.ExamId > 0 && ReExam.ClassId > 0)
            {
                OverlapResult result = _overlapsService.ClassHasOverlap(
                    ReExam.ClassId, ReExam.ExamStartDate, ReExam.ExamEndDate, ReExam.ExamId);
                if (result != null && result.HasConflict)
                    ModelState.AddModelError("ReExam.ClassId", $"ReExam Class: {result.Message}");
            }

            // ------------------------------------------End validation checks--------------------------------------------------------------
            if (!ModelState.IsValid)
                return Page();

            // ---- Persistence logic ----
            // Save or update ReExam
            if (EditReExam)
            {
                if (!HasReExam)
                {
                    await _service.AddItemAsync(ReExam);
                    Exam.ReExamId = ReExam.ExamId;
                }
                else
                {
                    await _service.UpdateItemAsync(ReExam);
                }
            }
            else if (HasReExam)
            {
                await _service.DeleteItemAsync(Exam.ReExamId.Value);
                Exam.ReExamId = null;
            }

            await _service.UpdateItemAsync(Exam);

            // Teachers for Exam
            await _teachersToExamService.RemoveAllFromExamAsync(Exam.ExamId);
            if (SelectedTeacherIds.Count > 0)
            {
                foreach (var teacherId in SelectedTeacherIds)
                    await _teachersToExamService.AddTeachersToExamsAsync(teacherId, Exam.ExamId, "Examiner");
            }
            if (CensorTeacherId.HasValue)
                await _teachersToExamService.AddTeachersToExamsAsync(CensorTeacherId.Value, Exam.ExamId, "Censor");

            // Rooms for Exam
            await _roomsToExamService.RemoveAllRoomsFromExamAsync(Exam.ExamId);
            if (SelectedRoomIds.Count > 0)
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
            await _studentsToExamService.SyncStudentsForExamAndClassAsync(Exam.ExamId, Exam.ClassId);

            // ReExam assignments
            if (EditReExam && ReExam.ExamId > 0)
            {
                // Teachers
                await _teachersToExamService.RemoveAllFromExamAsync(ReExam.ExamId);
                if (SelectedReExamTeacherIds.Count > 0)
                {
                    foreach (var teacherId in SelectedReExamTeacherIds)
                        await _teachersToExamService.AddTeachersToExamsAsync(teacherId, ReExam.ExamId, "Examiner");
                }
                // No Censor for ReExam in multi-edit (can be added if needed)

                // Rooms
                await _roomsToExamService.RemoveAllRoomsFromExamAsync(ReExam.ExamId);
                if (SelectedReExamRoomIds.Count > 0)
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
                await _studentsToExamService.AddStudentsFromClassToExamAsync(ReExam.ExamId, ReExam.ClassId);
            }

            TempData["SuccessMessage"] = "Exam updated successfully!";
            return RedirectToPage("GetEksamner");
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
                        Console.WriteLine($"    • {err.ErrorMessage}");
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
        
    
