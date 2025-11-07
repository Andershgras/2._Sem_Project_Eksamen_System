using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    public interface IRoomsToExams : ICRUD<RoomsToExam>
    {
        /// <summary>
        /// Add multiple room assignments to an exam.
        /// Each provided RoomsToExam should have RoomId, Role set. ExamId may be set by the caller or inside the implementation.
        /// </summary>
        /// <param name="examId">Target exam id</param>
        /// <param name="assignments">Collection of RoomsToExam assignments (RoomId + Role)</param>
        void AddRoomsToExam(int examId, IEnumerable<RoomsToExam> assignments);
    }    
}
