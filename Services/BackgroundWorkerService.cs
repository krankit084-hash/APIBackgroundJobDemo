using BackgroundJobDemo.Data;
using Microsoft.EntityFrameworkCore;

namespace BackgroundJobDemo.Services;

public class BackgroundWorkerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundWorkerService> _logger;

    public BackgroundWorkerService(IServiceProvider serviceProvider, ILogger<BackgroundWorkerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var job = await context.BackgroundJobs
                    .FirstOrDefaultAsync(j => j.Status == "Pending", stoppingToken);

                if (job != null)
                {
                    job.Status = "Processing";
                    job.StartedAt = DateTime.Now;
                    await context.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation($"Processing job {job.Id}: {job.JobName}");

                    try
                    {
                        // Simulate heavy work (replace with your actual work)
                        await Task.Delay(new Random().Next(10000, 30000), stoppingToken);

                        job.Status = "Completed";
                        job.CompletedAt = DateTime.Now;
                        job.ResultMessage = "Job completed successfully!";
                        await context.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation($"Job {job.Id} completed.");
                    }
                    catch (Exception ex)
                    {
                        job.Status = "Failed";
                        job.ErrorMessage = ex.Message;
                        await context.SaveChangesAsync(stoppingToken);

                        _logger.LogError($"Job {job.Id} failed: {ex.Message}");
                    }
                }

                await Task.Delay(5000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Worker error: {ex.Message}");
                await Task.Delay(5000, stoppingToken);
            }
        }

        _logger.LogInformation("Background Worker stopped.");
    }
}