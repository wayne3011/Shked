namespace SkedAuthorization.Application.Extensions;

public static class ListExtensions
{
    public static void ReplaceFirst(this List<string> list, string oldItem, string newItem)
    {
        var index = list.IndexOf(oldItem);
        list[index] = newItem;
    }
}