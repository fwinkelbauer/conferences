namespace BackupTool.Tests;

public static class Some
{
    private static DateTime Clock = DateTime.UtcNow;

    public static DateTime UtcNow()
    {
        Clock = Clock.AddSeconds(1);

        return Clock;
    }


    public static Crypto Crypto(string? password = null)
    {
        var iterations = BackupTool.Crypto.DefaultIterations;

        return new Crypto(
            password ?? "super-secret-password",
            RandomNumberGenerator.GetBytes(BackupTool.Crypto.SaltBytes),
            iterations);
    }

    public static BlobData Blob(string blobName)
    {
        return new BlobData(blobName, UtcNow());
    }

    public static BlobData[] Blobs(params string[] blobNames)
    {
        blobNames = blobNames.Length > 0
            ? blobNames
            : new[] { "blob 1", "dir/blob-2" };

        return blobNames
            .Select(Blob)
            .ToArray();
    }

    public static IRepository Repository()
    {
        return new MemoryRepository();
    }

    public static IBlobSystem BlobSystem(
        IEnumerable<BlobData>? blobs = null,
        Func<string, byte[]>? generator = null)
    {
        blobs ??= Array.Empty<BlobData>();
        generator ??= Encoding.UTF8.GetBytes;

        var blobSystem = new MemoryBlobSystem();

        foreach (var blob in blobs)
        {
            blobSystem.Write(blob, generator(blob.Name));
        }

        return blobSystem;
    }

    public static string Directory()
    {
        return Path.Combine(
            Path.GetTempPath(),
            "backup-tool-test",
            Path.GetRandomFileName());
    }
}
