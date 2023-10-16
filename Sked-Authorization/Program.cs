using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SkedAuthorization.Application.Infrastructure;
using SkedAuthorization.Application.Services;
using SkedAuthorization.Application.Services.Options;
using Microsoft.IdentityModel.Tokens;
using Shked.UserDAL.DbContext;
using Shked.UserDAL.DbContext.Options;
using Shked.UserDAL.Infrastructure;
using Shked.UserDAL.Repositories;
using SkedAuthorization.Application.Data.DTO;
using SkedAuthorization.Application.Services.Utils;
using SkedAuthorization.Application.Services.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IUserDbContext, MongoUserDbContext>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

var jwtOptions = builder.Configuration.GetSection("AuthOptions"); 
builder.Services.Configure<AuthOptions>(jwtOptions);
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("MongoOptions"));
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ITokenManager, TokenManager>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions["SecretAccess"])),
        ValidateLifetime = true
    });

builder.Services.AddScoped<IValidator<SignUpDTO>, SignUpDTOValidator>();
builder.Services.AddScoped<IValidator<SignInDTO>, SignInDTOValidator>();

builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();