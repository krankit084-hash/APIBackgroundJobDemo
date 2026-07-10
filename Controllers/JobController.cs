using BackgroundJobDemo.Data;
using BackgroundJobDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackgroundJobDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<JobController> _logger;

    public JobController(AppDbContext context, ILogger<JobController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartJob([FromBody] StartJobRequest request)
    {
        try
        {
            var job = new BackgroundJob
            {
                JobName = request.JobName,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.BackgroundJobs.Add(job);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Job {job.Id} created");

            return Ok(new
            {
                jobId = job.Id,
                status = job.Status,
                message = "Job started successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("status/{id}")]
    public async Task<IActionResult> GetJobStatus(int id)
    {
        var job = await _context.BackgroundJobs.FindAsync(id);

        if (job == null)
            return NotFound(new { error = "Job not found" });

        return Ok(new
        {
            jobId = job.Id,
            jobName = job.JobName,
            status = job.Status,
            createdAt = job.CreatedAt,
            startedAt = job.StartedAt,
            completedAt = job.CompletedAt,
            resultMessage = job.ResultMessage,
            errorMessage = job.ErrorMessage
        });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllJobs()
    {
        var jobs = await _context.BackgroundJobs
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        return Ok(jobs);
    }
}

public class StartJobRequest
{
    public string JobName { get; set; } = string.Empty;
}