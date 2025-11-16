using _2._Sem_Project_Eksamen_System.EFservices;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;

namespace _2._Sem_Project_Eksamen_System.Pages.Eksamner
{
    public class Edit_ExamModel : PageModel
    {
        private readonly ICRUD<Exam> _service;
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
        public Exam Exam { get; set; }

        [BindProperty]
        public Exam ReExam { get; set; }

        [BindProperty]
        public List<int> SelectedTeacherIds { get; set; } = new List<int>();

        [BindProperty]
        public int? ExaminerTeacherId { get; set; }
        [BindProperty]
        public int? CensorTeacherId { get; set; }

        [BindProperty]
        public int? SelectedRoomId { get; set; }

        public bool HasReExam => Exam?.ReExamId.HasValue ?? false;

        [BindProperty]
        public bool EditReExam { get; set; }

        public Edit_ExamModel(
            ICRUD<Exam> service,
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

        public async Task<IActionResult> OnGet(int id)
        {
            // Load dropdown lists
            var allClasses = await _classService.GetAllAsync();
            var allTeachers = await _teacherService.GetAllAsync();
            var allRooms = await _roomService.GetAllAsync();

            ClassList = new SelectList(allClasses, "ClassId", "ClassName");
            TeacherList = new SelectList(allTeachers, "TeacherId", "TeacherName");
            RoomList = new SelectList(allRooms, "RoomId", "Name");

            // Load exam with related data
            Exam = _service.GetItemById(id);
            if (Exam == null)
                return RedirectToPage("GetEksamner");

            // Load currently assigned teachers and their roles
            if (Exam.TeachersToExams != null)
            {
                var examiner = Exam.TeachersToExams.FirstOrDefault(t => t.Role == "Examiner");
                if (examiner != null)
                {
                    ExaminerTeacherId = examiner.TeacherId;
                }
                var censor = Exam.TeachersToExams.FirstOrDefault(t => t.Role == "Censor");
                if (censor != null)
                {
                    CensorTeacherId = censor.TeacherId;
                }
            }

            // Load currently assigned room
            if (Exam.RoomsToExams != null && Exam.RoomsToExams.Any())
            {
                SelectedRoomId = Exam.RoomsToExams.First().RoomId;
            }

            if (HasReExam)
            {
                ReExam = _service.GetItemById(Exam.ReExamId.Value);
                EditReExam = true;
            }
            else
            {
                ReExam = new Exam();
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            // Reload dropdown lists - USE DIFFERENT VARIABLE NAMES
            var allClasses = await _classService.GetAllAsync();
            var allTeachers = await _teacherService.GetAllAsync();
            var allRooms = await _roomService.GetAllAsync();

            ClassList = new SelectList(allClasses, "ClassId", "ClassName");
            TeacherList = new SelectList(allTeachers, "TeacherId", "TeacherName");
            RoomList = new SelectList(allRooms, "RoomId", "Name");

            // Clear validation for all ReExam fields when not editing/creating a ReExam
            if (!EditReExam)
            {
                foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ReExam.")))
                    ModelState[key]?.Errors.Clear();
            }

            // Truncate Exam name if too long
            if (Exam.ExamName.Length > 30)
                Exam.ExamName = Exam.ExamName.Substring(0, 30);

            // Validate Exam dates
            if (Exam.ExamStartDate > Exam.ExamEndDate)
                ModelState.AddModelError("Exam.ExamStartDate", "Exam start date must not be after end date.");
            if (Exam.DeliveryDate > Exam.ExamStartDate)
                ModelState.AddModelError("Exam.DeliveryDate", "Delivery date must not be after start date.");

            // Handle ReExam create/update logic if editing/creating
            if (EditReExam)
            {
                // Always set ReExam.ClassId from Exam.ClassId!
                ReExam.ClassId = Exam.ClassId;

                if (string.IsNullOrWhiteSpace(ReExam.ExamName))
                    ReExam.ExamName = $"ReEksamen-{Exam.ExamName}";

                if (ReExam.ExamName.Length > 30)
                    ReExam.ExamName = ReExam.ExamName.Substring(0, 30);

                // Validate ReExam dates
                if (ReExam.ExamStartDate > ReExam.ExamEndDate)
                    ModelState.AddModelError("ReExam.ExamStartDate", "ReExam start date must not be after end date.");
                if (ReExam.DeliveryDate > ReExam.ExamStartDate)
                    ModelState.AddModelError("ReExam.DeliveryDate", "ReExam delivery date must not be after start date.");

                // Validate ReExam dates against Exam dates
                if (ReExam.ExamStartDate <= Exam.ExamEndDate)
                    ModelState.AddModelError("ReExam.ExamStartDate", "ReExam start date must be after main exam end date.");
                if (ReExam.ExamEndDate <= Exam.ExamEndDate)
                    ModelState.AddModelError("ReExam.ExamEndDate", "ReExam end date must be after main exam end date.");
                if (ReExam.DeliveryDate <= Exam.DeliveryDate)
                    ModelState.AddModelError("ReExam.DeliveryDate", "ReExam delivery must be after main exam delivery date.");

                ReExam.IsReExam = true;

                if (Exam.IsFinalExam)
                    ReExam.IsFinalExam = true;

                if (!HasReExam) // Create new ReExam and link it
                {
                    _service.AddItem(ReExam);
                    Exam.ReExamId = ReExam.ExamId;
                }
                else // Update existing ReExam
                {
                    _service.UpdateItem(ReExam);
                }
            }
            else
            {
                // Unlink ReExam from Exam
                Exam.ReExamId = null;
            }

            foreach (var key in ModelState.Keys.Where(k => k.Equals("Exam.Class") || k.Equals("ReExam.Class") || k.Equals("ReExam.ExamName")))
            {
                ModelState[key]?.Errors.Clear();
                if (ModelState[key] != null)
                    ModelState[key].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }

            // validation checks--------------------------------------------------------------

            if (SelectedRoomId.HasValue)// check room availability for Exam--
            {
                OverlapResult result = _overlapsService.RoomHasOverlap(SelectedRoomId.Value, Exam.ExamStartDate, Exam.ExamEndDate, Exam.ExamId);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("SelectedRoomId", result.Message);
                    return Page();
                }
            }

            if (ReExam.ExamId > 0 && SelectedRoomId.HasValue) // check room availability for ReExam if creating
            {
                OverlapResult result = _overlapsService.RoomHasOverlap(SelectedRoomId.Value, ReExam.ExamStartDate, ReExam.ExamEndDate, ReExam.ExamId);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("SelectedRoomId", result.Message);
                    return Page();
                }
            }

            if (Exam.ClassId > 0) // check class availability for Exam
            {
                OverlapResult result = _overlapsService.ClassHasOverlap(Exam.ClassId, Exam.ExamStartDate, Exam.ExamEndDate, Exam.ExamId);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("Exam.ClassId", result.Message);
                    return Page();
                }
            }

