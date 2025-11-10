using System.Collections.Generic;
using System.Threading.Tasks;
using _2._Sem_Project_Eksamen_System.Utils;

namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    //<T> where T : class
    // overstående betydder at T skal være en reference type (class) og ikke en værdi type (struct).
    //(where T: <Type constraint>)
    public interface ICRUDAsync<T> where T : class
    {
        #region Async Methods
        /// <summary>
        /// Asynchronously retrieves all items of type T
        /// </summary>
        /// <returns>Collection of all items</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Asynchronously retrieves all items of type T with filtering
        /// </summary>
        /// <param name="filter">Filter criteria</param>
        /// <returns>Filtered collection of items</returns>
        Task<IEnumerable<T>> GetAllAsync(GenericFilter filter);

        /// <summary>
        /// Asynchronously retrieves an item by its ID
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <returns>The item if found, null otherwise</returns>
        Task<T?> GetItemByIdAsync(int id);

        /// <summary>
        /// Asynchronously adds a new item
        /// </summary>
        /// <param name="item">Item to add</param>
        Task AddItemAsync(T item);

        /// <summary>
        /// Asynchronously updates an existing item
        /// </summary>
        /// <param name="item">Item with updated values</param>
        Task UpdateItemAsync(T item);

        /// <summary>
        /// Asynchronously deletes an item by ID
        /// </summary>
        /// <param name="id">ID of item to delete</param>
        Task DeleteItemAsync(int id);
        #endregion
    }
}