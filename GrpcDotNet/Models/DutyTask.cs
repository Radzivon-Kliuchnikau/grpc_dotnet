namespace GrpcDotNet.Models;

public class DutyTask
{
    public int Id { get; set; }
    
    public string? TaskName { get; set; }
    
    public string? Description { get; set; }

    public string? TaskStatus { get; set; } = "New Task";
}