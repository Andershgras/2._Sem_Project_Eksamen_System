using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    public interface IStudentsToClasses : ICRUDAsync<StudentsToClass>
    {
        /// <summary>
        /// Retrieves a collection of students enrolled in the specified class.
        /// </summary>
        /// <param name="classId">The unique identifier of the class whose students are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an  IEnumerable{T} of Student
        /// objects representing the students in the class. If no students are found, the collection will be empty.</returns>
        Task<IEnumerable<Student>> GetStudentsFromClass(int classId);
    }
}
