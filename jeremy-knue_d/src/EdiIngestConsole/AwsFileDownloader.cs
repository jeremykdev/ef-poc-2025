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

    private TransferUtility CreateTransferUtility()
    {
        if (this._serviceUrl is null)
            throw new Exception("S3 Service URL not set");

        if (!ValidateIsUrl(this._serviceUrl))
            throw new Exception($"Service URL is invalid: {this._serviceUrl}");

        AmazonS3Config s3Config = new()
        {
            ServiceURL = this._serviceUrl!
        };

        AmazonS3Client s3Client = new(s3Config);

        TransferUtility transferUtility = new(s3Client);

        return transferUtility;
    }

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

