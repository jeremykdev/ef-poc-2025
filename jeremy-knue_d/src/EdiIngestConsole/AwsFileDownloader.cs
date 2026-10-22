using Amazon.S3;
using Amazon.S3.Transfer;

namespace EdiIngestConsole;

/// <summary>
/// Downloads files from AWS
/// </summary>
/// <remarks>
/// Depends on environment variables: "S3_BUCKET_NAME",  "S3_SERVICE_URL"
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
        if (String.IsNullOrWhiteSpace(this._serviceUrl))
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

        /*
        await transferUtility.DownloadDirectoryAsync(
            bucketName: this._bucketName,
            localDirectory: destinationDirectory

            );

        */

        /*
        AmazonS3Client s3Client = new();

        GetObjectRequest req = new()
        {
            BucketName = this._bucketName,
        };

        */

        // TOOD: finish
    }


    private static bool ValidateIsUrl(string input)
    {
        if (String.IsNullOrWhiteSpace(input))
            return false;

        if (Uri.TryCreate(input, UriKind.Absolute, out var result))
        {
            if (result.Scheme == Uri.UriSchemeHttp ||
                result.Scheme == Uri.UriSchemeHttps
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

