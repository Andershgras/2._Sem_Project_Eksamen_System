using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Pages.Students
{
    public class CreateStudentModel : PageModel
    {
        // Services for CRUD operations on Student,class  entities(used to populate dropdown)
        private readonly ICRUDAsync<Student> _studentService;
        private readonly ICRUDAsync<Class> _classService;
        // Direct DbContext for small specific operations espacially relatioship checks
        private readonly EksamensDBContext _context;

        //Bind student model for the form
        [BindProperty]
        public Student Student { get; set; } = new Student();

        //Bound selected class id from the dropdown (nullable)

        [BindProperty]
        public int? SelectedClassId { get; set; }
        //Dropdown for class selection
        public List<SelectListItem> ClassList { get; set; } = new List<SelectListItem>();
        //       
        public CreateStudentModel(
            ICRUDAsync<Student> studentService,
            ICRUDAsync<Class> classService,
            EksamensDBContext context)
        {
            _studentService = studentService;
            _classService = classService;
            _context = context;
        }
        //Get Handler to populate dropdown
        public async Task OnGetAsync()
        {
            await PopulateClassDropdown();
        }
        //Post handler to validate,check duplicates and create students and realatioships
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateClassDropdown();
                return Page();
            }

            // Very important part checking for duplicates before creating new student
            // Prevent duplicate student by name or email

            if (await StudentAlreadyExists(Student))
            {
                ModelState.AddModelError(string.Empty, "A student with the same name or email already exists.");
                await PopulateClassDropdown();
                return Page();
            }
            //Create Students Record
            // wait First, create the student
            await _studentService.AddItemAsync(Student);

            // If a class was selected, create the relationship
            if (SelectedClassId.HasValue && SelectedClassId.Value > 0)
            {
                //here this SelectedClassId.Value is the ID we need
                await CreateStudentClassRelationship(Student.StudentId, SelectedClassId.Value);
            }

            return RedirectToPage("/Students/GetStudent");
        }
        // Check for existing student by name or email (case-insensitive)
        // here i added duplicate check
        private async Task<bool> StudentAlreadyExists(Student newStudent)
        {
            // Check if student with same name OR email already exists in the database
                    var existingStudent = await _context.Students
                    .FirstOrDefaultAsync(s =>
                    s.StudentName.ToLower() == newStudent.StudentName.ToLower() ||
                    (!string.IsNullOrEmpty(newStudent.Email) &&
                     s.Email.ToLower() == newStudent.Email.ToLower()));

            return existingStudent != null;
        }
        // Fill ClassList used by the class dropdown
        private async Task PopulateClassDropdown()///I might modify this method later
        {
            // it is used to populate the ClassList for the dropdown
            var classes = await  _classService.GetAllAsync(new GenericFilter());

            ClassList.Clear();
            ClassList.Add(new SelectListItem
            {
                Value = "",
                Text = "Vælg en klasse",
                Selected = true
            });

            if (classes != null)
            {
                foreach (var classItem in classes)
                {
                    ClassList.Add(new SelectListItem
                    {
                        Value = classItem.ClassId.ToString(),
                        Text = classItem.ClassName
                    });
                }
            }
        }
        // Create link record between student and class then save
        private async Task CreateStudentClassRelationship(int studentId, int classId)
        {
            //this methods creates a student class relationship
            var studentClass = new StudentsToClass
            {
                StudentId = studentId,
                ClassId = classId
            };

            _context.StudentsToClasses.Add(studentClass);
            await _context.SaveChangesAsync();
        }
    }
}