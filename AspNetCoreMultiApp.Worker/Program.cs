using AspNetCoreMultiApp.Worker.Jobs;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "SQL Server connection string is missing. Set ConnectionStrings__DefaultConnection.");
}

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5173/";
if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var apiUri)
    || (apiUri.Scheme != Uri.UriSchemeHttp && apiUri.Scheme != Uri.UriSchemeHttps))
{
    throw new InvalidOperationException("ApiSettings:BaseUrl must be a valid HTTP(S) URL.");
}

builder.Services.AddHttpClient("Api", client => client.BaseAddress = apiUri);
builder.Services.AddTransient<ProductReportJob>();

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
    {
        PrepareSchemaIfNecessary = true,
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 5;
    options.Queues = ["default"];
});

using var host = builder.Build();
var recurringJobs = host.Services.GetRequiredService<IRecurringJobManager>();
recurringJobs.AddOrUpdate<ProductReportJob>(
    "product-report",
    job => job.ExecuteAsync(),
    Cron.Minutely);

await host.RunAsync();
