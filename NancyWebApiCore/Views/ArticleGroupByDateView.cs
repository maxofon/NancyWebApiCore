using Newtonsoft.Json;

namespace NancyWebApiCore.Views
{
    public class ArticleGroupByDateView
    {
        [JsonProperty("date")] public string Date { get; set; }

        [JsonProperty("total")] public int Total { get; set; }
    }
}