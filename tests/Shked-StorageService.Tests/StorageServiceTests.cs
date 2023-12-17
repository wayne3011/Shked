using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using ShkedStorageService.Application.DTO;
using ShkedStorageService.Application.Services;
using Xunit;

namespace Shked_StorageService.Tests;

public class StorageServiceTests
{
    private TaskAttachmentsService _taskAttachmentsService =
        new TaskAttachmentsService(S3ClientMoq.Create(), StorageOptionsTests.Options);
    private TaskAttachmentsService _invalidTaskAttachmentsService =
        new TaskAttachmentsService(S3ClientMoq.CreateInvalid(), StorageOptionsTests.Options);

    private TaskAttachmentsService _invalidTaskAttachmentsService1 =
        new TaskAttachmentsService(S3ClientMoq.CreateInvalidMiniatureLoadingAmazonS3Moq(), StorageOptionsTests.Options);
    private TaskAttachmentsService _throwablePutObjectTaskAttachmentsService =
        new TaskAttachmentsService(S3ClientMoq.CreateThrowableAmazonS3Moq(), StorageOptionsTests.Options);
    [Fact]
    public async void GetTaskAttachmentThumbnail_ValidTest1()
    {
        var fs = new FileStream("TestFiles/json_test_file.json", FileMode.Open);
        var fileDto = new FileDTO()
        {
            ContentType = "application/json",
            FileName = "json_test_file.json",
            FileStream = fs,
            LastModified = DateTimeOffset.Now,
            Extension = ".json",
            SizeKb = fs.Length / 1024
        };
        var result = await _taskAttachmentsService.GetTaskAttachmentThumbnail("ef6718c5-0e9e-4797-93e3-88b467d2afab", "json_test_file.json");
        Assert.True((fileDto.LastModified - result.LastModified).TotalSeconds < 1);
        result.Should().BeEquivalentTo(fileDto, cfg => cfg.Excluding(x => x.LastModified).Excluding(x => x.FileStream));
    }
    [Fact]
    public async void GetTaskAttachmentThumbnail_InvalidTest1()
    {
        var fs = new FileStream("TestFiles/json_test_file.json", FileMode.Open);
        var result = await _invalidTaskAttachmentsService.GetTaskAttachmentThumbnail("ef6718c5-0e9e-4797-93e3-88b467d2afab", "json_test_file.json");
        Assert.Null(result);
    }
    
    [Fact]
    public async void GetAttachmentFile_ValidTest()
    {
        var fs = new FileStream("TestFiles/json_test_file.json", FileMode.Open);
        var fileDto = new FileDTO()
        {
            ContentType = "application/json",
            FileName = "json_test_file.json",
            FileStream = fs,
            LastModified = DateTimeOffset.Now,
            Extension = ".json",
            SizeKb = fs.Length / 1024
        };
        var result = await _taskAttachmentsService.GetTaskAttachment("ef6718c5-0e9e-4797-93e3-88b467d2afab", "json_test_file.json");
        Assert.True((fileDto.LastModified - result.LastModified).TotalSeconds < 1);
        result.Should().BeEquivalentTo(fileDto, cfg => cfg.Excluding(x => x.LastModified).Excluding(x => x.FileStream));
    }
    [Fact]
    public async void GetAttachmentFile_InvalidTest1()
    {
        var fs = new FileStream("TestFiles/json_test_file.json", FileMode.Open);
        var result = await _invalidTaskAttachmentsService.GetTaskAttachment("ef6718c5-0e9e-4797-93e3-88b467d2afab", "json_test_file.json");
        Assert.Null(result);
    }

    [Fact]
    public async void CreateTemporaryFile_ValidTest1()
    {
        var fs = new FileStream("TestFiles/json_test_file.json", FileMode.Open);
        var ms = new FileStream("TestFiles/test_thumbnail_picture.jpeg", FileMode.Open);
        IFormFile formFile = new FormFile(fs, 0, fs.Length, "file", "json_test_file.json")
        {
            Headers = new HeaderDictionary()
            {
                {"Content-Type", "application/json"}
            },
            ContentType = "application/json"
        };
        IFormFile thumbnailFile = new FormFile(ms, 0, fs.Length, "file", "test_thumbnail_picture.jpeg")
        {
            Headers = new HeaderDictionary()
            {
                {"Content-Type", "application/json"}
            },
            ContentType = "image/jpeg"
        };
        string userId = "2bd04efb-cb61-4daf-a4ed-4022388da6a6";
        var exceptedResult = new CreationResult()
        {
            FilePath = "TEMP/2bd04efb-cb61-4daf-a4ed-4022388da6a6/json_test_file.json",
            ThumbnailPath = "TEMP/2bd04efb-cb61-4daf-a4ed-4022388da6a6/THUMBNAILS/json_test_file.json",
        };
        
        var result = await _taskAttachmentsService.CreateTemporaryFileAsync(thumbnailFile, formFile, userId);

        result.Should().BeEquivalentTo(exceptedResult);
    }
    
