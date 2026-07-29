using Newtonsoft.Json;

using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class Classification
    {
        [PrimaryKey]
        [JsonProperty("accountclassificationid")]
        public int AccountClassificationId { get; set; }

        [JsonProperty("accountclassificationname")]
        public string AccountClassificationName { get; set; }

        [JsonProperty("customertype")]
        public int CustomerType { get; set; }

        /// <summary>
        /// Optional hex color prescribed by the server (e.g. "#FF5733").
        /// When null or empty, <see cref="ClassificationColorService"/> auto-assigns
        /// a perceptually distinct dark color using the golden-angle HSL algorithm.
        /// </summary>
        [JsonProperty("colorhex")]
        public string ColorHex { get; set; }

        /// <summary>
        /// Optional static map-pin image filename prescribed by the server
        /// (e.g. "MapPin-Red.png"). When null or empty, a PNG is generated at
        /// runtime by <see cref="MapPinGenerator"/> and cached locally.
        /// </summary>
        [JsonProperty("mappinimagename")]
        public string MapPinImageName { get; set; }
    }
}