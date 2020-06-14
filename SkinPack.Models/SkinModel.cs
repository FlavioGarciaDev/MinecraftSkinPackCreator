using Newtonsoft.Json;
using System.Collections.Generic;

namespace SkinPackCreator.Models
{
    public class SkinModel
    {
        [JsonProperty("skins")]
        public IList<Skin> SkinList{ get; set; }

        [JsonProperty("serialize_name")]
        public string PackSerializeName { get; set; }

        [JsonProperty("localization_name")]
        public string PackLocName { get; set; }

        public class Skin
        {
            [JsonProperty("localization_name")]
            public string SkinName { get; set; }

            [JsonProperty("geometry")]
            public string SkinFormat { get; set; }

            [JsonProperty("texture")]
            public string SkinTextureFile { get; set; }

            [JsonProperty("type")]
            public string SkinType { get; set; }
        }
    }
}
