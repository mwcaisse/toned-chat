using Serilog;
using TonedChat.Web.Endpoints;
using TonedChat.Web.Services;


// create our logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Application is starting");

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddSerilog();

builder.Services.AddSingleton<ChatHistoryService>();
builder.Services.AddSingleton<ChatService>();
builder.Services.AddHostedService<ChatDispatchBackgroundService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173");
    });
});
 

var app = builder.Build();

app.UseWebSockets();
app.UseCors();

app.RegisterChatEndpoints();

app.Run();

