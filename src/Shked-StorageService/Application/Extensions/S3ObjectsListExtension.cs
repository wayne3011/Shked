using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Amazon.S3.Model;

namespace ShkedStorageService.Application.Extensions;

public static class S3ObjectsListExtension
{
    public static List<S3Object> WithoutThumbnails([NotNull] this List<S3Object> source, string ThumbnailsFolderName)
    {
        return source.Where(x => !Regex.IsMatch(x.Key, $"TEMP/.*/{ThumbnailsFolderName}.*")).ToList();
    }
}