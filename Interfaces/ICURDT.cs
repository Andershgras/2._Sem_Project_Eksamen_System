using System.Collections.Generic;
using System.Threading.Tasks;
using _2._Sem_Project_Eksamen_System.Utils;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    //<T> where T : class
    // overstående betydder at T skal være en reference type (class) og ikke en værdi type (struct).
    //(where T: <Type constraint>)
    public interface ICRUDT<T> where T : class
    {

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returnere alle items af typen T</returns>
        // NOTE: methods converted to Task-based async signatures
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAll(GenericFilter Filter);
        /// <summary>
        /// Tilføjer et item af typen T, til en User specificeret corlection.
        /// </summary>
        /// <param name="item"></param>
        Task AddItem(T item);

        /// <summary>
        /// Henter et item af typen T ved brug af et ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T?> GetItemById(int id);
        /// <summary>
        /// Sletter et item af typen T.
        /// </summary>
        /// <remarks>Brug GetItemById method til at finde det specifikke object</remarks>
        Task DeleteItem(int id);
        /// <summary>
        /// Opdaterer et item af typen T.
        /// </summary>
        /// <param name="item"></param>
        Task UpdateItem(T item);
        #endregion
    }
}