            if (ReExam.ExamId > 0 && ReExam.ClassId > 0) // check class availability for ReExam if creating
            {
                OverlapResult result = _overlapsService.ClassHasOverlap(ReExam.ClassId, ReExam.ExamStartDate, ReExam.ExamEndDate, ReExam.ExamId);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("ReExam.ClassId", result.Message);
                    return Page();
                }
            }

            if (!SelectedTeacherIds.IsNullOrEmpty()) // check teacher availability for exam
            {
                OverlapResult result = _overlapsService.TeacherHasOverlap(SelectedTeacherIds.First(),
                    Exam.ExamStartDate, Exam.ExamEndDate, Exam.IsFinalExam, Exam.IsReExam, Exam.ExamId);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("SelectedTeacherIds", result.Message);
                    return Page();
                }
            }

            if (!ModelState.IsValid)
                return Page();

            // Update the exam
            _service.UpdateItem(Exam);

           _teachersToExamService.RemoveAllFromExam(Exam.ExamId);

            if (ExaminerTeacherId.HasValue)
            {
                _teachersToExamService.AddTeachersToExams(ExaminerTeacherId.Value, Exam.ExamId, "Examiner");
            }
            if(CensorTeacherId.HasValue)
            {
                if(ExaminerTeacherId.HasValue && ExaminerTeacherId.Value == CensorTeacherId.Value)
                {
                    ModelState.AddModelError("CensorTeacherId", "Censor cannot be the same as Examiner.");
                }
                _teachersToExamService.AddTeachersToExams(CensorTeacherId.Value, Exam.ExamId, "Censor");

            }

            // Update room assignment - USING THE SAME CLEAN APPROACH AS TEACHERS
            // First remove existing room assignments
            _roomsToExamService.RemoveAllRoomsFromExam(Exam.ExamId);//Added This line to remove all existing room assignments

            if (SelectedRoomId.HasValue)
            {
                var availableRooms = await _roomService.GetAllAsync();
                var roomExists = availableRooms.Any(r => r.RoomId == SelectedRoomId.Value);

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

            // Update student assignments
            _studentsToExamService.SyncStudentsForExamAndClass(Exam.ExamId, Exam.ClassId);

          
            // Update room assignment - USING THE SAME APPROACH AS CREATE_EXAM
            // First remove existing room assignment (you might need to implement this)
            // For now, we'll just add new one (this might create duplicates)
           
            return RedirectToPage("GetEksamner");
        }
    }
}