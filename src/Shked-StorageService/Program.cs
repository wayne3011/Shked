using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.Runtime.Endpoints;
using Amazon.S3;
using ShkedStorageService.Application.Infrastructure;
using ShkedStorageService.Application.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<ITaskAttachmentsService, TaskAttachmentsService>();


var AWSOptions = builder.Configuration.GetSection("AWS");
builder.Services.AddAWSService<IAmazonS3>(new AWSOptions()
{
    Credentials = new BasicAWSCredentials(AWSOptions["AccessKey"], AWSOptions["SecretKey"]),
    DefaultClientConfig =
    {
        ServiceURL = AWSOptions["ServiceUrl"],
        Timeout = TimeSpan.FromSeconds(5),
    },
    Profile = "s3"
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<StorageOptions>(AWSOptions);
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}
app.UseRouting();
app.MapControllers();
app.Run();