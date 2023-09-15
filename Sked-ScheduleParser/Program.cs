using System.Reflection;
using SkedScheduleParser.Application.Handlers.Options;
using SkedScheduleParser.Application.Infrastructure;
using SkedScheduleParser.Application.Services;
using SkedScheduleParser.Application.Services.Options;

var builder = WebApplication.CreateBuilder(args);

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();