using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    public interface IStudentsToExams : ICRUDAsync<StudentsToExam>
    {

        #region Custom Methods for StudentsToExam

        /// <summary>
        /// Returns the number of students added to the exam
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="examId"></param>
        /// <returns></returns>
        Task AddStudentsFromClassToExamAsync(int classId, int examId);
        // Remove all students from exam (if class changes or exam deleted)
        Task RemoveAllFromExamAsync(int examId);

        // Update students on an exam if Class changes
        Task SyncStudentsForExamAndClassAsync(int examId, int newClassId);
        // Add students to an existing exam
        Task AddStudentsToExamAsync(IEnumerable<int> studIds, int examId);
        #endregion
    }
}
