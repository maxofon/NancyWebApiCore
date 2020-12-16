using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataAccess.Entities
{
    public class ResultWrapper
    {
        [JsonProperty("status")] public string Status { get; set; }

        [JsonProperty("section")] public string Section { get; set; }

        [JsonProperty("num_results")] public int NumResults { get; set; }

        [JsonProperty("results")] public ICollection<Article> Articles { get; set; }
    }
}