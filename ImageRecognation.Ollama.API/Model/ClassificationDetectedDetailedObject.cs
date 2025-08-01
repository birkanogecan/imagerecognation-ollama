using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ImageRecognation.Ollama.API.Model
{
    public class ClassificationDetectedDetailedObject
    {
        [JsonPropertyName("category")]
        public string Category { get; set; }
        [JsonPropertyName("subcategory")]
        public string Subcategory { get; set; }
        [JsonPropertyName("brand")]
        public string Brand { get; set; }
        [JsonPropertyName("gender")]
        public string Gender { get; set; }
        [JsonPropertyName("size")]
        public string Size { get; set; }
        [JsonPropertyName("fit_type")]
        public string FitType { get; set; }
        [JsonPropertyName("sleeve_length")]
        public string SleeveLength { get; set; }
        [JsonPropertyName("length")]
        public string Length { get; set; }
        [JsonPropertyName("waist_type")]
        public string WaistType { get; set; }
        [JsonPropertyName("rise")]
        public string Rise { get; set; }
        [JsonPropertyName("color")]
        public string Color { get; set; }
        [JsonPropertyName("material")]
        public string Material { get; set; }
        [JsonPropertyName("fabric_type")]
        public string FabricType { get; set; }
        [JsonPropertyName("pattern")]
        public string Pattern { get; set; }
        [JsonPropertyName("neckline")]
        public string Neckline { get; set; }
        [JsonPropertyName("closure_type")]
        public string ClosureType { get; set; }
        [JsonPropertyName("details")]
        public string Details { get; set; }
        [JsonPropertyName("season")]
        public string Season { get; set; }
        [JsonPropertyName("usage_occasion")]
        public string UsageOccasion { get; set; }
        [JsonPropertyName("care_instructions")]
        public string CareInstructions { get; set; }
        [JsonPropertyName("stretchability")]
        public string Stretchability { get; set; }
        [JsonPropertyName("transparency_level")]
        public string TransparencyLevel { get; set; }
        [JsonPropertyName("lining")]
        public string Lining { get; set; }
        [JsonPropertyName("sustainability")]
        public string Sustainability { get; set; }
    }

    public static class ClassificationMappingExtensions
    {
        public static ClassificationDetectedObject ToDetectedObject(this ClassificationDetectedDetailedObject detailed, string objectLabel = "")
        {
            var attributes = new Dictionary<string, string>
            {
                ["category"] = detailed.Category ?? string.Empty,
                ["subcategory"] = detailed.Subcategory ?? string.Empty,
                ["brand"] = detailed.Brand ?? string.Empty,
                ["gender"] = detailed.Gender ?? string.Empty,
                ["size"] = detailed.Size ?? string.Empty,
                ["fit_type"] = detailed.FitType ?? string.Empty,
                ["sleeve_length"] = detailed.SleeveLength ?? string.Empty,
                ["length"] = detailed.Length ?? string.Empty,
                ["waist_type"] = detailed.WaistType ?? string.Empty,
                ["rise"] = detailed.Rise ?? string.Empty,
                ["color"] = detailed.Color ?? string.Empty,
                ["material"] = detailed.Material ?? string.Empty,
                ["fabric_type"] = detailed.FabricType ?? string.Empty,
                ["pattern"] = detailed.Pattern ?? string.Empty,
                ["neckline"] = detailed.Neckline ?? string.Empty,
                ["closure_type"] = detailed.ClosureType ?? string.Empty,
                ["details"] = detailed.Details ?? string.Empty,
                ["season"] = detailed.Season ?? string.Empty,
                ["usage_occasion"] = detailed.UsageOccasion ?? string.Empty,
                ["care_instructions"] = detailed.CareInstructions ?? string.Empty,
                ["stretchability"] = detailed.Stretchability ?? string.Empty,
                ["transparency_level"] = detailed.TransparencyLevel ?? string.Empty,
                ["lining"] = detailed.Lining ?? string.Empty,
                ["sustainability"] = detailed.Sustainability ?? string.Empty
            };
            return new ClassificationDetectedObject
            {
                ObjectLabel = objectLabel,
                Attributes = attributes
            };
        }
    }
}