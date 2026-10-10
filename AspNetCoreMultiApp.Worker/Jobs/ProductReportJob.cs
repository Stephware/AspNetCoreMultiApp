using System.Net.Http.Json;
using System.Text.Json;
using AspNetCoreMultiApp.Worker.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AspNetCoreMultiApp.Worker.Jobs
{
    public class ProductReportJob
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductReportJob> _logger;

        public ProductReportJob(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ProductReportJob> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task ExecuteAsync()
        {
            _logger.LogInformation("ProductReportJob started at: {time}", DateTime.Now);
            var client = _httpClientFactory.CreateClient("Api");
            var products = await client.GetFromJsonAsync<List<ProductDTO>>("api/products") ?? new List<ProductDTO>();
            var directory = _configuration["WorkerSettings:ReportDirectory"] ?? "Reports";
            Directory.CreateDirectory(directory);

            var fileName = $"Product-report-{DateTime.Now:yyyyMMdd-HHmmss}.json";
            var filePath = Path.Combine(directory, fileName);
            var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });

            await File.WriteAllTextAsync(filePath, json);

            _logger.LogInformation("ProductReportJob completed at: {time}. Report saved to {filePath}", DateTime.Now, filePath);
        }
    }
}
