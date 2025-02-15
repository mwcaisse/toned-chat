using Serilog;
using TonedChat.Web.Endpoints;

// create our logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Application is starting");

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddSerilog();

var app = builder.Build();

app.UseWebSockets();

app.RegisterChatEndpoints();

app.Run();

