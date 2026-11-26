namespace ChatService.Models;

public class Conversation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string CustomerName { get; set; } = string.Empty;

    public string? AgentName { get; set; }

    public string Status { get; set; } = "Waiting";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ClosedAt { get; set; }

    public ICollection<Message> Messages { get; set; }
        = new List<Message>();
}