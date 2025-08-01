namespace BackupTool;

public sealed record BlobData(string Name, DateTime LastWriteTimeUtc);
public sealed record Snapshot(Dictionary<string, BlobData> Blobs, DateTime CreationTimeUtc);
public sealed record class SnapshotData(string ChunkId, byte[] Salt, int Iterations);

public static class Program
{
    public static void Main()
    {
        var source = @"???";
        var repository = @"???";

        CreateSnapshot(source, repository);
    }

    private static void CreateSnapshot(string source, string repository)
    {
        var chunks = Path.Combine(repository, "chunks");
        var salt = RandomNumberGenerator.GetBytes(Crypto.SaltBytes);
        var iterations = Crypto.DefaultIterations;
        var crypto = new Crypto(Prompt.NewPassword(), salt, iterations);
        var snapshotContent = new Dictionary<string, BlobData>();

        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            var blobData = new BlobData(file, File.GetLastWriteTimeUtc(file));
            var blobEncrypted = crypto.Encrypt(File.ReadAllBytes(file));
            var blobChunkId = ChunkId.From(blobEncrypted);
            var blobStored = Path.Combine(chunks, blobChunkId);

            PathUtils.EnsureParent(blobStored);
            File.WriteAllBytes(blobStored, blobEncrypted);

            snapshotContent.Add(blobChunkId, blobData);
        }

        var snapshot = new Snapshot(snapshotContent, DateTime.UtcNow);
        var snapshotEncrypted = crypto.Encrypt(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(snapshot)));
        var snapshotChunkId = ChunkId.From(snapshotEncrypted);
        var snapshotStored = Path.Combine(chunks, snapshotChunkId);

        PathUtils.EnsureParent(snapshotStored);
        File.WriteAllBytes(snapshotStored, snapshotEncrypted);

        var snapshotData = new SnapshotData(snapshotChunkId, salt, iterations);
        File.WriteAllBytes(Path.Combine(repository, "backup"), Encoding.UTF8.GetBytes(JsonSerializer.Serialize(snapshotData)));
    }
}
