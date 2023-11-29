namespace ShkedTasksService.Application.Extensions;

public static class ArrayExtensions
{
    public static T? TakeLastIfNotOnly<T>(this T[] array) where T : class
    {
        if (array.Length is 0 or 1) return null;
        return array.Last();
    }
    
}