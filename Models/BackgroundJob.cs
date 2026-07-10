namespace BackgroundJobDemo.Models;

public class BackgroundJob
{
    public int Id { get; set; }
    public string JobName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ResultMessage { get; set; }
    public string? ErrorMessage { get; set; }
}