using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FileRegisterer.Models
{
    public class CasemanagementofIllnessResult
    {
        public string Personalcode { get; set; }
        public DateTime DossierStartDate { get; set; }
        public List<AppointmentIllnessData> AppointmentList { get; set; }
        public List<ActionsIllnessData> ActionsList { get; set; }
        public List<CommunicationIllnessData> Communications { get; set; }
        public List<ReportsspecialistsIllnessData> ReportsspecialistsList { get; set; }
        public List<ReportscompanyPhysiciansIllnessData> ReportscompanyPhysiciansList { get; set; }
    }

    public class AppointmentIllnessData : IDocument
    {
        [JsonProperty("document")]
        public string Document { get; set; }
    }

    public class ActionsIllnessData : IDocument
    {
        [JsonProperty("document")]
        public string Document { get; set; }
    }

    public class CommunicationIllnessData : IDocument
    {
        [JsonProperty("document")]
        public string Document { get; set; }
    }

    public class ReportsspecialistsIllnessData : IDocument
    {
        [JsonProperty("document")]
        public string Document { get; set; }
    }

    public class ReportscompanyPhysiciansIllnessData : IDocument
    {
        [JsonProperty("document")]
        public string Document { get; set; }
    }
}