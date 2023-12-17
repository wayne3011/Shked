using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Amazon.Runtime.SharedInterfaces;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver.Linq;
using Moq;
using ShkedTasksService.Application.DTO;
using ShkedTasksService.Application.Infrastructure;

namespace Shked_TasksService.Tests;

public class TaskAttachmentStorageApiMoq
{
    public static ITaskAttachmentsStorageApi Create()
    {
        var mock = new Mock<ITaskAttachmentsStorageApi>();
        mock.Setup(x => x.DeletePermanentFile(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult(true));
        mock.Setup(x => x.DeleteTemporaryFile(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.FromResult(true));
        mock.Setup(x => x.UploadTemporaryFile(It.IsAny<IFormFile>(), It.IsAny<IFormFile>(), It.IsAny<string>()))
            .Returns(Task.FromResult(true));
        mock.Setup(x => x.GetPermanentFile(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((filename, taskId) => Task.FromResult(GetFileDto(filename)));
        mock.Setup(x => x.GetPermanentThumbnail(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((filename, taskId) => Task.FromResult(GetThumbnailDto(filename)));
        mock.Setup(x => x.GetTemporaryFileAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((filename, userId) => Task.FromResult(GetFileDto(filename)));
        mock.Setup(x => x.GetTemporaryThumbnailAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((filename, userId) => Task.FromResult(GetThumbnailDto(filename)));
        mock.Setup(x => x.GetListOfTemporaryFile(It.IsAny<string>()))
            .Returns(() =>
            {
                int index = Random.Shared.Next(TempAttachmentsDtoTestObjects.Count);
                int count = TempAttachmentsDtoTestObjects.Count - index;
                return Task.FromResult(TempAttachmentsDtoTestObjects as IEnumerable<AttachmentDto>);
            });
        mock.Setup(x => x.MoveFilesToPermanentAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(() =>
            {
                int index = Random.Shared.Next(TempAttachmentsDtoTestObjects.Count);
                int count = TempAttachmentsDtoTestObjects.Count - index;
                return Task.FromResult(TempAttachmentsDtoTestObjects as IEnumerable<AttachmentDto>);
            });
        
        return mock.Object;
    }

    public static FileDTO? GetFileDto(string filename)
    {
        var fs = new FileStream("TestFiles/json_test_file.json", FileMode.Open);
        return new FileDTO
        {
            FileName = filename,
            ContentType = "application/json",
            SizeKb = fs.Length,
            Extension = ".json",
            LastModified = DateTimeOffset.Now,
            FileStream = fs
        };
    }
    public static FileDTO? GetThumbnailDto(string filename)
    {
        var fs = new FileStream("TestFiles/test_thumbnail_picture.jpeg", FileMode.Open);
        return new FileDTO
        {
            FileName = filename,
            ContentType = "image/jpeg",
            SizeKb = fs.Length,
            Extension = ".jpeg",
            LastModified = DateTimeOffset.Now,
            FileStream = fs
        };
    }
    public static List<AttachmentDto> TempAttachmentsDtoTestObjects = new ()
    {
        new AttachmentDto { FileName = "Document", Extension = ".docx", SizeKb = 1024 },
        new AttachmentDto { FileName = "Image", Extension = ".png", SizeKb = 512 },
        new AttachmentDto { FileName = "Spreadsheet", Extension = ".xlsx", SizeKb = 2048 },
        new AttachmentDto { FileName = "Presentation", Extension = ".pptx", SizeKb = 1536 },
        new AttachmentDto { FileName = "TextFile", Extension = ".txt", SizeKb = 256 },
        new AttachmentDto { FileName = "Video", Extension = ".mp4", SizeKb = 4096 },
        new AttachmentDto { FileName = "Audio", Extension = ".mp3", SizeKb = 768 },
        new AttachmentDto { FileName = "CodeFile", Extension = ".cs", SizeKb = 128 },
        new AttachmentDto { FileName = "PDF", Extension = ".pdf", SizeKb = 3000 },
        new AttachmentDto { FileName = "Presentation2", Extension = ".pptx", SizeKb = 1800 }
    };
}