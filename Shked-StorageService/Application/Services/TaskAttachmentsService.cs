using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using ShkedStorageService.Application.DTO;
using ShkedStorageService.Application.Infrastructure;

namespace ShkedStorageService.Application.Services;

public class TaskAttachmentsService : ITaskAttachmentsService
{
    private readonly IAmazonS3 _s3client;
    public TaskAttachmentsService(IAmazonS3 s3Client)
    {
        _s3client = s3Client;
    }
    public async Task<IEnumerable<string>> CreateAttachmentsAsync(IFormFileCollection fileStream, string taskId)
    {
        var bucketName = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(taskId)));
        var filePaths = new List<string?>();
        foreach (var file in fileStream)
        {
            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = file.FileName,
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            var response = await _s3client.PutObjectAsync(request);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                filePaths.Add($"{request.BucketName}/{request.Key}");
            }
        }

        return filePaths;
    }

    public async Task<FileDTO> GetAttachmentAsync(string folder, string fileName)
    {
        var response = await _s3client.GetObjectAsync(folder, fileName);
        var fileDto = new FileDTO()
        {
            FileName = fileName,
            ContentType = response.Metadata["Content-Type"],
            FileStream = response.ResponseStream,
            LastModified = response.LastModified
        };
        return fileDto;
    }
}