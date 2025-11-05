using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Pages.Students
{
    public class CreateStudentModel : PageModel
    {
        private readonly ICRUDT<Student> _studentService;
        private readonly ICRUDT<Class> _classService;

        [BindProperty]
        public Student Student { get; set; } = new Student();

        [BindProperty]
        public int? SelectedClassId { get; set; }

        public List<SelectListItem> ClassList { get; set; } = new List<SelectListItem>();

        public CreateStudentModel(ICRUDT<Student> studentService, ICRUDT<Class> classService)
        {
            _studentService = studentService;
            _classService = classService;
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
                // You'll need to implement this part based on your StudentsToClass relationship
                // This might require an additional service for StudentsToClass
                await CreateStudentClassRelationship(Student.StudentId, SelectedClassId.Value);
            }

            return RedirectToPage("/Students/GetStudent");
        }

        private async Task PopulateClassDropdown()
        {
            var classes = await _classService.GetAll(new GenericFilter());

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
            // You'll need to implement this method based on your data model
            // This would typically use a service for StudentsToClass
            // Example:
            // var studentClass = new StudentsToClass { StudentId = studentId, ClassId = classId };
            // await _studentClassService.AddItem(studentClass);
        }
    }
}