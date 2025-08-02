namespace BackupTool.Tests;

public class SnapshotStoreTests
{
    [Fact]
    public void Store_And_Restore_Give_Back_Input()
    {
        var snapshotStore = new SnapshotStore(Some.Repository(), Some.Crypto());
        var expected = Some.BlobSystem(Some.Blobs());
        var actual = Some.BlobSystem();

        snapshotStore.Store(expected, Some.UtcNow());
        snapshotStore.Restore(actual);

        Assert.Equal(ToDictionary(expected), ToDictionary(actual));
    }

    [Fact]
    public void Restore_Throws_On_Wrong_Password()
    {
        var repository = Some.Repository();
        var crypto1 = Some.Crypto("some password");
        var crypto2 = new Crypto("other password", crypto1.Salt, crypto1.Iterations);
        var snapshotStore1 = new SnapshotStore(repository, crypto1);
        var snapshotStore2 = new SnapshotStore(repository, crypto2);
        var expected = Some.BlobSystem(Some.Blobs());

        snapshotStore1.Store(expected, Some.UtcNow());

        Assert.Throws<AuthenticationTagMismatchException>(
            () => snapshotStore2.Restore(Some.BlobSystem()));
    }

    [Fact]
    public void Check_Returns_True_If_Data_Is_Valid()
    {
        var snapshotStore = new SnapshotStore(Some.Repository(), Some.Crypto());
        var expected = Some.BlobSystem(Some.Blobs());

        snapshotStore.Store(expected, Some.UtcNow());

        Assert.True(snapshotStore.Check());
    }

    [Fact]
    public void Check_Throws_If_Snapshot_Is_Broken()
    {
        var repository = Some.Repository();
        var snapshotStore = new SnapshotStore(repository, Some.Crypto());
        var expected = Some.BlobSystem(Some.Blobs());

        snapshotStore.Store(expected, Some.UtcNow());

        foreach (var name in repository.List())
        {
            repository.Write(name, new byte[] { 0x00, 0x00 });
        }

        Assert.Throws<JsonException>(
            () => snapshotStore.Check());
    }

    private static Dictionary<BlobData, byte[]> ToDictionary(
        IBlobSystem blobSystem)
    {
        var dict = new Dictionary<BlobData, byte[]>();

        foreach (var blob in blobSystem.List())
        {
            dict[blob] = blobSystem.Read(blob);
        }

        return dict;
    }
}
