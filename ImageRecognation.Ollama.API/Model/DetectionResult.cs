

using System.Text.Json.Serialization;

namespace ImageRecognation.Ollama.API.Model
{
    public record DetectionResult
    {
        [JsonPropertyName("filestream")]
        public byte[] FileStream { get; internal set; }
        
        [JsonPropertyName("objects")]
        public List<DetectedObject> Objects { get; internal set; }
    }
}