using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Serilog;
using TonedChat.Web.Data;
using TonedChat.Web.Endpoints;
using TonedChat.Web.Services;
using TonedChat.Web.Utils;


// create our logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Application is starting");

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddSerilog();

builder.Services.AddSingleton<IClock>(SystemClock.Instance);

// Create our database
var dbFile = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "taut-chat.db");
Log.Information("Using '{dbFile}' as our database file", dbFile);
builder.Services.AddDbContext<TautDatabaseContext>(options =>
{
    options.UseSqlite($"Data Source={dbFile}", x => x.UseNodaTime())
        .UseSnakeCaseNamingConvention();
});

builder.Services.AddScoped<ChatMessageService>();
builder.Services.AddScoped<ChatChannelService>();

builder.Services.AddSingleton<MessageService>();
builder.Services.AddHostedService<MessageDispatchBackgroundService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173");
    });
});

builder.Services.Configure<JsonOptions>(TautSerializer.ApplyJsonSerializerOptions);

var app = builder.Build();

app.UseWebSockets();
app.UseCors();

app.RegisterChatEndpoints();

// Migrate our database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TautDatabaseContext>();
    await db.Database.MigrateAsync();
}

// Register the chat service to listen for application stop
var chatService = app.Services.GetRequiredService<MessageService>();
app.Lifetime.ApplicationStopping.Register(chatService.Stop);

app.Run();
