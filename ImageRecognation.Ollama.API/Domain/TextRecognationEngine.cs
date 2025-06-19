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
        /// <summary>
        /// Performs text recognition on the provided image file and returns the extracted text.
        /// </summary>
        /// <remarks>This method uses an external API to perform text recognition. The image file is
        /// converted to a Base64-encoded string and sent to the API for processing. Ensure the API endpoint is
        /// accessible and properly configured.</remarks>
        /// <param name="file">The image file to process. Must be a valid <see cref="IFormFile"/> containing the image data.</param>
        /// <returns>A <see cref="string"/> containing the recognized text from the image, organized in lines.</returns>
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
