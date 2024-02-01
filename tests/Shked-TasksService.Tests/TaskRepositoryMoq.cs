using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using Moq;
using ShkedTasksService.DAL.Entities;
using ShkedTasksService.DAL.Infrastructure;

namespace Shked_TasksService.Tests;

public class TaskRepositoryMoq
{
private static List<TaskEntity> _taskEntities = new List<TaskEntity>
{
    new TaskEntity
    {
        GroupName = "М3О-325Бк-21",
        Id = "4cdae903-cbda-4d1d-84ff-8c0cc6eda5fd",
        Deadline = DateTime.Now.AddDays(5),
        IsPublic = true,
        LessonOrdinal = 3,
        PublishTime = DateTime.Now,
        Text = "Написать отчет",
        UserID = "2bd04efb-cb61-4daf-a4ed-4022388da6a6",
        Attachments = new List<Attachment>()
        {
            new Attachment
            {
                FileName = "Отчет",
                Extension = ".pdf",
                SizeKb = 2048
            },
            new Attachment
            {
                FileName = "Презентация",
                Extension = ".pptx",
                SizeKb = 4096
            }
        }
    },
    new TaskEntity
    {
        GroupName = "М3О-325Бк-21",
        Id = "f6c71cae-2373-4190-88dc-d7353016ddd9",
        Deadline = DateTime.Now.AddDays(-7),
        IsPublic = false,
        LessonOrdinal = 6,
        PublishTime = DateTime.Now,
        Text = "Сосатвить доклад",
        UserID = "2bd04efb-cb61-4daf-a4ed-4022388da6a6",
        Attachments = new List<Attachment>()
        {
            new Attachment
            {
                FileName = "Тема",
                Extension = ".txt",
                SizeKb = 512
            },
            new Attachment
            {
                FileName = "Исследование",
                Extension = ".docx",
                SizeKb = 8192
            }
        }
    },
    new TaskEntity
    {
        GroupName = "М3О-319Бк-21",
        Id = "faf9095c-6b3b-4fbf-abde-fd3ecbe11708\n",
        Deadline = DateTime.Now.AddDays(3),
        IsPublic = true,
        LessonOrdinal = 1,
        PublishTime = DateTime.Now,
        Text = "Подготовить презентацию",
        UserID = "a8a00f35-b4da-4292-b60c-b4fff73ab348",
        Attachments = new List<Attachment>()
        {
            new Attachment
            {
                FileName = "Презентация",
                Extension = ".pptx",
                SizeKb = 3072
            },
            new Attachment
            {
                FileName = "Иллюстрации",
                Extension = ".jpg",
                SizeKb = 1024
            },
            new Attachment
            {
                FileName = "Таблица",
                Extension = ".xlsx",
                SizeKb = 2048
            }
        }
    },
    new TaskEntity
    {
        GroupName = "М3О-319Бк-21",
        Id = "11b9cc7c-0f65-425e-886a-0417ba5047a4",
        Deadline = DateTime.Now.AddDays(4),
        IsPublic = false,
        LessonOrdinal = 4,
        PublishTime = DateTime.Now,
        Text = "Провести эксперимент",
        UserID = "a8a00f35-b4da-4292-b60c-b4fff73ab348",
        Attachments = new List<Attachment>()
        {
            new Attachment
            {
                FileName = "Описание",
                Extension = ".doc",
                SizeKb = 1536
            },
            new Attachment
            {
                FileName = "Результаты",
                Extension = ".pdf",
                SizeKb = 4096
            }
        }
    },
    new TaskEntity
    {
        GroupName = "М3О-325Бк-21",
        Id = "92ed51ec-4b44-446e-9fd0-f231c67bed69",
        Deadline = DateTime.Now.AddDays(6),
        IsPublic = true,
        LessonOrdinal = 5,
        PublishTime = DateTime.Now,
        Text = "Подготовить доклад",
        UserID = "cc9d01f7-b127-49b1-99ba-0d29a48d676c",
        Attachments = new List<Attachment>
        {
            new Attachment
            {
                FileName = "Доклад",
                Extension = ".docx",
                SizeKb = 2560
            },
            new Attachment
            {
                FileName = "Иллюстрации",
                Extension = ".png",
                SizeKb = 768
            },
            new Attachment
            {
                FileName = "Графики",
                Extension = ".pdf",
                SizeKb = 2048
            }
        }
    }
};

    public static ITaskRepository Create()
    {
        var mock = new Mock<ITaskRepository>();
        mock.Setup(obj => obj.FindAsync(It.IsAny<string>()))
            .Returns<string>(obj => Task.FromResult(_taskEntities.Find(x => x.Id == obj)));
        mock.Setup(obj => obj.CreateAsync(It.IsAny<TaskEntity>()))
            .Returns(() => Task.FromResult(true));
        mock.Setup(obj => obj.DeleteAsync(It.IsAny<string>()))
            .Returns(() => Task.FromResult(true));
        mock.Setup(obj => obj.GetActualTasks(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((groupName, userId) => Task.FromResult(_taskEntities.Where(task =>
                task.GroupName == groupName
                && ((task.UserID == userId && !task.IsPublic) || task.IsPublic))));
        return mock.Object;
    }
}