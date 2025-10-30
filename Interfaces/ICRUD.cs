namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    //<T> where T : class
    // overstående betydder at T skal være en reference type (class) og ikke en værdi type (struct).
    //(where T: <Type constraint>)
    public interface ICRUD<T> where T : class
    {
        #region Properties
        IEnumerable<T> Items { get; set; }

        #endregion
        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returnere alle items af typen T</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Tilføjer et item af typen T, til en User specificeret corlection.
        /// </summary>
        /// <param name="item"></param>
        void AddItem(T item);

        /// <summary>
        /// Henter et item af typen T ved brug af et ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? GetItemById(int id);
        void DeleteItem(T item);
        void UpdateItem(T item);
        #endregion
    }
}
