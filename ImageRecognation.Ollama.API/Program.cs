
using ImageRecognation.Ollama.API.Domain;
using ImageRecognation.Ollama.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace ImageRecognation.Ollama.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddHttpClient();
            builder.Services.AddTransient<ObjectDetectionEngine>();
            builder.Services.AddTransient<ObjectClassificationEngine>();

            builder.Services.AddCors(options =>
            {

                // Geliştirme için tüm origin’lere izin:
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                );
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();
            app.UseCors("AllowAll");
            
            app.MapPost("/detect", async (HttpRequest request) =>
            {
                var form = await request.ReadFormAsync();
                var file = form.Files.GetFile("file");
                var objectDetectionEngine = app.Services.GetRequiredService<ObjectDetectionEngine>();
                DetectionResult detectionResult = await objectDetectionEngine.Detect(file);

                return detectionResult;
            })
            .WithName("Detect");

            app.MapPost("/classify", async (HttpRequest request) =>
            {
                var form = await request.ReadFormAsync();
                var file = form.Files.GetFile("file");
                var objectDetectionEngine = app.Services.GetRequiredService<ObjectClassificationEngine>();
                ClassificationResult classificationResult = await objectDetectionEngine.Classify(file);

                return classificationResult;
            })
            .WithName("Classify");


            app.Run();
        }
    }
}
