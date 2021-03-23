using System.Collections.Generic;

namespace FileRegisterer.Models
{
    public class CasemanagementWIAResult
    {
        public List<TasksModel> Tasks { get; set; } = new List<TasksModel>();
        public List<CommunicationModel> Communication { get; set; }
        public List<ReportsSpecialistModel> ReportsSpecialists { get; set; }
    }

    public class TasksModel : IDocument
    {
        public string Document { get; set; }
    }

    public class CommunicationModel : IDocument
    {
        public string Document { get; set; }
    }

    public class ReportsSpecialistModel : IDocument
    {
        public string Document { get; set; }
    }
}