using System.Collections.Generic;
using Newtonsoft.Json;

namespace SkinPackCreator.Models
{
    public class ManifestModel
    {
        [JsonProperty("header")]
        public HeaderModel Header { get; set; }

        [JsonProperty("modules")]
        public IList<ModulesModel> Modules { get; set; }

        [JsonProperty("format_version")]
        public int FormatVersion { get; set; }
    }

    public class ModulesModel
    {
        [JsonProperty("version")]
        public int[] Version { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("uuid")]
        public string UUID { get; set; }
    }

    public class HeaderModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("version")]
        public int[] Version { get; set; }

        [JsonProperty("uuid")]
        public string UUID { get; set; }

    }
}
