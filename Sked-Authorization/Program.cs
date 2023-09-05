using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SkedAuthorization.Application.Infrastructure;
using SkedAuthorization.Application.Services;
using SkedAuthorization.Application.Services.Options;
using SkedAuthorization.DAL.DbContext;
using SkedAuthorization.DAL.Infrastructure;
using SkedAuthorization.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SkedAuthorization.DAL.DbContext.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IUserDbContext, MongoUserDbContext>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
var jwtOptions = builder.Configuration.GetSection("AuthOptions"); 
builder.Services.Configure<AuthOptions>(jwtOptions);
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("MongoOptions"));
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt => opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = jwtOptions["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtOptions["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions["Secret"])),
        ValidateLifetime = true
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();