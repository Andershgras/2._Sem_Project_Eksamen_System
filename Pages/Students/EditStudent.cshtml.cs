using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.EntityFrameworkCore;
using _2._Sem_Project_Eksamen_System.Utils;

namespace _2._Sem_Project_Eksamen_System.Pages.Students
{
    public class EditStudentModel : PageModel
    {
        private readonly ICRUDAsync<Student> _studentService;
        private readonly ICRUDAsync<Class> _classService;
        private readonly EksamensDBContext _context;

        [BindProperty]
        public Student Student { get; set; } = new Student();

        [BindProperty]
        public int? SelectedClassId { get; set; }

        public List<SelectListItem> ClassList { get; set; } = new List<SelectListItem>();

        public EditStudentModel(
            ICRUDAsync<Student> studentService,
            ICRUDAsync<Class> classService,
            EksamensDBContext context)
        {
            _studentService = studentService;
            _classService = classService;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        // id is the StudentId
        {
            var student = await _studentService.GetItemByIdAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            Student = student;

            // Get current class assignment if exists
            var currentClass = await _context.StudentsToClasses
                .FirstOrDefaultAsync(sc => sc.StudentId == id);

            if (currentClass != null)
            {
                SelectedClassId = currentClass.ClassId;
            }

            await PopulateClassDropdown();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateClassDropdown();
                return Page();
            }

            // Update the student
            await _studentService.UpdateItemAsync(Student);

            // Update class relationship
            await UpdateStudentClassRelationship(Student.StudentId, SelectedClassId);

            return RedirectToPage("/Students/GetStudent");
        }

        private async Task PopulateClassDropdown()
        {
            var classes = await Task.Run(() => _classService.GetAllAsync(new GenericFilter()));//Method is async now

            ClassList.Clear();
            ClassList.Add(new SelectListItem
            {
                Value = "",
                Text = "Vælg en klasse"
            });

            if (classes != null)
            {
                foreach (var classItem in classes)
                {
                    ClassList.Add(new SelectListItem
                    {
                        Value = classItem.ClassId.ToString(),
                        Text = classItem.ClassName,
                        Selected = SelectedClassId == classItem.ClassId
                    });
                }
            }
        }

        private async Task UpdateStudentClassRelationship(int studentId, int? classId)
        {
            // Remove existing class relationships
            var existingRelations = _context.StudentsToClasses
                .Where(sc => sc.StudentId == studentId);

            _context.StudentsToClasses.RemoveRange(existingRelations);

            // Add new relationship if a class is selected
            if (classId.HasValue && classId > 0)
            {
                var studentClass = new StudentsToClass
                {
                    StudentId = studentId,
                    ClassId = classId.Value
                };

                _context.StudentsToClasses.Add(studentClass);
            }

            await _context.SaveChangesAsync();
        }
    }
}