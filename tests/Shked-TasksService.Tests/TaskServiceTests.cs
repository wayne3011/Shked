using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using ShkedTasksService.Application.DTO;
using ShkedTasksService.Application.Services;
using ShkedTasksService.DAL.Entities;
using Xunit;

namespace Shked_TasksService.Tests;

public class TaskServiceTests
{
    private Mapper _mapper = new Mapper(new MapperConfiguration(cfg =>
    {
        cfg.CreateMap<AttachmentDto, Attachment>().ReverseMap();
        cfg.CreateMap<TaskDTO, TaskEntity>();
        cfg.CreateMap<TaskEntity, TaskDTO>();
    }));
    private TasksService tasksService;
    
        public TaskServiceTests()
        {
            tasksService =  new TasksService(TaskRepositoryMoq.Create(), _mapper,
                TaskAttachmentStorageApiMoq.Create(), UserApiMoq.Create());
        }
    private DateTime _dateTime = DateTime.Now;
    [Theory]
    [ClassData(typeof(TaskServiceTestData))]
    public async void CreateTaskAsync_ValidTest1(string userId, TaskDTO taskDto, TaskDTO? exceptResult)
    {
        var result = await tasksService.CreateTaskAsync(userId, taskDto);
        if(exceptResult == null) Assert.Null(taskDto);
        else
        {
            exceptResult.Should().BeEquivalentTo(result, opt => opt.Excluding(x => x.Id));
            Assert.Matches("^[{(]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[)}]?$", result.Id);
        }
    }

    [Fact]
    public void UploadTemporaryFileAsync_ValidTest1()
    {
        var fileStream = new FileStream("TestFiles/json_test_file.json", FileMode.Open);
        var thumbnailStream = new FileStream("TestFiles/test_thumbnail_picture.jpeg", FileMode.Open);
        Assert.True(tasksService.UploadTemporaryFileAsync(
            new FormFile(fileStream, 0, fileStream.Length, "file", "json_test_file.json"), 
            new FormFile(thumbnailStream, 0, fileStream.Length, "thumbnail", "test_thumbnail_picture.jpeg"),
            "d54a9842-2c96-4ded-936e-0ecde60ac64b").Result);
        
    }

    [Fact]
    public async void GetListOfTemporaryFile_ValidTest1()
    {
        var result = await tasksService.GetListOfTemporaryFile("d54a9842-2c96-4ded-936e-0ecde60ac64b");
        TaskAttachmentStorageApiMoq.TempAttachmentsDtoTestObjects.Should().BeEquivalentTo(result, 
            cfg => cfg.ExcludingMissingMembers());
    }

    [Theory]
    [ClassData(typeof(GetTasks_ValidTest1_Data))]
    public async void GetTasks_ValidTest1(string userId, IEnumerable<TaskDTO> exceptedResult)
    {
        var result = (await tasksService.GetTasksAsync(userId)).ToList();
        result.Should().BeEquivalentTo(exceptedResult.ToList(), options => options.Excluding(x => x.Deadline)
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, precision: TimeSpan.FromMinutes(1)))
            .When(info => info.Path == "Deadline"));
    }

    [Fact]
    public async void GetPermanentFile_ValidTest1()
    {
        var result = await tasksService.GetPermanentFileAsync("filename", "4cdae903-cbda-4d1d-84ff-8c0cc6eda5fd", "2bd04efb-cb61-4daf-a4ed-4022388da6a6");
        var expected = TaskAttachmentStorageApiMoq.GetFileDto("filename");
        result.Should().BeEquivalentTo(expected, options => options.Excluding(x => x!.LastModified).Excluding(x => x!.FileStream));
        Assert.True((result.LastModified - expected.LastModified).TotalSeconds < 1);
    }
    
    [Fact]
    public async void GetPermanentFile_InvalidTest1()
    {
        var result = await tasksService.GetPermanentFileAsync("filename", "caa77631-e391-443e-96cd-284fd26d6d92", "2bd04efb-cb61-4daf-a4ed-4022388da6a6");
        Assert.Null(result);
    }
    
    [Fact]
    public async void GetPermanentFile_InvalidTest2()
    {
        var result = await tasksService.GetPermanentFileAsync("filename", "11b9cc7c-0f65-425e-886a-0417ba5047a4", "5f1a17be-35f0-4844-8f8a-f0ea621b3d28");
        Assert.Null(result);
    }
    
    [Fact]
    public async void GetPermanentThumbnailFile_ValidTest1()
    {
        var result = await tasksService.GetPermanentThumbnailAsync("filename", "4cdae903-cbda-4d1d-84ff-8c0cc6eda5fd", "2bd04efb-cb61-4daf-a4ed-4022388da6a6");
        var expected = TaskAttachmentStorageApiMoq.GetThumbnailDto("filename");
        result.Should().BeEquivalentTo(expected, options => options.Excluding(x => x!.LastModified).Excluding(x => x!.FileStream));
        Assert.True((result.LastModified - expected.LastModified).TotalSeconds < 1);
    }
    
    [Fact]
    public async void GetPermanentThumbnailFile_InvalidTest1()
    {
        var result = await tasksService.GetPermanentThumbnailAsync("filename", "01265f98-be53-4cb2-adbb-ce07c80f4a3c", "2bd04efb-cb61-4daf-a4ed-4022388da6a6");
        Assert.Null(result);
    }
    
    [Fact]
    public async void GetPermanentThumbnailFile_InvalidTest2()
    {
        var result = await tasksService.GetPermanentThumbnailAsync("filename", "4cdae903-cbda-4d1d-84ff-8c0cc6eda5fd", "8b1acef8-4fe2-4fd8-bc72-13db02bb1ffa");
        Assert.Null(result);
    }
    
    [Fact]
    public async void GetTempFile_ValidTest1()
    {
        var result = await tasksService.GetTemporaryFileAsync("filename", "2bd04efb-cb61-4daf-a4ed-4022388da6a6");
        var expected = TaskAttachmentStorageApiMoq.GetFileDto("filename");
        result.Should().BeEquivalentTo(expected, options => options.Excluding(x => x!.LastModified).Excluding(x => x!.FileStream));
        Assert.True((result.LastModified - expected.LastModified).TotalSeconds < 1);
    }
    
    [Fact]
    public async void GetTempThumbnailFile_ValidTest1()
    {
        var result = await tasksService.GetTemporaryThumbnailAsync("filename", "2bd04efb-cb61-4daf-a4ed-4022388da6a6");
        var expected = TaskAttachmentStorageApiMoq.GetThumbnailDto("filename");
        result.Should().BeEquivalentTo(expected, options => options.Excluding(x => x!.LastModified).Excluding(x => x!.FileStream));
        Assert.True((result.LastModified - expected.LastModified).TotalSeconds < 1);
    }
    
    [Fact]
    public async void DeleteTempFile_ValidTest1()
    {
        var result = await tasksService.DeleteTemporaryFileAsync("filename", "2bd04efb-cb61-4daf-a4ed-4022388da6a6");
        Assert.True(result);
    }

    [Fact]
    async void DeleteTask_ValidTest1()
    {
        var result = await tasksService.DeleteTaskAsync("4cdae903-cbda-4d1d-84ff-8c0cc6eda5fd",
            "2bd04efb-cb61-4daf-a4ed-4022388da6a6");
        Assert.True(result);
    }
}

