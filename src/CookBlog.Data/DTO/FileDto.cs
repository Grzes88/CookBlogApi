namespace CookBlog.Core.DTO;

public class FileDto
{
    public string FileName { get; }
    public string ContentType { get; }
    public byte[] FileContents { get; }

    public FileDto(byte[] fileContents, string fileName, string contentType)
    {
        FileContents = fileContents;
        FileName = fileName;
        ContentType = contentType;
    }
}
