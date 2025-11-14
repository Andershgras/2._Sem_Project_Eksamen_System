using _2._Sem_Project_Eksamen_System.Interfaces;
using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace _2._Sem_Project_Eksamen_System.EFservices
{
    public class EFOverlapService : ICheckOverlap
    {
        private readonly EksamensDBContext _context;

        public EFOverlapService(EksamensDBContext context)
        {
            _context = context;
        }

        private bool DateRangesOverlap(DateOnly? aStart, DateOnly? aEnd, DateOnly? bStart, DateOnly? bEnd)
        {
            if (!aStart.HasValue || !aEnd.HasValue || !bStart.HasValue || !bEnd.HasValue)
                return false;
            return aStart.Value <= bEnd.Value && aEnd.Value >= bStart.Value;
        }

        //excludeExamId bruges til at undgå at sammenligne med sig selv ved opdatering
        public OverlapResult TeacherHasOverlap(int teacherId, DateOnly? newStart, DateOnly? newEnd, bool newIsFinal, bool newIsReExam, int? excludeExamId = null)
        {
            if (teacherId <= 0) return OverlapResult.Ok();
            if (!newStart.HasValue || !newEnd.HasValue) return OverlapResult.Ok();

            var assignments = _context.TeachersToExams
                .Include(t => t.Exam)
                .Include(t => t.Teacher)
                .Where(t => t.TeacherId == teacherId)
                .AsEnumerable()
                .Where(a => a.Exam != null)
                .ToList();

            var teacher = assignments.Select(a => a.Teacher).FirstOrDefault() ?? _context.Teachers.AsNoTracking().FirstOrDefault(t => t.TeacherId == teacherId);
            var teacherLabel = teacher != null ? $"Teacher {teacher.TeacherName ?? teacher.TeacherId.ToString()}" : $"Teacher (ID {teacherId})";

            foreach (var a in assignments)
            {
                var existing = a.Exam!;
                if (excludeExamId.HasValue && existing.ExamId == excludeExamId.Value) continue; // skips the current exam if excluding is requested

                // allow overlap if either existing OR new is final or reexam
                var existingAllowsOverlap = existing.IsFinalExam || existing.IsReExam;
                var newAllowsOverlap = newIsFinal || newIsReExam;

                if (!existingAllowsOverlap && !newAllowsOverlap)
                {
                    if (DateRangesOverlap(existing.ExamStartDate, existing.ExamEndDate, newStart, newEnd))
                    {
                        return CreateConflictResult(existing, teacherLabel, conflictingTeacherId: teacherId);
                    }
                }
            }
            return OverlapResult.Ok();
        }

        public OverlapResult ClassHasOverlap(int classId, DateOnly? newStart, DateOnly? newEnd)
        {
            if (classId <= 0) throw new AggregateException("Needs Id to be bigger then 0");

            if (!newStart.HasValue || !newEnd.HasValue) return OverlapResult.Ok();

            var exams = _context.Exams
                .Where(e => e.ClassId == classId)
                .ToList();

            foreach (var existing in exams)
            {
                if (DateRangesOverlap(existing.ExamStartDate, existing.ExamEndDate, newStart, newEnd))
                    return CreateConflictResult(existing, "Class");
            }
            return OverlapResult.Ok();
        }

        public OverlapResult RoomHasOverlap(int roomId, DateOnly? newStart, DateOnly? newEnd)
        {
            if (roomId <= 0) return OverlapResult.Ok();
            if (!newStart.HasValue || !newEnd.HasValue) return OverlapResult.Ok();

            var conflicts = _context.RoomsToExams
                .Include(rte => rte.Exam)
                .Where(rte => rte.RoomId == roomId && rte.Exam != null)
                .AsEnumerable() 
                .Where(rte =>
                {
                    return DateRangesOverlap(rte.Exam.ExamStartDate, rte.Exam.ExamEndDate, newStart, newEnd);
                })
                .ToList();

            if (!conflicts.Any())
                return OverlapResult.Ok();

            var existing = conflicts.First().Exam!;
            return CreateConflictResult(existing, $"Room", conflictingRoomId: roomId);
        }

        // Helper to create a conflict result message
        private OverlapResult CreateConflictResult(Exam existingExam, string contextLabel, int? conflictingRoomId = null, int? conflictingTeacherId = null)
        {
            if (existingExam == null)
                return OverlapResult.Ok();

            var start = existingExam.ExamStartDate.HasValue
                ? existingExam.ExamStartDate.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
                : "N/A";

            var end = existingExam.ExamEndDate.HasValue
                ? existingExam.ExamEndDate.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
                : "N/A";

            var name = string.IsNullOrWhiteSpace(existingExam.ExamName) ? $"Exam ID {existingExam.ExamId}" : existingExam.ExamName;

            // Construct the message
            // actorPart is either "The resource" or the contextLabel provided
            var actorPart = string.IsNullOrWhiteSpace(contextLabel) ? "The resource" : contextLabel;
            var msg = $"{actorPart} is allready asigned to an existing exam '{name}' ({start} —> {end}). Choose another {actorPart.ToLower()} or change the dates.";

            return new OverlapResult
            {
                HasConflict = true,
                Message = msg,
                ConflictingEntityType = "Exam",
                ConflictingEntityId = existingExam.ExamId,
                ConflictingExamName = existingExam.ExamName,
                ConflictingExamStart = existingExam.ExamStartDate,
                ConflictingExamEnd = existingExam.ExamEndDate,
                ConflictingRoomId = conflictingRoomId,
                ConflictingTeacherId = conflictingTeacherId
            };
        }
    }
}
