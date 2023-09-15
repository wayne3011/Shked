using Microsoft.AspNetCore.SignalR;
using Serilog;
using SkedGroupsService.Application.HttpClients;
using SkedGroupsService.Application.HttpClients.Options;
using SkedGroupsService.Application.Hubs;
using SkedGroupsService.Application.Infrastructure;
using SkedGroupsService.Application.Kafka;
using SkedGroupsService.DAL;
using SkedGroupsService.DAL.DbContext.Options;
using SkedGroupsService.DAL.Infrastructure;
using SkedGroupsService.DAL.Repositories;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.Configure<ParserApiOptions>(builder.Configuration.GetSection("ParserApiOptions"));
builder.Services.Configure<KafkaConsumerOptions>(builder.Configuration.GetSection("Kafka"));
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("MongoOptions"));
builder.Services.AddTransient<IScheduleRepository, ScheduleRepository>();
builder.Services.AddTransient<IScheduleDbContext, ScheduleDbContext>();
builder.Services.AddSingleton<KafkaConsumerHostedService>();
builder.Services.AddHostedService(p => p.GetRequiredService<KafkaConsumerHostedService>());
builder.Services.AddTransient<IScheduleParserApi, ScheduleParserApi>();
builder.Services.AddSignalR(p => p = new HubOptions()
{
    HandshakeTimeout = TimeSpan.FromSeconds(1000)
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapHub<GroupHub>("/GroupsSchedule"));
app.MapControllers();
app.Map("/", () => "Hello!!!");
app.Run();