using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    public interface ITeachersToExam : ICRUD<TeachersToExam>
    {
        /// <summary>
        /// Returns the number of students added to the exam
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="examId"></param>
        /// <returns></returns>
        public void AddTeachersToExams(int classId, int examId);
        // Remove all students from exam (if class changes or exam deleted)
        public void RemoveAllFromExam(int examId);

        // Update students on an exam if Class changes
        public void SyncTeacherToExam(int examId, int newClassId);
    }
}
