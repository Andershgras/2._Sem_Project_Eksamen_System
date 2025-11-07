namespace _2._Sem_Project_Eksamen_System.ViewModels
{
    public class PageHeaderModel
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        
        public PageHeaderModel(string title, string? description = null)
        {
            Title = title;
            Description = description;
        }
    }
}
