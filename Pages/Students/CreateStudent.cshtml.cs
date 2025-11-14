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
        private readonly ICRUDAsync<Student> _studentService;
        private readonly ICRUDAsync<Class> _classService;
        private readonly EksamensDBContext _context;

        [BindProperty]
        public Student Student { get; set; } = new Student();

        [BindProperty]
        public int? SelectedClassId { get; set; }

        public List<SelectListItem> ClassList { get; set; } = new List<SelectListItem>();

        public CreateStudentModel(
            ICRUDAsync<Student> studentService,
            ICRUDAsync<Class> classService,
            EksamensDBContext context)
        {
            _studentService = studentService;
            _classService = classService;
            _context = context;
        }

        public async Task OnGetAsync()
        {
            await PopulateClassDropdown();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateClassDropdown();
                return Page();
            }

            // Very important part checking for duplicates before creating new student

            if (await StudentAlreadyExists(Student))
            {
                ModelState.AddModelError(string.Empty, "A student with the same name or email already exists.");
                await PopulateClassDropdown();
                return Page();
            }

            // wait First, create the student
            await _studentService.AddItemAsync(Student);

            // If a class was selected, create the relationship
            if (SelectedClassId.HasValue && SelectedClassId > 0)
            {
                await CreateStudentClassRelationship(Student.StudentId, SelectedClassId.Value);
            }

            return RedirectToPage("/Students/GetStudent");
        }

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

        private async Task PopulateClassDropdown()
        {
            // it is used to populate the ClassList for the dropdown
            var classes = await Task.Run(() => _classService.GetAllAsync(new GenericFilter()));

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