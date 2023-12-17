using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Moq;

namespace Shked_StorageService.Tests;

public class S3ClientMoq
{
    public static IAmazonS3 Create()
    {
        var mock = new Mock<IAmazonS3>();
        mock.Setup(x => x.GetObjectAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None)).Returns<string, string, CancellationToken>((bucketName, key, token) =>
        {
            var getObject = new GetObjectResponse()
            {
                BucketName = bucketName,
                LastModified = DateTime.Now,
                ResponseStream = new FileStream("TestFiles/json_test_file.json", FileMode.Open),
                HttpStatusCode = HttpStatusCode.OK
            };
            getObject.Metadata["Content-Type"] = "application/json";
            return Task.FromResult(getObject);
        });
        mock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .Returns<PutObjectRequest, CancellationToken>((request, token) => Task.FromResult(new PutObjectResponse()
            {
                HttpStatusCode = HttpStatusCode.OK,
            }));
        mock.Setup(x => x.CopyObjectAsync(It.IsAny<CopyObjectRequest>(), It.IsAny<CancellationToken>()))
            .Returns<CopyObjectRequest, CancellationToken>((request, token) =>
            Task.FromResult(new CopyObjectResponse()
            {
                HttpStatusCode = HttpStatusCode.OK
            }));
        mock.Setup(x => x.Paginators.ListObjectsV2(It.IsAny<ListObjectsV2Request>()))
            .Returns<ListObjectsV2Request>((request) => new S3ObjectPaginatorMoq());
        mock.Setup(x => x.ListObjectsAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
            .Returns<string, string, CancellationToken>((s,prefix, token) => Task.FromResult(new ListObjectsResponse()
            {
                S3Objects = tempFiles,
                HttpStatusCode = HttpStatusCode.OK
            }));
        mock.Setup(x => x.DeleteObjectsAsync(It.IsAny<DeleteObjectsRequest>(), CancellationToken.None))
            .Returns<DeleteObjectsRequest, CancellationToken>((x, z) => Task.FromResult(new DeleteObjectsResponse()
            {
                HttpStatusCode = HttpStatusCode.OK
            }));
        return mock.Object;
    }
    
    public static IAmazonS3 CreateInvalid()
    {
        var mock = new Mock<IAmazonS3>();
        mock.Setup(x => x.GetObjectAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
            .Throws(new SocketException());
        mock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .Returns<PutObjectRequest, CancellationToken>((request, token) => Task.FromResult(new PutObjectResponse()
            {
                HttpStatusCode = HttpStatusCode.BadRequest,
            }));
        mock.Setup(x => x.CopyObjectAsync(It.IsAny<CopyObjectRequest>(), It.IsAny<CancellationToken>()))
            .Returns<CopyObjectRequest, CancellationToken>((request, token) =>
            Task.FromResult(new CopyObjectResponse()
            {
                HttpStatusCode = HttpStatusCode.OK
            }));
        mock.Setup(x => x.Paginators.ListObjectsV2(It.IsAny<ListObjectsV2Request>()))
            .Returns<ListObjectsV2Request>((request) => new S3ObjectPaginatorMoq());
        mock.Setup(x => x.ListObjectsAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
            .Returns<string, string, CancellationToken>((s,prefix, token) => Task.FromResult(new ListObjectsResponse()
            {
                S3Objects = tempFiles,
                HttpStatusCode = HttpStatusCode.OK
            }));
        mock.Setup(x => x.DeleteObjectsAsync(It.IsAny<DeleteObjectsRequest>(), CancellationToken.None))
            .Throws(new SocketException());
        return mock.Object;
    }

    public static IAmazonS3 CreateInvalidMiniatureLoadingAmazonS3Moq()
    {
        var mock = new Mock<IAmazonS3>();
        mock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .Returns<PutObjectRequest, CancellationToken>((request, token) => Task.FromResult(new PutObjectResponse()
            {
                HttpStatusCode = Regex.Match(request.Key, "THUMBNAILS").Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest,
            }));
        return mock.Object;
    }

    public static IAmazonS3 CreateThrowableAmazonS3Moq()
    {
        var mock = new Mock<IAmazonS3>();
        mock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .Throws(new SocketException());
        return mock.Object;
    }
    public static IAmazonS3 CreateThrowableForFileUploadsAmazonS3Moq()
    {
        var mock = new Mock<IAmazonS3>();
        mock.Setup(x => x.PutObjectAsync(It.Is<PutObjectRequest>(request => !Regex.Match(request.Key, "THUMBNAILS").Success), It.IsAny<CancellationToken>()))
            .Throws(new SocketException());
        mock.Setup(x => x.PutObjectAsync(It.Is<PutObjectRequest>(request => Regex.Match(request.Key, "THUMBNAILS").Success), It.IsAny<CancellationToken>()))
            .Returns<PutObjectRequest,CancellationToken>((request, token) => Task.FromResult(new PutObjectResponse()
            {
                HttpStatusCode = HttpStatusCode.OK
            }));
        return mock.Object;
    }


    static async IAsyncEnumerable<ListObjectsV2Response> AsyncTempLists()
    {

            ListObjectsV2Response listObjectsV2Response = new ListObjectsV2Response();
            listObjectsV2Response.S3Objects.AddRange(tempFiles);
            yield return listObjectsV2Response;
        
    }
    private static List<S3Object> tempFiles = new()
    {
        new()
        {
            BucketName = "shked-tasks",
            Key = "TEMP/2bd04efb-cb61-4daf-a4ed-4022388da6a6/json_test_file.json",
            LastModified = DateTime.Now,
            Size = 1040
        },
        new()
        {
            BucketName = "shked-tasks",
            Key = "TEMP/2bd04efb-cb61-4daf-a4ed-4022388da6a6/THUMBNAILS/json_test_file.json",
            LastModified = DateTime.Now,
            Size = 1040
        },
    };
    class S3ObjectPaginatorMoq : IListObjectsV2Paginator
    {
        public IPaginatedEnumerable<ListObjectsV2Response> Responses { get => new PaginatedObjects(); }
        public IPaginatedEnumerable<S3Object> S3Objects { get ; }
        public IPaginatedEnumerable<string> CommonPrefixes { get; }
    } 
    public class PaginatedObjects : List<S3Object>, IPaginatedEnumerable<ListObjectsV2Response>
    {
        public string NextToken { get; set; } // Можно добавить свойство для управления токеном продолжения
        public IAsyncEnumerator<ListObjectsV2Response> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            return AsyncTempLists().GetAsyncEnumerator();
        }
    }
}
