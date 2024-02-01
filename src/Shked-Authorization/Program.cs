using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ShkedAuthorization.Application.Infrastructure;
using ShkedAuthorization.Application.Services;
using ShkedAuthorization.Application.Services.Options;
using Microsoft.IdentityModel.Tokens;
using Shked.UserDAL.DbContext;
using Shked.UserDAL.DbContext.Options;
using Shked.UserDAL.Infrastructure;
using Shked.UserDAL.Repositories;
using ShkedAuthorization.Application.Data.DTO;
using ShkedAuthorization.Application.Services.Utils;
using ShkedAuthorization.Application.Services.Validators;

var builder = WebApplication.CreateBuilder(args);

//регистрация сервисов
builder.Services.AddTransient<IAuthService, AuthService>();
//регистрация конфигураций
var jwtOptions = builder.Configuration.GetSection("AuthOptions"); 
builder.Services.Configure<AuthOptions>(jwtOptions);
Console.WriteLine(jwtOptions["Issuer"]);
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("MongoOptions"));
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
//регистрация дополнительных сервисов
builder.Services.AddTransient<ITokenManager, TokenManager>();
builder.Services.AddTransient<IUserDbContext, MongoUserDbContext>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//валидаторы
builder.Services.AddScoped<IValidator<SignUpDTO>, SignUpDTOValidator>();
builder.Services.AddScoped<IValidator<SignInDTO>, SignInDTOValidator>();

builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.yaml", "Auth Service Open API"));
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();