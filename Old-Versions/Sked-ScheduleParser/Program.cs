using System.Reflection;
using Serilog;
using SkedScheduleParser.Application.Handlers.Options;
using SkedScheduleParser.Application.Infrastructure;
using SkedScheduleParser.Application.Services;
using SkedScheduleParser.Application.Services.Options;


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Host.UseSerilog();
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Add Kafka
builder.Services.AddTransient<IKafkaProducer, KafkaProducer>();
//Add Services
builder.Services.AddScoped<IScheduleParserService, ScheduleParserService>();
//Add Options
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));
builder.Services.Configure<ScheduleParserOptions>(builder.Configuration.GetSection("ScheduleParserOptions"));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Map("/", () => "Schedule Parser Alive!");
app.UseAuthorization();

app.MapControllers();
app.Map("/", () => "ALIVE!");
app.Run();