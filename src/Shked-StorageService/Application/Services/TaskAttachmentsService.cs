using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ShkedStorageService.Application.DTO;
using ShkedStorageService.Application.Infrastructure;

namespace ShkedStorageService.Application.Services;

public class TaskAttachmentsService : ITaskAttachmentsService
{
    private readonly IAmazonS3 _s3client;
    private readonly IOptions<StorageOptions> _storageOptions;
    private readonly string ThumbnailsFolderName = "THUMBNAILS/";
    public TaskAttachmentsService(IAmazonS3 s3Client, IOptions<StorageOptions> storageOptions)
    {
        _s3client = s3Client;
        _storageOptions = storageOptions;
    }

    private string GetTempFilesPath(string userId) => "TEMP/" + userId + '/';
    private string GetTempThumbnailsPath(string userId) => "TEMP/" + userId + '/' + "THUMBNAILS/";
    public async Task<IEnumerable<string>> CreateAttachmentsAsync(IFormFileCollection fileStream, string taskId)
    {
        var path = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(taskId)));
        var filePaths = new List<string?>();
        foreach (var file in fileStream)
        {
            var request = new PutObjectRequest()
            {
                BucketName = _storageOptions.Value.BucketName,
                Key = path + file.FileName,
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

    public async Task<CreationResult?> CreateTemporaryFileAsync(IFormFile miniature, IFormFile file, string userId)
    {
        var creationResult = new CreationResult();
        var filesPath = GetTempFilesPath(userId);
        var miniaturePath = GetTempThumbnailsPath(userId);
        
        PutObjectResponse response;
        if (!await UploadMiniatureAsync(miniature, file, filesPath, creationResult)) return null;
        if (!await UploadFileAsync(file, miniaturePath, creationResult))
        {
            await DeleteMiniatureAsync(miniaturePath, file.FileName);
            return null;
        }

        return creationResult;
    }

    public async Task<bool> MoveToPermanentFiles(string userId, string taskId)
    {
        var listObjects = _s3client.Paginators.ListObjectsV2(new ListObjectsV2Request()
        {
            BucketName = _storageOptions.Value.BucketName,
            Prefix = GetTempFilesPath(userId)
        });
        await foreach (var response in listObjects.Responses)
        {
            foreach (var s3Object in response.S3Objects.Where(x => !Regex.IsMatch(x.Key, $"TEMP/.*/{ThumbnailsFolderName}.*")))
            {
                string oldPath = s3Object.Key;
                string fileName = oldPath.Substring(oldPath.LastIndexOf('/') + 1);
                string oldThumbnailPath = GetTempThumbnailsPath(userId) + fileName;
                string newPath = taskId + '/' + fileName;
                string newThumbnailPath = taskId + '/' + ThumbnailsFolderName + fileName;
                
                var copyFileRequest = new CopyObjectRequest()
                {
                    SourceBucket = _storageOptions.Value.BucketName,
                    DestinationBucket = _storageOptions.Value.BucketName,
                    SourceKey = oldPath,
                    DestinationKey = newPath
                };
                var copyThumbnailRequest = new CopyObjectRequest()
                {
                    SourceBucket = _storageOptions.Value.BucketName,
                    DestinationBucket = _storageOptions.Value.BucketName,
                    SourceKey = oldThumbnailPath,
                    DestinationKey = newThumbnailPath
                };
                var copyObjectResponse = await _s3client.CopyObjectAsync(copyThumbnailRequest);
                if (copyObjectResponse.HttpStatusCode != HttpStatusCode.OK) return false;
                copyObjectResponse = await _s3client.CopyObjectAsync(copyFileRequest);
                if (copyObjectResponse.HttpStatusCode != HttpStatusCode.OK) return false;

                var deleteFileRequest = new DeleteObjectsRequest()
                {
                    BucketName = _storageOptions.Value.BucketName,
                    Objects = new List<KeyVersion>()
                    {
                        new() { Key = oldPath },
                        new() { Key = oldThumbnailPath }
                    }
                };
                var deleteResponse = await _s3client.DeleteObjectsAsync(deleteFileRequest);
                if (deleteResponse.HttpStatusCode != HttpStatusCode.OK) return false;
            }
        }

        return true;
    }

    // public async Task<FileDTO?> GetFileThumbnail(string fileName, string userId)
    // {
    //     try
    //     {
    //         var path = "TEMP/" + userId + '/' + ThumbnailsFolderName + fileName;
    //         var response = await _s3client.GetObjectAsync(_storageOptions.Value.BucketName, path);
    //         if (response.HttpStatusCode != HttpStatusCode.OK) return null;
    //         var fileDto = new FileDTO()
    //         {
    //             FileName = fileName,
    //             ContentType = response.Metadata["Content-Type"],
    //             FileStream = response.ResponseStream,
    //             LastModified = response.LastModified
    //         };
    //         return fileDto;
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         return null;
    //     }
    //
    // }

    private async Task DeleteMiniatureAsync(string path, string fileName)
    {
        var deleteMiniatureRequest = new DeleteObjectRequest()
        {
            BucketName = _storageOptions.Value.BucketName,
            Key = path + fileName
        };
        await _s3client.DeleteObjectAsync(deleteMiniatureRequest);
    }

    private async Task<bool> UploadFileAsync(IFormFile file, string path, CreationResult creationResult)
    {
        try
        {
            var uploadFileRequest = new PutObjectRequest()
            {
                BucketName = _storageOptions.Value.BucketName,
                Key = path + file.FileName,
                InputStream = file.OpenReadStream()
            };
            uploadFileRequest.Metadata.Add("Content-Type", file.ContentType);
            var response = await _s3client.PutObjectAsync(uploadFileRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                creationResult.FilePath = uploadFileRequest.Key;
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private async Task<bool> UploadMiniatureAsync(IFormFile miniature, IFormFile file, string path,
        CreationResult creationResult)
    {
        try
        {
            var uploadMiniatureRequest = new PutObjectRequest()
            {
                BucketName = _storageOptions.Value.BucketName,
                Key = path + file.FileName,
                InputStream = miniature.OpenReadStream()
            };
            uploadMiniatureRequest.Metadata.Add("Content-Type", file.ContentType);
            var response = await _s3client.PutObjectAsync(uploadMiniatureRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                creationResult.ThumbnailPath = uploadMiniatureRequest.Key;
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
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

    public async Task<FileDTO?> GetTemporaryThumbnail(string userId, string fileName)
    {
        var thumbnailKey = GetTempThumbnailsPath(userId) + fileName;
        var s3Object = await _s3client.GetObjectAsync(_storageOptions.Value.BucketName, thumbnailKey);
        if (s3Object.HttpStatusCode != HttpStatusCode.OK) return null;
        var fileDto = new FileDTO()
        {
            FileName = fileName,
            ContentType = s3Object.Metadata["Content-Type"],
            FileStream = s3Object.ResponseStream,
            LastModified = s3Object.LastModified
        };
        return fileDto;
    }

    public async Task<FileDTO?> GetTemporaryFile(string userId, string fileName)
    {
        var filePath = GetTempFilesPath(userId) + fileName;
        var s3Object = await _s3client.GetObjectAsync(_storageOptions.Value.BucketName, filePath);
        if (s3Object.HttpStatusCode != HttpStatusCode.OK) return null;
        var fileDto = new FileDTO()
        {
            FileName = fileName,
            ContentType = s3Object.Metadata["Content-Type"],
            FileStream = s3Object.ResponseStream,
            LastModified = s3Object.LastModified
        };
        return fileDto;
    }
}