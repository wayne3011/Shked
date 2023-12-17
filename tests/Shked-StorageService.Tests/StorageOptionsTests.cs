using Microsoft.Extensions.Options;
using ShkedStorageService.Application.Services;

namespace Shked_StorageService.Tests;

public class StorageOptionsTests
{
    public static IOptions<StorageOptions> Options { get; } = new OptionsWrapper<StorageOptions>(new StorageOptions()
    {
        AccessKey = "TaskService",
        SecretKey = "12345678",
        ServiceUrl = "http://127.0.0.1:9000",
        BucketName = "shked-tasks"
    });
}