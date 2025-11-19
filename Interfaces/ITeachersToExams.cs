using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    public interface ITeachersToExam : ICRUDAsync<TeachersToExam>
    {
        Task RemoveAllFromExamAsync(int examId);
        Task AddTeachersToExamsAsync(int teacherId, int examId, string role = null);
    }
}
