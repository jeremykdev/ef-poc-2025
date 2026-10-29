using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Azure.Core;
using System.Collections.Concurrent;

namespace EdiIngestConsole;

/// <summary>
/// Downloads files from AWS
/// </summary>
/// <remarks>
/// Depends on environment variables: 
/// S3_BUCKET_NAME - specifies bucket name
/// S3_SERVICE_URL - specifies URL to connect to 
/// AWS_ACCESS_KEY_ID - aws credential
/// AWS_SECRET_ACCESS_KEY - aws credential
/// </remarks>
public class AwsFileDownloader : IFileDownloader, IDisposable
{
    /// <summary>
    /// Bucket name
    /// </summary>
    /// <remarks>Depends on environment variable named "S3_BUCKET_NAME"</remarks>
    private readonly string? _bucketName;

    /// <summary>
    /// AWS Service URL
    /// </summary>
    /// <remarks>Depends on environment variable named "S3_SERVICE_URL"</remarks>
    private readonly string? _serviceUrl;

    private AmazonS3Client _s3Client;

    public AwsFileDownloader()
    {
        this._bucketName = Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
        this._serviceUrl = Environment.GetEnvironmentVariable("S3_SERVICE_URL");
        this._s3Client = this.CreateS3Client();
    }


    private AmazonS3Client CreateS3Client()
    {
        if (this._serviceUrl is null)
            throw new Exception("S3 Service URL not set");

        if (!ValidateIsUrl(this._serviceUrl))
            throw new Exception($"Service URL is invalid: {this._serviceUrl}");

        AmazonS3Config s3Config = new()
        {
            ServiceURL = this._serviceUrl!
        };

        return new(s3Config);
    }


    private TransferUtility CreateTransferUtility() => new(this._s3Client);


    public async Task DownloadFiles(string destinationDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);

        // check for bucket name, cannot proceed if not set
        if (String.IsNullOrWhiteSpace(this._bucketName))
            throw new Exception("S3 bucket name not set");

        if (!Directory.Exists(destinationDirectory))
            Directory.CreateDirectory(destinationDirectory);

        TransferUtility transferUtility = this.CreateTransferUtility();

        var response = await transferUtility.DownloadDirectoryWithResponseAsync(
                bucketName: this._bucketName,
                localDirectory: destinationDirectory,
                s3Directory: @"/"
            );

        Console.WriteLine($"Download response: {response.Result}");
    }


    public async Task<List<S3Object>> List3Objects()
    {
        ListObjectsV2Request listRequest = new()
        {
            BucketName = this._bucketName,
        };

        var paginator = this._s3Client.Paginators.ListObjectsV2(listRequest);

        ConcurrentBag<S3Object> bag = [];

        await foreach (var response in paginator.Responses)
        {
            foreach (var obj in response.S3Objects)
                bag.Add(obj);
        }

        return bag.ToList();
    }


    public async Task<Stream> GetStream(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        GetObjectRequest objectRequest = new()
        {
            BucketName = this._bucketName,
            Key = key
        };

        var response = await this._s3Client.GetObjectAsync(objectRequest);

        // the Amazon stream does not support seek, copy to memory stream
        using var responseStream = response.ResponseStream;

        var memStream = new MemoryStream();
        responseStream.CopyTo(memStream);
        memStream.Seek(0, SeekOrigin.Begin);
        return memStream;
    }

    private static bool ValidateIsUrl(string? input)
    {
        if (Uri.TryCreate(input, UriKind.Absolute, out var url))
        {
            if (url.Scheme == Uri.UriSchemeHttp ||
                url.Scheme == Uri.UriSchemeHttps
                )
                return true;
        }

        return false;
    }


    public void Dispose()
    {
        this._s3Client?.Dispose();
    }
}


public interface IFileDownloader
{
    Task DownloadFiles(string destinationDirectory);

    Task<List<S3Object>> List3Objects();

    Task<Stream> GetStream(string key);
}