    [Fact]
    public async void CreateTemporaryFile_InvalidTest1()
    {
        var fs = new FileStream("TestFiles/json_test_file.json", FileMode.Open);
        var ms = new FileStream("TestFiles/test_thumbnail_picture.jpeg", FileMode.Open);
        IFormFile formFile = new FormFile(fs, 0, fs.Length, "file", "json_test_file.json")
        {
            Headers = new HeaderDictionary()
            {
                {"Content-Type", "application/json"}
            },
            ContentType = "application/json"
        };
        IFormFile thumbnailFile = new FormFile(ms, 0, fs.Length, "file", "test_thumbnail_picture.jpeg")
        {
            Headers = new HeaderDictionary()
            {
                {"Content-Type", "application/json"}
            },
            ContentType = "image/jpeg"
        };
        string userId = "2bd04efb-cb61-4daf-a4ed-4022388da6a6";
        var exceptedResult = new CreationResult()
        {
            FilePath = "TEMP/2bd04efb-cb61-4daf-a4ed-4022388da6a6/json_test_file.json",
            ThumbnailPath = "TEMP/2bd04efb-cb61-4daf-a4ed-4022388da6a6/THUMBNAILS/json_test_file.json",
        };
        
        var result = await _invalidTaskAttachmentsService.CreateTemporaryFileAsync(thumbnailFile, formFile, userId);

        Assert.Null(result);
    }
    [Fact]
    public async void CreateTemporaryFile_InvalidTest2()
    {
        var fs = new FileStream("TestFiles/json_test_file.json", FileMode.Open);
        var ms = new FileStream("TestFiles/test_thumbnail_picture.jpeg", FileMode.Open);
        IFormFile formFile = new FormFile(fs, 0, fs.Length, "file", "json_test_file.json")
        {
            Headers = new HeaderDictionary()
            {
                {"Content-Type", "application/json"}
            },
            ContentType = "application/json"
        };
        IFormFile thumbnailFile = new FormFile(ms, 0, fs.Length, "file", "test_thumbnail_picture.jpeg")
        {
            Headers = new HeaderDictionary()
            {
                {"Content-Type", "application/json"}
            },
            ContentType = "image/jpeg"
        };
        string userId = "2bd04efb-cb61-4daf-a4ed-4022388da6a6";
        var exceptedResult = new CreationResult()
        {
            FilePath = "TEMP/2bd04efb-cb61-4daf-a4ed-4022388da6a6/json_test_file.json",
            ThumbnailPath = "TEMP/2bd04efb-cb61-4daf-a4ed-4022388da6a6/THUMBNAILS/json_test_file.json",
        };
        
        var result = await  _invalidTaskAttachmentsService1.CreateTemporaryFileAsync(thumbnailFile, formFile, userId);

        Assert.Null(result);
    }
    [Fact]
    public async void CreateTemporaryFile_InvalidTest3()
    {
        var fs = new FileStream("TestFiles/json_test_file.json", FileMode.Open);
        var ms = new FileStream("TestFiles/test_thumbnail_picture.jpeg", FileMode.Open);
        IFormFile formFile = new FormFile(fs, 0, fs.Length, "file", "json_test_file.json")
        {
            Headers = new HeaderDictionary()
            {
                {"Content-Type", "application/json"}
            },
            ContentType = "application/json"
        };
        IFormFile thumbnailFile = new FormFile(ms, 0, fs.Length, "file", "test_thumbnail_picture.jpeg")
        {
            Headers = new HeaderDictionary()
            {
                {"Content-Type", "application/json"}
            },
            ContentType = "image/jpeg"
        };
        string userId = "2bd04efb-cb61-4daf-a4ed-4022388da6a6";
        var exceptedResult = new CreationResult()
        {
            FilePath = "TEMP/2bd04efb-cb61-4daf-a4ed-4022388da6a6/json_test_file.json",
            ThumbnailPath = "TEMP/2bd04efb-cb61-4daf-a4ed-4022388da6a6/THUMBNAILS/json_test_file.json",
        };
        
        var result = await  _throwablePutObjectTaskAttachmentsService.CreateTemporaryFileAsync(thumbnailFile, formFile, userId);

        Assert.Null(result);
    }
    [Fact]
    public async void CreateTemporaryFile_InvalidTest4()
    {
        var fs = new FileStream("TestFiles/json_test_file.json", FileMode.Open);
        var ms = new FileStream("TestFiles/test_thumbnail_picture.jpeg", FileMode.Open);
        IFormFile formFile = new FormFile(fs, 0, fs.Length, "file", "json_test_file.json")
        {
            Headers = new HeaderDictionary()
            {
                {"Content-Type", "application/json"}
            },
            ContentType = "application/json"
        };
        IFormFile thumbnailFile = new FormFile(ms, 0, fs.Length, "file", "test_thumbnail_picture.jpeg")
        {
            Headers = new HeaderDictionary()
            {
                {"Content-Type", "application/json"}
            },
            ContentType = "image/jpeg"
        };
        string userId = "2bd04efb-cb61-4daf-a4ed-4022388da6a6";
        var exceptedResult = new CreationResult()
        {
            FilePath = "TEMP/2bd04efb-cb61-4daf-a4ed-4022388da6a6/json_test_file.json",
            ThumbnailPath = "TEMP/2bd04efb-cb61-4daf-a4ed-4022388da6a6/THUMBNAILS/json_test_file.json",
        };
        
        var result = await  new TaskAttachmentsService(S3ClientMoq.CreateThrowableForFileUploadsAmazonS3Moq(), StorageOptionsTests.Options)
            .CreateTemporaryFileAsync(thumbnailFile, formFile, userId);

        Assert.Null(result);
    }

