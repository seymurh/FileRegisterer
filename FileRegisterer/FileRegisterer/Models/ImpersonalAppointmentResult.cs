using Newtonsoft.Json;
using System.Collections.Generic;

namespace FileRegisterer.Models
{
    public class ImpersonalAppointmentResult
    {
        [JsonProperty("value")]
        public List<ImpersonalAppointmentData> Value { get; set; }
    }

    public class ImpersonalAppointmentData : IDocument
    {
        [JsonProperty("document")]
        public string Document { get; set; }
    }
}