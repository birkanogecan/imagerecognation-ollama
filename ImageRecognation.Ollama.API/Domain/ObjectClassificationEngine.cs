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
        public async Task<ClassificationResult> Classify(IFormFile file)
        {

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            string base64Image = Convert.ToBase64String(fileBytes);
            using Image<Rgba32> img = Image.Load<Rgba32>(fileBytes);
            // 2. Ollama'ya HTTP ile istek at
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
                catch
                {

                }
            }


            //Font font = SystemFonts.CreateFont("Arial", 18);

            //foreach (var d in detectedObjects)
            //{
            //    Point point = new Point(d.X, d.Y);
            //    Size size = new Size(d.Width, d.Height);
            //    var rect = new Rectangle(point, size);

            //    img.Mutate(ctx =>
            //    {
            //        ctx.Draw(Pens.Solid(Color.Red, 3), rect);
            //        var labelPos = new PointF(d.X, Math.Max(0, d.Y - 24));
            //        ctx.DrawText(d.Label, font, Color.Yellow, labelPos);
            //    });
            //}

            using var outStream = new MemoryStream();
            img.SaveAsPng(outStream);

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

