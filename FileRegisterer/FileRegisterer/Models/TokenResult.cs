using Newtonsoft.Json;

namespace FileRegisterer.Models
{
    public class TokenResult
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}