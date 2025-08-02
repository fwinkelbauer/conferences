namespace BackupTool;

public sealed record BlobData(string Name, DateTime LastWriteTimeUtc);
public sealed record Snapshot(Dictionary<string, BlobData> Blobs, DateTime CreationTimeUtc);
public sealed record class SnapshotData(string ChunkId, byte[] Salt, int Iterations);

public static class Program
{
    public static void Main()
    {
        var repository = new DryRunRepository(
            new FileRepository(@"???"));

        var crypto = new Crypto(
            Prompt.NewPassword(),
            RandomNumberGenerator.GetBytes(Crypto.SaltBytes),
            Crypto.DefaultIterations);

        var blobSystem = new DryRunBlobSystem(
            new LoggingBlobSystem(
                new FileBlobSystem(@"???")));

        new SnapshotStore(repository, crypto)
            .Store(blobSystem, DateTime.UtcNow);
    }
}
