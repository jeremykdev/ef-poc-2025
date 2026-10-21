namespace EdiIngestConsole;



public class FileMover : IFileMover
{
    private const string _completed = "completed";

    public string MoveCompletedFile(string filename)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filename);

        DateTime utcNow = DateTime.UtcNow;

        string destinationDirectory = Path.Combine(_completed, utcNow.ToString("yyyy-MM-dd"));

        if (!Directory.Exists(destinationDirectory))
            Directory.CreateDirectory(destinationDirectory);

        string destinationFilename = Path.GetFileName(filename);
        string destinationFilenameWithPath = Path.Combine(destinationDirectory, destinationFilename);

        // when file with same name already exists in destination create a new filename
        if (File.Exists(destinationFilenameWithPath))
        {
            string extension = Path.GetExtension(filename);
            string baseFilenameNoExt = Path.GetFileNameWithoutExtension(filename);
            string randomFileNamePart = Path.GetRandomFileName().Replace(".", String.Empty);
            string newFilename = String.Concat(baseFilenameNoExt, '-', utcNow.ToString("yyyyMMdd-HHmmss.fffffff"), '-', randomFileNamePart, extension);
            
            destinationFilenameWithPath = Path.Combine(destinationDirectory, newFilename);
        }

        File.Move(filename, destinationFilenameWithPath);
        return destinationFilenameWithPath;
    }
}
public interface IFileMover
{
    string MoveCompletedFile(string filename);
}