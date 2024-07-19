using Newtonsoft.Json;
namespace HackerNewsApp.BusinessEntities
{
    public class HackerNewsStoryModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
