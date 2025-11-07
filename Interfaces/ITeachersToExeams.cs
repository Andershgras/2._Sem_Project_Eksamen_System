using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    public interface ITeachersToExam : ICRUD<TeachersToExam>
    {
        int AddTeachersFromClassToExam(int classId, int examId);
    }
}
