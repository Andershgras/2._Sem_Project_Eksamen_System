using _2._Sem_Project_Eksamen_System.EFservices;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;
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

        private async Task PopulateExaminerSelectAsync(int id)
        {
            var teachers = (await _teacherService.GetAllAsync()).ToList();
            ExaminerSelect = teachers.Select(t => new GenericMultySelect
            {
                SelectValue = t.TeacherId,
                SelectText = t.TeacherName ?? string.Empty,
                IsSelected = SelectedTeacherIds != null && SelectedTeacherIds.Contains(t.TeacherId)
            }).ToList();
        }

        public async Task<IActionResult> OnGet(int id)
        {
            //Getting all data from classes , teachers and rooms for dropdown lists
            var allClasses = await _classService.GetAllAsync();
            var allTeachers = await _teacherService.GetAllAsync();
            var allRooms = await _roomService.GetAllAsync();

            //now creating select lists to populate dropdowns
            ClassList = new SelectList(allClasses, "ClassId", "ClassName");
            TeacherList = new SelectList(allTeachers, "TeacherId", "TeacherName");
            RoomList = new SelectList(allRooms, "RoomId", "Name");

            // Get currently assigned Examiner teachers for the exam
            var examinerTeachers = await _teachersToExamService.GetTeachersByExamIdAndRoleAsync(id, "Examiner");
            SelectedTeacherIds = examinerTeachers.Select(t => t.TeacherId).ToList();

            await PopulateExaminerSelectAsync(id); // Populate examiner select list


            // Load the selected exam with its related exam with related data
            Exam = await _service.GetItemByIdAsync(id);
            if (Exam == null)
                return RedirectToPage("GetEksamner");

            //Extract assigned teachers for roles
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


            // Preselect the first assigned room for editing (if any)
            if (Exam.RoomsToExams != null && Exam.RoomsToExams.Any())
            {
                SelectedRoomId = Exam.RoomsToExams.First().RoomId;
            }
            // Load ReExam if it exists
            if (HasReExam)
            {
                ReExam = await _service.GetItemByIdAsync(Exam.ReExamId.Value);
                EditReExam = true;
                //  used in the view to determine whether ReExam fields should be shown and editable
            }
            else
            {
                ReExam = new Exam();
            }

            return Page();
        }

        public async Task<IActionResult> OnPost(int id)
        {
            // Reload dropdown lists and populate withdata
            var allClasses = await _classService.GetAllAsync();
            var allTeachers = await _teacherService.GetAllAsync();
            var allRooms = await _roomService.GetAllAsync();
            

            ClassList = new SelectList(allClasses, "ClassId", "ClassName");
            TeacherList = new SelectList(allTeachers, "TeacherId", "TeacherName");
            RoomList = new SelectList(allRooms, "RoomId", "Name");

            await PopulateExaminerSelectAsync(id);
            

            //Clear validation for  all ReExam fields when not editing/creating a ReExam
            if (!EditReExam)
            {
                foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ReExam.")))
                    ModelState[key]?.Errors.Clear();
            }

            // Trim Exam name if too long
            if (Exam.ExamName.Length > 30)
                Exam.ExamName = Exam.ExamName.Substring(0, 30);

            // Validate Main Exam dates
            if (Exam.ExamStartDate > Exam.ExamEndDate)
                ModelState.AddModelError("Exam.ExamStartDate", "Exam start date must not be after end date.");
            if (Exam.DeliveryDate > Exam.ExamStartDate)
                ModelState.AddModelError("Exam.DeliveryDate", "Delivery date must not be after start date.");

            // while  editing or updating ReExam logic and validations
            if (EditReExam)
            {
               
                ReExam.ClassId = Exam.ClassId;

                //To set default classId and name for ReExam if not provided
                if (string.IsNullOrWhiteSpace(ReExam.ExamName))
                    ReExam.ExamName = $"ReEksamen-{Exam.ExamName}";
                // Trim ReExam name if too long
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

                //Derfore reexam persistance logic until all logic is validated
                if (!HasReExam)
                {
                    // If this is a new ReExam, we must set Exam.ReExamId to null for now. 
                    // We only link the exams after both are successfully created/updated in the DB.
                    Exam.ReExamId = null;
                }
            }
            else
            {
                // Unlink ReExam from Exam
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

            // Check Examiner availaiblity  to avoid overlap conflicts
            if (ExaminerTeacherId.HasValue && CensorTeacherId.HasValue && ExaminerTeacherId.Value == CensorTeacherId.Value)
            {
                ModelState.AddModelError("CensorTeacherId", "Censor cannot be the same as Examiner.");
            }

            // Check teacher availability for Exam
            if (ExaminerTeacherId.HasValue)
            {
                OverlapResult result = _overlapsService.TeacherHasOverlap(ExaminerTeacherId.Value,
                  Exam.ExamStartDate, Exam.ExamEndDate, Exam.IsFinalExam, Exam.IsReExam, Exam.ExamId);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("ExaminerTeacherId", "Examiner: " + result.Message);
                }
            }
            // Check Censor availability for Exam
            if (CensorTeacherId.HasValue)
            {
                OverlapResult result = _overlapsService.TeacherHasOverlap(CensorTeacherId.Value,
                  Exam.ExamStartDate, Exam.ExamEndDate, Exam.IsFinalExam, Exam.IsReExam, Exam.ExamId);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("CensorTeacherId", "Censor: " + result.Message);
                }
            }

            // Check teacher availability for ReExam (if creating/editing)
            if (EditReExam && ReExam.ExamId > 0)
            {
                if (ExaminerTeacherId.HasValue)
                {
                    OverlapResult result = _overlapsService.TeacherHasOverlap(ExaminerTeacherId.Value,
                      ReExam.ExamStartDate, ReExam.ExamEndDate, ReExam.IsFinalExam, ReExam.IsReExam, ReExam.ExamId);
                    if (result != null && result.HasConflict)
                    {
                        ModelState.AddModelError("ReExam.ExaminerTeacherId", "ReExam Examiner: " + result.Message);
                    }
                }
                // Check Censor availability for ReExam
                if (CensorTeacherId.HasValue)
                {
                    OverlapResult result = _overlapsService.TeacherHasOverlap(CensorTeacherId.Value,
                      ReExam.ExamStartDate, ReExam.ExamEndDate, ReExam.IsFinalExam, ReExam.IsReExam, ReExam.ExamId);
                    if (result != null && result.HasConflict)
                    {
                        ModelState.AddModelError("ReExam.CensorTeacherId", "ReExam Censor: " + result.Message);
                    }
                }
            }
            // Check room availability for Exam
            if (SelectedRoomId.HasValue)
            {
                OverlapResult result = _overlapsService.RoomHasOverlap(SelectedRoomId.Value, Exam.ExamStartDate, Exam.ExamEndDate, Exam.ExamId);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("SelectedRoomId", result.Message ?? "Something went wrong with Rooms");
                }
            }
            // Check room availability for ReExam

            if (EditReExam && ReExam.ExamId > 0 && SelectedRoomId.HasValue) // check room availability for ReExam if creating/editing
            {
                OverlapResult result = _overlapsService.RoomHasOverlap(SelectedRoomId.Value, ReExam.ExamStartDate, ReExam.ExamEndDate, ReExam.ExamId);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("SelectedRoomId", "ReExam Room: " + result.Message); // Added ReExam context
                }
            }
            // check class availability for Exam
            if (Exam.ClassId > 0) 
            {
                OverlapResult result = _overlapsService.ClassHasOverlap(Exam.ClassId, Exam.ExamStartDate, Exam.ExamEndDate, Exam.ExamId);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("Exam.ClassId", result.Message ?? "Something went wrong with Rooms");
                }
            }
            // check class availability for ReExam

            if (EditReExam && ReExam.ExamId > 0 && ReExam.ClassId > 0) // check class availability for ReExam if creating/editing
            {
                OverlapResult result = _overlapsService.ClassHasOverlap(ReExam.ClassId, ReExam.ExamStartDate, ReExam.ExamEndDate, ReExam.ExamId);
                if (result != null && result.HasConflict)
                {
                    ModelState.AddModelError("ReExam.ClassId", "ReExam Class: " + result.Message); // Added ReExam context
                }
            }

            // ------------------------------------------End validation checks--------------------------------------------------------------

            if (!ModelState.IsValid)
                return Page();


            // START PERSISTENCE LOGIC

            // Save or update ReExam
            if (EditReExam)
            {
                if (!HasReExam) // Create new ReExam and link it
                {
                    await _service.AddItemAsync(ReExam);
                    Exam.ReExamId = ReExam.ExamId; // Link ReExam to main Exam
                }
                else // Update existing ReExam
                {
                    await _service.UpdateItemAsync(ReExam);
                }
            }
            else if (HasReExam)
            {
                // ReExam exsists but editing is not desired , so delete it
                await _service.DeleteItemAsync(Exam.ReExamId.Value);
                Exam.ReExamId = null;
            }

            // Save changes to Main Exam
            await _service.UpdateItemAsync(Exam);
            // 1. Update Teachers for Main Exam
            await _teachersToExamService.RemoveAllFromExamAsync(Exam.ExamId);

            if (ExaminerTeacherId.HasValue)
            {
                await _teachersToExamService.AddTeachersToExamsAsync(ExaminerTeacherId.Value, Exam.ExamId, "Examiner");
            }
            if (CensorTeacherId.HasValue)
            {
                await _teachersToExamService.AddTeachersToExamsAsync(CensorTeacherId.Value, Exam.ExamId, "Censor");
            }

            // 2. Update Room assignment for Main Exam
            await _roomsToExamService.RemoveAllRoomsFromExamAsync(Exam.ExamId);

            if (SelectedRoomId.HasValue)
            {
                var mapping = new RoomsToExam
                {
                    ExamId = Exam.ExamId,
                    RoomId = SelectedRoomId.Value,
                    Role = null
                };
                await _roomsToExamService.AddItemAsync(mapping);
            }

            // 3. Update student assignments for Main Exam
            await _studentsToExamService.SyncStudentsForExamAndClassAsync(Exam.ExamId, Exam.ClassId);

            // 4. Update Assignments for ReExam
            if (EditReExam && ReExam.ExamId > 0)
            {
                // Teacher assignment for ReExam (inherits Examiner/Censor from main exam)
                await _teachersToExamService.RemoveAllFromExamAsync(ReExam.ExamId);
                if (ExaminerTeacherId.HasValue)
                {
                    await _teachersToExamService.AddTeachersToExamsAsync(ExaminerTeacherId.Value, ReExam.ExamId, "Examiner");
                }
                if (CensorTeacherId.HasValue)
                {
                    await _teachersToExamService.AddTeachersToExamsAsync(CensorTeacherId.Value, ReExam.ExamId, "Censor");
                }

                // Room assignment for ReExam (inherits room from main exam)
                await _roomsToExamService.RemoveAllRoomsFromExamAsync(ReExam.ExamId);
                if (SelectedRoomId.HasValue)
                {
                    var mapping = new RoomsToExam
                    {
                        ExamId = ReExam.ExamId,
                        RoomId = SelectedRoomId.Value,
                        Role = null
                    };
                    await _roomsToExamService.AddItemAsync(mapping);
                }

                // Student assignment for ReExam
                await _studentsToExamService.AddStudentsFromClassToExamAsync(ReExam.ExamId, ReExam.ClassId);
            }

            // Add a success message
            TempData["SuccessMessage"] = "Exam updated successfully!";
            return RedirectToPage("GetEksamner");
        }
    }
}