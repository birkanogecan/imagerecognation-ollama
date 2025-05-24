using ImageRecognation.Ollama.API.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ImageRecognation.Ollama.API.Domain
{
    public class TextRecognationEngine
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public TextRecognationEngine(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> TextRecognation(IFormFile file)
        {
            using var ms = new MemoryStream();

            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            string base64Image = Convert.ToBase64String(fileBytes);
            
            var client = _httpClientFactory.CreateClient();

            var ollamaRequest = new
            {
                model = "qwen2.5vl:7b",
                prompt = $"Read all texts in the image, output in lines.",
                stream = false,
                images = new[] { base64Image }
            };

            var response = await client.PostAsJsonAsync("http://localhost:11434/api/generate", ollamaRequest);

            var ollamaResult = await response.Content.ReadFromJsonAsync<OllamaApiResponse>();

            return ollamaResult.response;
        }

    }
}
