using Shked.UserDAL.DbContext;
using Shked.UserDAL.DbContext.Options;
using Shked.UserDAL.Infrastructure;
using Shked.UserDAL.Repositories;
using ShkedUsersService.Application.Infrastructure;
using ShkedUsersService.Application.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<IUserDbContext, MongoUserDbContext>();

builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("MongoOptions"));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();