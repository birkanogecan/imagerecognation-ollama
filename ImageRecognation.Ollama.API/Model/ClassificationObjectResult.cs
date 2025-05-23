using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace ImageRecognation.Ollama.API.Model
{
    public record ClassificationObjectResult
    {

        [JsonPropertyName("objects")]
        public List<ClassificationDetectedObject> Objects { get; init; } = new();
    }

    public record ClassificationDetectedObject
    {
        [JsonPropertyName("object_label")]
        public string ObjectLabel { get; init; } = string.Empty;

        [JsonPropertyName("attributes")]
        public Dictionary<string, string> Attributes { get; init; } = new();
    }
}

