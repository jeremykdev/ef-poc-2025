using Amazon.S3;
using Amazon.S3.Transfer;

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
public class AwsFileDownloader : IFileDownloader
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

    public AwsFileDownloader()
    {
        this._bucketName = Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
        this._serviceUrl = Environment.GetEnvironmentVariable("S3_SERVICE_URL");
    }


    private AmazonS3Client CreateS3Client()
    {
        AmazonS3Config s3Config = new()
        {
            ServiceURL = this._serviceUrl!
        };

        return new(s3Config);        
    }


    private TransferUtility CreateTransferUtility()
    {
        if (this._serviceUrl is null)
            throw new Exception("S3 Service URL not set");

        if (!ValidateIsUrl(this._serviceUrl))
            throw new Exception($"Service URL is invalid: {this._serviceUrl}");

        var s3Client = this.CreateS3Client();
       
        TransferUtility transferUtility = new(s3Client);

        return transferUtility;
    }

    // TODO: add method to get listing of objects/files in bucket

    // TODO: add method to get object/file from bucket as stream

    // TODO: support IDispose for AmazonS3Client 
    // may need to make instance a class level variable and mark class as IDispose then dispose of client when instance disposed

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
}


public interface IFileDownloader
{
    public Task DownloadFiles(string destinationDirectory);
}

