using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    // Service contract for linking teachers and exams with async CRUD support
    public interface ITeachersToExam : ICRUDAsync<TeachersToExam>
    {
        // Remove all teacher assignments for a specific exam asynchronously
        Task RemoveAllFromExamAsync(int examId);
        // Add a teacher to an exam with optional role asynchronously
        Task AddTeachersToExamsAsync(int teacherId, int examId, string role = null);
    }
}
