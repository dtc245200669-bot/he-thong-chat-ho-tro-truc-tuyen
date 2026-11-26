
using ChatService.Data;
using ChatService.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Ket noi PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Dang ky SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// API trang chu
app.MapGet("/", () => Results.Ok(new
{
    message = "He thong chat ho tro truc tuyen dang hoat dong"
}));

// API kiem tra ket noi database
app.MapGet("/health/database", async (ApplicationDbContext dbContext) =>
{
    var connected = await dbContext.Database.CanConnectAsync();

    return connected
        ? Results.Ok(new { status = "healthy", database = "connected" })
        : Results.Problem("Khong the ket noi PostgreSQL");
});

// API tao cuoc hoi thoai
app.MapPost("/conversations", async (
    CreateConversationRequest request,
    ApplicationDbContext dbContext) =>
{
    if (string.IsNullOrWhiteSpace(request.CustomerName))
    {
        return Results.BadRequest(new
        {
            message = "Ten khach hang khong duoc de trong"
        });
    }

    var conversation = new Conversation
    {
        CustomerName = request.CustomerName.Trim(),
        Status = "Waiting",
        CreatedAt = DateTime.UtcNow
    };

    dbContext.Conversations.Add(conversation);
    await dbContext.SaveChangesAsync();

    return Results.Created(
        $"/conversations/{conversation.Id}",
        new
        {
            conversation.Id,
            conversation.CustomerName,
            conversation.Status,
            conversation.CreatedAt
        }
    );
});

// API lay danh sach cuoc hoi thoai
app.MapGet("/conversations", async (ApplicationDbContext dbContext) =>
{
    var conversations = await dbContext.Conversations
        .AsNoTracking()
        .OrderByDescending(c => c.CreatedAt)
        .Select(c => new
        {
            c.Id,
            c.CustomerName,
            c.AgentName,
            c.Status,
            c.CreatedAt,
            c.ClosedAt
        })
        .ToListAsync();

    return Results.Ok(conversations);
});

// API xem chi tiet mot cuoc hoi thoai
app.MapGet("/conversations/{id:guid}", async (
    Guid id,
    ApplicationDbContext dbContext) =>
{
    var conversation = await dbContext.Conversations
        .AsNoTracking()
        .Where(c => c.Id == id)
        .Select(c => new
        {
            c.Id,
            c.CustomerName,
            c.AgentName,
            c.Status,
            c.CreatedAt,
            c.ClosedAt
        })
        .FirstOrDefaultAsync();

    if (conversation == null)
    {
        return Results.NotFound(new
        {
            message = "Khong tim thay cuoc hoi thoai"
        });
    }

    return Results.Ok(conversation);
});

// API lay lich su tin nhan cua cuoc hoi thoai
app.MapGet("/conversations/{id:guid}/messages", async (
    Guid id,
    ApplicationDbContext dbContext) =>
{
    // Kiem tra cuoc hoi thoai co ton tai khong
    var conversationExists = await dbContext.Conversations
        .AnyAsync(c => c.Id == id);

    if (!conversationExists)
    {
        return Results.NotFound(new
        {
            message = "Khong tim thay cuoc hoi thoai"
        });
    }

    // Lay danh sach tin nhan theo thoi gian gui
    var messages = await dbContext.Messages
        .AsNoTracking()
        .Where(m => m.ConversationId == id)
        .OrderBy(m => m.SentAt)
        .Select(m => new
        {
            m.Id,
            m.ConversationId,
            m.SenderName,
            m.SenderType,
            m.Content,
            m.SentAt
        })
        .ToListAsync();

    return Results.Ok(messages);
});

// API dong cuoc hoi thoai
app.MapPost("/conversations/{id:guid}/close", async (
    Guid id,
    ApplicationDbContext dbContext) =>
{
    var conversation = await dbContext.Conversations
        .FirstOrDefaultAsync(c => c.Id == id);

    // Kiem tra cuoc hoi thoai co ton tai khong
    if (conversation == null)
    {
        return Results.NotFound(new
        {
            message = "Khong tim thay cuoc hoi thoai"
        });
    }

    // Kiem tra da dong hay chua
    if (conversation.Status == "Closed")
    {
        return Results.Ok(new
        {
            message = "Cuoc hoi thoai da duoc dong truoc do",
            conversation.Id,
            conversation.Status,
            conversation.ClosedAt
        });
    }

    // Cap nhat trang thai
    conversation.Status = "Closed";
    conversation.ClosedAt = DateTime.UtcNow;

    await dbContext.SaveChangesAsync();

    return Results.Ok(new
    {
        message = "Dong cuoc hoi thoai thanh cong",
        conversation.Id,
        conversation.Status,
        conversation.ClosedAt
    });
});

app.Run();

// Du lieu gui len khi tao cuoc hoi thoai
public record CreateConversationRequest(string CustomerName);
