
using MongoDB.Bson.Serialization.Attributes;

namespace ShkedTasksService.DAL.Entities;

public class TaskEntity
{
    [BsonId]
    public string Id { get; set; }
    public string UserID { get; set; }
    public DateTime PublishTime { get; set; }
    public DateTime Deadline { get; set; }
    public bool IsPublic { get; set; }
    public int LessonOrdinal { get; set; }
    public string Text { get; set; }
    public string GroupName { get; set; }
    public IEnumerable<string> Attachments { get; set; }
}