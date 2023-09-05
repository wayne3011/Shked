using SkedAuthoriztion.Application.Infrastructure;
using SkedAuthoriztion.Application.Services;
using SkedAuthoriztion.Application.Services.Options;
using SkedAuthoriztion.DAL.DbContext;
using SkedAuthoriztion.DAL.Infrastructure;
using SkedAuthoriztion.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IUserDbContext, MongoUserDbContext>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("AuthOptions"));
builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("MongoOptions"));
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