public class TaskServiceTestData : TheoryData<string, TaskDTO, TaskDTO?>
{
    public TaskServiceTestData()
    {
        DateTime deadline = DateTime.Now.AddDays(10);
        Add(
            "2bd04efb-cb61-4daf-a4ed-4022388da6a6",
            new TaskDTO
            {
                Id = null,
                UserID = null,
                Deadline = deadline,
                IsPublic = true,
                LessonOrdinal = 1,
                Text = "Купить зубную щётку",
                Attachments = null,
                GroupName = "М3О-325Бк-21"
            },
            new TaskDTO
            {
                Id = null,
                UserID = "2bd04efb-cb61-4daf-a4ed-4022388da6a6",
                Deadline = deadline,
                IsPublic = true,
                LessonOrdinal = 1,
                Text = "Купить зубную щётку",
                Attachments = TaskAttachmentStorageApiMoq.Create().GetListOfTemporaryFile("").Result.ToList(),
                GroupName = "М3О-325Бк-21"
            }
        );
    }
}

public class GetTasks_ValidTest1_Data : TheoryData<string, IEnumerable<TaskDTO>>
{
    public GetTasks_ValidTest1_Data()
    {
        Add("2bd04efb-cb61-4daf-a4ed-4022388da6a6", new []
        {
            new TaskDTO()
            {
                GroupName = "М3О-325Бк-21",
                Id = "4cdae903-cbda-4d1d-84ff-8c0cc6eda5fd",
                Deadline = DateTime.Now.AddDays(5),
                IsPublic = true,
                LessonOrdinal = 3,
                Text = "Написать отчет",
                UserID = "2bd04efb-cb61-4daf-a4ed-4022388da6a6",
                Attachments = new List<AttachmentDto>
                {
                    new ()
                    {
                        FileName = "Отчет",
                        Extension = ".pdf",
                        SizeKb = 2048
                    },
                    new ()
                    {
                        FileName = "Презентация",
                        Extension = ".pptx",
                        SizeKb = 4096
                    }
                }
            },
            new TaskDTO()
            {
                GroupName = "М3О-325Бк-21",
                Id = "f6c71cae-2373-4190-88dc-d7353016ddd9",
                Deadline = DateTime.Now.AddDays(-7),
                IsPublic = false,
                LessonOrdinal = 6,
                Text = "Сосатвить доклад",
                UserID = "2bd04efb-cb61-4daf-a4ed-4022388da6a6",
                Attachments = new List<AttachmentDto>()
                {
                    new AttachmentDto
                    {
                        FileName = "Тема",
                        Extension = ".txt",
                        SizeKb = 512
                    },
                    new AttachmentDto
                    {
                        FileName = "Исследование",
                        Extension = ".docx",
                        SizeKb = 8192
                    }
                }
            },
            new TaskDTO()
            {
                GroupName = "М3О-325Бк-21",
                Id = "92ed51ec-4b44-446e-9fd0-f231c67bed69",
                Deadline = DateTime.Now.AddDays(6),
                IsPublic = true,
                LessonOrdinal = 5,
                Text = "Подготовить доклад",
                UserID = "cc9d01f7-b127-49b1-99ba-0d29a48d676c",
                Attachments = new List<AttachmentDto>
                {
                    new()
                    {
                        FileName = "Доклад",
                        Extension = ".docx",
                        SizeKb = 2560
                    },
                    new()
                    {
                        FileName = "Иллюстрации",
                        Extension = ".png",
                        SizeKb = 768
                    },
                    new()
                    {
                        FileName = "Графики",
                        Extension = ".pdf",
                        SizeKb = 2048
                    }
                }
            }
            
        });
    }
}