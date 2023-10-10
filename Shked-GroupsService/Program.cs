using ShkedGroupsService.Application.DTO.ScheduleDTO.JsonConverters;
using ShkedGroupsService.Application.Infrastructure;
using ShkedGroupsService.Application.Services;
using ShkedGroupsService.Application.Sources;
using ShkedGroupsService.DAL;
using ShkedGroupsService.DAL.Infrastructure;
using ShkedGroupsService.DAL.Options;
using ShkedGroupsService.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("MongoOptions"));
builder.Services.Configure<ScheduleAPIOptions>(builder.Configuration.GetSection("ScheduleApiOptions"));

builder.Services.AddTransient<IScheduleDbContext, ScheduleDbContext>();
builder.Services.AddTransient<IScheduleRepository, ScheduleRepository>();
builder.Services.AddTransient<IScheduleApi, ScheduleAPI>();
builder.Services.AddTransient<IGroupsService,GroupsService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers().AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter()));
var app = builder.Build();
if (!builder.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseRouting();
app.MapControllers();

app.Run();