using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    public interface IRoomsToExams : ICRUDT<RoomsToExam>
    {
        void AddTeachersFromClassToExam(int roomId, int e);

      
    }
}
