using _2._Sem_Project_Eksamen_System.Models1;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    public interface ITeachersToExam : ICRUD<TeachersToExam>
    {
       
      //  public void AddTeachersToExams(int teacherId, int examId);
        
        public void RemoveAllFromExam(int examId);

        //////////////////////TESTING PURPOSES ONLY FOR ROLL//////////////////////
        // New method with role parameter
        public void AddTeachersToExams(int teacherId, int examId, string role = null);

    }
}
