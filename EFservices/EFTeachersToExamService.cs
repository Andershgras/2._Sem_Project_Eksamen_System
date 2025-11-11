using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using _2._Sem_Project_Eksamen_System.Utils;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFTeachersToExamService : ITeachersToExam
    {
        private readonly EksamensDBContext _context;

        public EFTeachersToExamService(EksamensDBContext context)
        {
            _context = context;
        }

        public IEnumerable<TeachersToExam> GetAll()
        {
            // Returns all teacher-to-exam assignments, with teacher and exam 
            return _context.TeachersToExams
                .Include(te => te.Teacher)
                .Include(te => te.Exam)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<TeachersToExam> GetAll(GenericFilter filter)
        {
            // Example: filter by role or teacher or exam name if needed; left basic here
            return _context.TeachersToExams // needs implementation in GernericFilter to make this work properly
                .Where(te => te.Role != null && te.Role.ToLower().Contains(filter.FilterByName.ToLower()))
                .AsNoTracking()
                .ToList();
        }

        public TeachersToExam? GetItemById(int id)
        {
            return _context.TeachersToExams
                .Include(te => te.Teacher)
                .Include(te => te.Exam)
                .FirstOrDefault(te => te.TeacherExamId == id);
        }

        public void AddItem(TeachersToExam item)
        {
            _context.TeachersToExams.Add(item);
            _context.SaveChanges();
        }

        public void UpdateItem(TeachersToExam item)
        {
            var existing = _context.TeachersToExams.Find(item.TeacherExamId);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(item);
                _context.SaveChanges();
            }
        }

        public void DeleteItem(int id)
        {
            var toDelete = _context.TeachersToExams.Find(id);
            if (toDelete != null)
            {
                _context.TeachersToExams.Remove(toDelete);
                _context.SaveChanges();
            }
        }
        /// <summary>
        /// Add all teachers that belong to the given class to the specified exam.
        /// This implements the interface method AddTeachersToExams(int classId, int examId).
        /// Behavior:
        ///  - Prefers Teacher.ClassId if present in the EF model (efficient DB query).
        ///  - Otherwise searches for a join entity with ClassId & TeacherId (fallback).
        /// Existing mappings are skipped (no duplicates).
        /// </summary>
        public void AddTeachersToExams(int classId, int examId)
        {
            if (classId <= 0) throw new ArgumentException("classId must be > 0", nameof(classId));
            if (examId <= 0) throw new ArgumentException("examId must be > 0", nameof(examId));

            // already assigned teacher ids for this exam -> skip duplicates
            var existingTeacherIds = _context.TeachersToExams
                .Where(t => t.ExamId == examId)
                .Select(t => t.TeacherId)
                .ToHashSet();

            var teacherIdsInClass = new List<int>();

            // Preferred: Teacher entity has ClassId property (direct FK)
            var teacherEntity = _context.Model.FindEntityType(typeof(Teacher));
            if (teacherEntity != null && teacherEntity.FindProperty("ClassId") != null)
            {
                teacherIdsInClass = _context.Teachers
                    .AsNoTracking()
                    .Where(t => EF.Property<int?>(t, "ClassId") == classId)
                    .Select(t => t.TeacherId)
                    .Distinct()
                    .ToList();
            }
            else
            {
                // Fallback: find a join entity with ClassId & TeacherId (e.g., ClassTeachers)
                var joinEntity = _context.Model.GetEntityTypes()
                    .FirstOrDefault(et =>
                        et.ClrType != typeof(Teacher) &&
                        et.ClrType != typeof(TeachersToExam) &&
                        et.FindProperty("ClassId") != null &&
                        et.FindProperty("TeacherId") != null);

                if (joinEntity != null)
                {
                    try
                    {
                        var clr = joinEntity.ClrType;
                        var set = _context.Set(clr).AsNoTracking().AsQueryable();

                        // Materialize and reflect -- safe fallback when no direct mapping exists.
                        var rows = set.ToList();

                        var classProp = clr.GetProperty("ClassId");
                        var teacherProp = clr.GetProperty("TeacherId");

                        if (classProp != null && teacherProp != null)
                        {
                            foreach (var row in rows)
                            {
                                var cVal = classProp.GetValue(row);
                                if (cVal == null) continue;
                                if (Convert.ToInt32(cVal) != classId) continue;

                                var tVal = teacherProp.GetValue(row);
                                if (tVal == null) continue;

                                teacherIdsInClass.Add(Convert.ToInt32(tVal));
                            }
                        }
                    }
                    catch
                    {
                        // swallow fallback errors, treat as no teachers found
                        teacherIdsInClass = new List<int>();
                    }
                }
            }

            if (!teacherIdsInClass.Any()) return;

            var toAdd = teacherIdsInClass
                .Distinct()
                .Where(id => !existingTeacherIds.Contains(id))
                .Select(id => new TeachersToExam
                {
                    ExamId = examId,
                    TeacherId = id,
                    Role = null
                })
                .ToList();

            if (!toAdd.Any()) return;

            _context.TeachersToExams.AddRange(toAdd);
            _context.SaveChanges();
        }

        /// <summary>
        /// Remove all teacher mappings for the given exam.
        /// Implements RemoveAllFromExam(int examId).
        /// </summary>
        public void RemoveAllFromExam(int examId)
        {
            if (examId <= 0) throw new ArgumentException("examId must be > 0", nameof(examId));

            var existing = _context.TeachersToExams.Where(t => t.ExamId == examId).ToList();
            if (!existing.Any()) return;

            _context.TeachersToExams.RemoveRange(existing);
            _context.SaveChanges();
        }

        /// <summary>
        /// Sync teacher mappings for an exam to those of a new class.
        /// Implements SyncTeacherToExam(int examId, int newClassId).
        /// Behavior:
        ///  - Removes all existing teachers for the exam.
        ///  - Adds teachers from the new class (using the same discovery logic as AddTeachersToExams).
        /// </summary>
        public void SyncTeacherToExam(int examId, int newClassId)
        {
            if (examId <= 0) throw new ArgumentException("examId must be > 0", nameof(examId));
            if (newClassId <= 0) throw new ArgumentException("newClassId must be > 0", nameof(newClassId));

            // Remove existing mappings
            var existing = _context.TeachersToExams.Where(t => t.ExamId == examId).ToList();
            if (existing.Any())
                _context.TeachersToExams.RemoveRange(existing);

            // Discover teachers for newClassId using same logic as AddTeachersToExams
            var teacherIdsInClass = new List<int>();
            var teacherEntity = _context.Model.FindEntityType(typeof(Teacher));
            if (teacherEntity != null && teacherEntity.FindProperty("ClassId") != null)
            {
                teacherIdsInClass = _context.Teachers
                    .AsNoTracking()
                    .Where(t => EF.Property<int?>(t, "ClassId") == newClassId)
                    .Select(t => t.TeacherId)
                    .Distinct()
                    .ToList();
            }
            else
            {
                var joinEntity = _context.Model.GetEntityTypes()
                    .FirstOrDefault(et =>
                        et.ClrType != typeof(Teacher) &&
                        et.ClrType != typeof(TeachersToExam) &&
                        et.FindProperty("ClassId") != null &&
                        et.FindProperty("TeacherId") != null);

                if (joinEntity != null)
                {
                    try
                    {
                        var clr = joinEntity.ClrType;
                        var set = _context.Set(clr).AsNoTracking().AsQueryable();
                        var rows = set.ToList();

                        var classProp = clr.GetProperty("ClassId");
                        var teacherProp = clr.GetProperty("TeacherId");

                        if (classProp != null && teacherProp != null)
                        {
                            foreach (var row in rows)
                            {
                                var cVal = classProp.GetValue(row);
                                if (cVal == null) continue;
                                if (Convert.ToInt32(cVal) != newClassId) continue;

                                var tVal = teacherProp.GetValue(row);
                                if (tVal == null) continue;

                                teacherIdsInClass.Add(Convert.ToInt32(tVal));
                            }
                        }
                    }
                    catch
                    {
                        teacherIdsInClass = new List<int>();
                    }
                }
            }

            // Add new mappings
            var toAdd = teacherIdsInClass
                .Distinct()
                .Select(id => new TeachersToExam
                {
                    ExamId = examId,
                    TeacherId = id,
                    Role = null
                })
                .ToList();

            if (toAdd.Any())
                _context.TeachersToExams.AddRange(toAdd);

            _context.SaveChanges();
        }
    }

}
}