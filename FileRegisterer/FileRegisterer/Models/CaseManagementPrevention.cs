using Newtonsoft.Json;
using System.Collections.Generic;

namespace FileRegisterer.Models
{
    public class CasemanagementPreventionResult
    {
        public string Personalcode { get; set; }
        public List<Appointment> AppointmentList { get; set; } = new List<Appointment>();
        public List<Actions> ActionsList { get; set; } = new List<Actions>();
        public List<Communication> Communications { get; set; } = new List<Communication>();
        public List<Reportsspecialist> ReportsSpecialistsList { get; set; }
        public List<ReportscompanyPhysician> ReportsCompanyPhysiciansList { get; set; }

    }

    public class Appointment : IDocument
    {
        [JsonProperty("document")]
        public string Document { get; set; }
    }

    public class Actions : IDocument
    {
        [JsonProperty("document")]
        public string Document { get; set; }
    }

    public class Communication : IDocument
    {
        [JsonProperty("document")]
        public string Document { get; set; }
    }

    public class Reportsspecialist : IDocument
    {
        [JsonProperty("document")]
        public string Document { get; set; }
    }

    public class ReportscompanyPhysician : IDocument
    {
        [JsonProperty("document")]
        public string Document { get; set; }
    }
}