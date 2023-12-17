using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ShkedTasksService.Application.APIs;
using ShkedTasksService.Application.APIs.Options;
using ShkedTasksService.Application.Infrastructure;
using ShkedTasksService.Application.Services;
using ShkedTasksService.DAL;
using ShkedTasksService.DAL.DbContext.Options;
using ShkedTasksService.DAL.Infrastructure;
using ShkedTasksService.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ITaskDbContext, MongoDbContext>();
builder.Services.AddTransient<ITaskRepository, TaskRepository>();

builder.Services.AddTransient<ITaskAttachmentsStorageApi, TaskAttachmentsStorageApi>();
builder.Services.AddTransient<IUsersApi, UsersApi>();

builder.Services.AddTransient<ITasksService, TasksService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.Configure<UsersApiOptions>(builder.Configuration.GetSection("UsersApiSettings"));
var jwtOptions = builder.Configuration.GetSection("AuthOptions");
builder.Services.Configure<TaskAttachmentsStorageApiOptions>(
    builder.Configuration.GetSection("TaskAttachmentsStorageApiSettings"));
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("MongoOptions"));
builder.Services.Configure<UsersApiOptions>(builder.Configuration.GetSection("UsersApiSettings"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
    o.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidIssuer = jwtOptions["Issuer"],
        ValidateAudience = false,
        ValidAudience = jwtOptions["Audience"],
        ValidateIssuerSigningKey = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions["Secret"].PadRight((512/8), '\0'))),
        ValidateLifetime = false,
        ValidateActor = false,
        ValidateTokenReplay = false
    });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();