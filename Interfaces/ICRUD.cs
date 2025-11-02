namespace _2._Sem_Project_Eksamen_System.Interfaces
{
    //<T> where T : class
    // overstående betydder at T skal være en reference type (class) og ikke en værdi type (struct).
    //(where T: <Type constraint>)
    public interface ICRUD<T> where T : class
    {

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returnere alle items af typen T</returns>
        public IEnumerable<T> GetAll();

        /// <summary>
        /// Tilføjer et item af typen T, til en User specificeret corlection.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(T item);

        /// <summary>
        /// Henter et item af typen T ved brug af et ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T? GetItemById(int id);
        /// <summary>
        /// Sletter et item af typen T.
        /// </summary>
        /// <remarks>Brug GetItemById method til at finde det specifikke object</remarks>
        public void DeleteItem(int id);
        /// <summary>
        /// Opdaterer et item af typen T.
        /// </summary>
        /// <param name="item"></param>
        public void UpdateItem(T item);
        #endregion
    }
}
