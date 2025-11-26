using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    // Service contract for linking teachers and exams with async CRUD support
    public interface ITeachersToExam : ICRUDAsync<TeachersToExam>
    {
        // Remove all teacher assignments for a specific exam asynchronously
        Task RemoveAllFromExamAsync(int examId);
        // Add a teacher to an exam with optional role asynchronously
        Task AddTeachersToExamsAsync(int teacherId, int examId, string? role = null);

        /// <summary>
        /// Asynchronously retrieves a collection of teachers associated with a specific exam and role.
        /// </summary>
        /// <param name="examId">The unique identifier of the exam for which teachers are being retrieved.</param>
        /// <param name="role">The role of the teachers to filter by. This parameter is case-sensitive.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see
        /// cref="IEnumerable{Teacher}"/> of teachers matching the specified exam ID and role. If no teachers are found,
        /// the result is an empty collection.</returns>
        Task<IEnumerable<Teacher>> GetTeachersByExamIdAndRoleAsync(int examId, string role);

    }
}
