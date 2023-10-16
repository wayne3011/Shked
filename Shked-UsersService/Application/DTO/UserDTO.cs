namespace ShkedUsersService.Application.DTO;


public class UserDTO
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Group { get; set; }
    public IEnumerable<FriendGroup> FriendGroups { get; set; }
}

public class FriendGroup
{
    public string Title { get; set; }
    public string Group { get; set; }
}