    [Fact]
    public async void MoveToPermanentFilesAsync_TestValid1()
    {
        var expectedResult = new List<FileDTO>()
        {
            new FileDTO()
            {
                FileName = "json_test_file.json",
                LastModified = DateTimeOffset.Now,
                Extension = ".json",
                SizeKb = 1,
            }
        };
        var result = await _taskAttachmentsService.MoveToPermanentFilesAsync("2bd04efb-cb61-4daf-a4ed-4022388da6a6",
            "29e087c5-1796-4e40-9550-9caa36e8e3ef");
        result.Should().BeEquivalentTo(expectedResult, options => options.Excluding(x => x.LastModified)
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, precision: TimeSpan.FromMinutes(1)))
            .When(info => info.Path == "LastModified"));
    }
    
    [Fact]
    public async void GetListOfTemporaryFiles_TestValid1()
    {
        var expectedResult = new List<FileDTO>()
        {
            new FileDTO()
            {
                FileName = "json_test_file",
                LastModified = DateTimeOffset.Now,
                Extension = ".json",
                SizeKb = 1,
            }
        };
        var result = await _taskAttachmentsService.GetListOfTemporaryFiles("2bd04efb-cb61-4daf-a4ed-4022388da6a6");
        result.Should().BeEquivalentTo(expectedResult, options => options.Excluding(x => x.LastModified)
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, precision: TimeSpan.FromMinutes(1)))
            .When(info => info.Path == "LastModified"));
    }
        
    [Fact]
    public async void GetTemporaryFileAsync_ValidTest()
    {
        var fs = new FileStream("TestFiles/json_test_file.json", FileMode.Open);
        var fileDto = new FileDTO()
        {
            ContentType = "application/json",
            FileName = "json_test_file.json",
            FileStream = fs,
            LastModified = DateTimeOffset.Now,
            Extension = ".json",
            SizeKb = fs.Length / 1024
        };
        var result = await _taskAttachmentsService.GetTemporaryFileAsync("ef6718c5-0e9e-4797-93e3-88b467d2afab", "json_test_file.json");
        Assert.True((fileDto.LastModified - result.LastModified).TotalSeconds < 1);
        result.Should().BeEquivalentTo(fileDto, cfg => cfg.Excluding(x => x.LastModified).Excluding(x => x.FileStream));
    }
    
    [Fact]
    public async void GetTemporaryThumbnailAsync_ValidTest()
    {
        var fs = new FileStream("TestFiles/json_test_file.json", FileMode.Open);
        var fileDto = new FileDTO()
        {
            ContentType = "application/json",
            FileName = "json_test_file.json",
            FileStream = fs,
            LastModified = DateTimeOffset.Now,
            Extension = ".json",
            SizeKb = fs.Length / 1024
        };
        var result = await _taskAttachmentsService.GetTemporaryThumbnailAsync("ef6718c5-0e9e-4797-93e3-88b467d2afab", "json_test_file.json");
        Assert.True((fileDto.LastModified - result.LastModified).TotalSeconds < 1);
        result.Should().BeEquivalentTo(fileDto, cfg => cfg.Excluding(x => x.LastModified).Excluding(x => x.FileStream));
    }

    [Fact]
    public async void DeletePermanentFileAsync_ValidTest1()
    {
        Assert.True(await _taskAttachmentsService.DeletePermanentFileAsync("ef6718c5-0e9e-4797-93e3-88b467d2afab","json_test_file.json"));
    }
    [Fact]
    public async void DeletePermanentFileAsync_InvalidTest1()
    {
        Assert.False(await _invalidTaskAttachmentsService.DeletePermanentFileAsync("ef6718c5-0e9e-4797-93e3-88b467d2afab","json_test_file.json"));
    }
    [Fact]
    public async void DeleteTemporaryFileAsync_ValidTest1()
    {
        Assert.False(await _invalidTaskAttachmentsService.DeleteTemporaryFileAsync("ef6718c5-0e9e-4797-93e3-88b467d2afab","json_test_file.json"));
    }
}