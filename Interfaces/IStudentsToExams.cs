using _2._Sem_Project_Eksamen_System.Models1;
using Microsoft.EntityFrameworkCore;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    public interface IStudentsToExams : ICRUD<StudentsToExam>
    {

        #region Custom Methods for StudentsToExam
        public void AddStudentsFromClassToExam(int classId, int examId);
        // Remove all students from exam (if class changes or exam deleted)
        public void RemoveAllFromExam(int examId);

        // Update students on an exam if Class changes
        public void SyncStudentsForExamAndClass(int examId, int newClassId);
        
        #endregion
    }
}
