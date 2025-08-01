namespace ImageRecognation.Ollama.API.Model
{
    public record ClassificationResult
    {
        public List<ClassificationDetectedObject> Objects { get; set; } = new();
    }
}
