namespace EdiIngestConsole;



public class FileMover : IFileMover
{
    private const string _completed = "completed";

    public static void MoveCompletedFile(string filename)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filename);

        if (!Directory.Exists(_completed))
            Directory.CreateDirectory(_completed);

        string extension = Path.GetExtension(filename);
        string baseFilenameNoExt = Path.GetFileNameWithoutExtension(filename);
        string randomFileNamePart = Path.GetRandomFileName().Replace(".", String.Empty);

        string newFilename = String.Concat(baseFilenameNoExt, '-', DateTime.UtcNow.ToString("yyyyMMdd-HHmmss.fffffff"), '-', randomFileNamePart, extension);
        string newFilenameWithPath = Path.Combine(_completed, newFilename);

        File.Move(filename, newFilenameWithPath);
    }
}
public interface IFileMover
{
    static abstract void MoveCompletedFile(string filename);
}