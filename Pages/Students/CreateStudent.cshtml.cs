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
        private readonly ICRUDT<Student> _studentService;
        private readonly ICRUD<Class> _classService;
        private readonly EksamensDBContext _context; // Add this

        [BindProperty]
        public Student Student { get; set; } = new Student();

        [BindProperty]
        public int? SelectedClassId { get; set; }

        public List<SelectListItem> ClassList { get; set; } = new List<SelectListItem>();

        public CreateStudentModel(
            ICRUDT<Student> studentService,
            ICRUD<Class> classService,
            EksamensDBContext context) // Add this parameter
        {
            _studentService = studentService;
            _classService = classService;
            _context = context; // Add this
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

            // First, create the student
            await _studentService.AddItem(Student);

            // If a class was selected, create the relationship
            if (SelectedClassId.HasValue && SelectedClassId > 0)
            {
                await CreateStudentClassRelationship(Student.StudentId, SelectedClassId.Value);
            }

            return RedirectToPage("/Students/GetStudent");
        }

        private async Task PopulateClassDropdown()
        {
            // FETCH CLASSES FOR DROPDOWN
            var classes = await Task.Run(() => _classService.GetAll(new GenericFilter()));

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
            // ACTUALLY CREATE THE RELATIONSHIP
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