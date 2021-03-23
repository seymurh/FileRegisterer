using Newtonsoft.Json;
using System.Collections.Generic;

namespace FileRegisterer.Models
{
    public class MaternityLeaveResult
    {
        [JsonProperty("value")]
        public List<MaternityLeaveTask> Value { get; set; }
    }

    public class MaternityLeaveTask : IDocument
    {
        public string Document { get; set; }
    }
}