using System.Collections.Generic;
using System.Threading.Tasks;
using _2._Sem_Project_Eksamen_System.Models1;
using _2._Sem_Project_Eksamen_System.Utils;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    public interface IExamService : ICRUDAsync<Exam>
    {
        Task<IEnumerable<Exam>> GetAllAsync(ExtendedExamFilter filter);
    }
}
