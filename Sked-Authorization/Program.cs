using SkedAuthorization.Application.Infrastructure;
using SkedAuthorization.Application.Services;
using SkedAuthorization.Application.Services.Options;
using SkedAuthorization.DAL.DbContext;
using SkedAuthorization.DAL.Infrastructure;
using SkedAuthorization.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using SkedAuthorization.DAL.DbContext.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IUserDbContext, MongoUserDbContext>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("AuthOptions"));
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("MongoOptions"));
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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