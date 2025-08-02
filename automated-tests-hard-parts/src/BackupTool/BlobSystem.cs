namespace BackupTool;

public interface IBlobSystem
{
    BlobData[] List();

    byte[] Read(BlobData blob);

    void Write(BlobData blob, byte[] content);
}

public sealed class FileBlobSystem : IBlobSystem
{
    private readonly string _directory;

    public FileBlobSystem(string directory)
    {
        _directory = directory;
    }

    public BlobData[] List()
    {
        return Directory.GetFiles(_directory, "*", SearchOption.AllDirectories)
            .Select(f => new BlobData(
                Path.GetRelativePath(_directory, f),
                File.GetLastWriteTimeUtc(f)))
            .ToArray();
    }

    public byte[] Read(BlobData blob)
    {
        return File.ReadAllBytes(
            Path.Combine(_directory, blob.Name));
    }

    public void Write(BlobData blob, byte[] content)
    {
        var path = Path.Combine(_directory, blob.Name);

        PathUtils.EnsureParent(path);
        File.WriteAllBytes(path, content);
        File.SetLastWriteTimeUtc(path, blob.LastWriteTimeUtc);
    }
}

public class MemoryBlobSystem : IBlobSystem
{
    private readonly Dictionary<BlobData, byte[]> _values = new();

    public BlobData[] List()
    {
        return _values.Keys.ToArray();
    }

    public byte[] Read(BlobData blob)
    {
        return _values[blob];
    }

    public void Write(BlobData blob, byte[] content)
    {
        _values[blob] = content;
    }
}

public class LoggingBlobSystem : IBlobSystem
{
    private readonly IBlobSystem _blobSystem;

    public LoggingBlobSystem(IBlobSystem blobSystem)
    {
        _blobSystem = blobSystem;
    }

    public BlobData[] List()
    {
        return _blobSystem.List();
    }

    public byte[] Read(BlobData blob)
    {
        Console.WriteLine($"Processing {blob.Name}");

        return _blobSystem.Read(blob);
    }

    public void Write(BlobData blob, byte[] content)
    {
        _blobSystem.Write(blob, content);
    }
}

public sealed class DryRunBlobSystem : IBlobSystem
{
    private readonly IBlobSystem _blobSystem;

    public DryRunBlobSystem(IBlobSystem blobSystem)
    {
        _blobSystem = blobSystem;
    }

    public BlobData[] List()
    {
        return _blobSystem.List();
    }

    public byte[] Read(BlobData blob)
    {
        return _blobSystem.Read(blob);
    }

    public void Write(BlobData blob, byte[] content)
    {
        // We do nothing on purpose
    }
}
