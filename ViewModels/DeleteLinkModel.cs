namespace _2._Sem_Project_Eksamen_System.ViewModels
{
    public class DeleteLinkModel
    {
        // Target Razor Page, e.g. "./Delete" or "/Eksamner/Delete_Exam"
        public string Page { get; set; } = "./Delete";

        // EITHER set Id (will map to route key "id")
        public int? Id { get; set; }

        // OR provide custom route keys via dictionary (e.g., { "teacherId": 3 })
        public IDictionary<string, object>? Routes { get; set; }

        // Button label text (keep “Delete” like your Exams page)
        public string Label { get; set; } = "Delete";
    }
}
