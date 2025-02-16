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
builder.Services.AddSingleton<ChatService>();
builder.Services.AddHostedService<ChatDispatchBackgroundService>();

var app = builder.Build();

app.UseWebSockets();

app.RegisterChatEndpoints();

app.Run();

