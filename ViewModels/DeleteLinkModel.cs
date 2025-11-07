namespace _2._Sem_Project_Eksamen_System.ViewModels
{
    public class DeleteLinkModel
    {
        public string Page { get; set; } = "./Delete";

        public int? Id { get; set; }

        public IDictionary<string, object>? Routes { get; set; }

        public string Label { get; set; } = "Delete";
    }
}
