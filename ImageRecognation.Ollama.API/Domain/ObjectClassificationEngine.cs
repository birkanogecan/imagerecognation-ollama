using ImageRecognation.Ollama.API.Model;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ImageRecognation.Ollama.API.Domain
{
    public class ObjectClassificationEngine
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ObjectClassificationEngine(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        /// <summary>
        /// Analyzes the provided image file to detect textile products and their attributes.
        /// </summary>
        /// <remarks>This method uses a computer vision model to analyze the image and extract information
        /// about textile products. Each detected product includes an object label (e.g., "tshirt", "jeans") and a set
        /// of attributes such as color, material, and category. Attributes with unknown values are explicitly marked as
        /// "unknown".</remarks>
        /// <param name="file">The image file to be analyzed. Must be a valid image format.</param>
        /// <returns>A <see cref="ClassificationResult"/> containing the detected textile products and their attributes. If no
        /// products are detected, the <see cref="ClassificationResult.Objects"/> property will be an empty list.</returns>
        public async Task<ClassificationResult> Classify(IFormFile file)
        {
            using var ms = new MemoryStream();

            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            string base64Image = Convert.ToBase64String(fileBytes);
            using Image<Rgba32> img = Image.Load<Rgba32>(fileBytes);
            
            var client = _httpClientFactory.CreateClient();

            var ollamaRequest = new
            {
                model = "qwen2.5vl:7b",
                prompt = $"You are a cutting-edge computer-vision and fashion product expert. " +
                $"\r\nYour task is to analyze the provided image, detect all textile items " +
                $"\r\n1. Detect each textile product in the image.  " +
                $"\r\n2. For each detected product, output an object with:  " +
                $"\r\n   - \"object_label\": a short identifier (e.g. \"tshirt\", \"jeans\", \"jacket\")  \r\n   - \"attributes\": a JSON object with dynamic keys—each key is the name of an e-commerce attribute you identify (e.g. \"color\", \"sex\", \"material\", \"brand\", \"category\", \"subcategory\", etc.).  \r\n     • Return exactly one value per attribute as a string.  " +
                $"\r\n     • Do NOT return arrays or multiple values.  \r\n3. If you cannot detect a particular attribute, set its value to \"unknown\".  \r\n4. Return ONLY valid JSON with a single root object containing an \"objects\" array—no extra text or explanation.\r\n",
                stream = false,
                images = new[] { base64Image }
            };

            var response = await client.PostAsJsonAsync("http://localhost:11434/api/generate", ollamaRequest);
            var ollamaResult = await response.Content.ReadFromJsonAsync<OllamaApiResponse>();
            var jsonBlock = ExtractJsonArray(ollamaResult?.response);

            var detectedObjects = new List<ClassificationDetectedObject>();

            if (!string.IsNullOrWhiteSpace(jsonBlock))
            {
                try
                {
                    detectedObjects = JsonSerializer.Deserialize<List<ClassificationDetectedObject>>(jsonBlock) ?? new List<ClassificationDetectedObject>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            ClassificationResult detectionResult = new ClassificationResult()
            {
                Objects = detectedObjects
            };

            return detectionResult;
        }

        private static string? ExtractJsonArray(string? response)
        {
            if (string.IsNullOrWhiteSpace(response)) return null;

            var match = Regex.Match(response, @"\[.*\]", RegexOptions.Singleline);

            return match.Success ? match.Value : null;
        }
    }
}

