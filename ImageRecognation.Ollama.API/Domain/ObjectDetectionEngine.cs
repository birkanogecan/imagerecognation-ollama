using ImageRecognation.Ollama.API.Model;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageRecognation.Ollama.API.Domain
{
    public class ObjectDetectionEngine
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ObjectDetectionEngine(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<DetectionResult> Detect(IFormFile file)
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
                prompt = $"You are an object‑detection helper.\nReturn only a JSON array. Each item must contain: label (string), x, y, width, height (first detect image size and give detected object pixel coordinates. Compute x, y, width, height value using exactly the provided image widht: {img.Width}px and height: {img.Height}px values.) for every object you detect in the image ",
                stream = false,
                images = new[] { base64Image }
            };

            var response = await client.PostAsJsonAsync("http://localhost:11434/api/generate", ollamaRequest);

            var ollamaResult = await response.Content.ReadFromJsonAsync<OllamaApiResponse>();

            var jsonBlock = ExtractJsonArray(ollamaResult?.response);

            var detectedObjects = new List<DetectedObject>();

            if (!string.IsNullOrWhiteSpace(jsonBlock))
            {
                try
                {
                    detectedObjects = JsonSerializer.Deserialize<List<DetectedObject>>(jsonBlock) ?? new List<DetectedObject>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }


            Font font = SystemFonts.CreateFont("Arial", 18);

            foreach (var d in detectedObjects)
            {
                Point point = new Point(d.X, d.Y);
                Size size = new Size(d.Width, d.Height);
                var rect = new Rectangle(point, size);

                img.Mutate(ctx =>
                {
                    ctx.Draw(Pens.Solid(Color.Red, 3), rect);
                    var labelPos = new PointF(d.X, Math.Max(0, d.Y - 24));
                    ctx.DrawText(d.Label, font, Color.Yellow, labelPos);
                });
            }

            using var outStream = new MemoryStream();

            img.SaveAsPng(outStream);

            DetectionResult detectionResult = new DetectionResult()
            {
                FileStream = outStream.ToArray(),
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
