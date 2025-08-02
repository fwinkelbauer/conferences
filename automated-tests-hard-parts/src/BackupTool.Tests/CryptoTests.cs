namespace BackupTool.Tests;

public class CryptoTests
{
    [Fact]
    public void Encrypt_And_Decrypt_Return_Input()
    {
        var salt = RandomNumberGenerator.GetBytes(Crypto.SaltBytes);
        var iterations = Crypto.DefaultIterations;
        var crypto = new Crypto("asdf", salt, iterations);
        byte[] plain = Encoding.UTF8.GetBytes("Hello world");

        var encrypted = crypto.Encrypt(plain);
        var decrypt = crypto.Decrypt(encrypted);

        Assert.Equal(plain, decrypt);
    }

    [Fact]
    public void Encrypt_Gives_Different_Result_For_Different_Salt()
    {
        var crypto1 = new Crypto(
            "asdf",
            RandomNumberGenerator.GetBytes(Crypto.SaltBytes),
            Crypto.DefaultIterations);

        var crypto2 = new Crypto(
            "asdf",
            RandomNumberGenerator.GetBytes(Crypto.SaltBytes),
            Crypto.DefaultIterations);

        var encrypted1 = crypto1.Encrypt("Hello world"u8);
        var encrypted2 = crypto2.Encrypt("Hello world"u8);

        Assert.NotEqual(encrypted1, encrypted2);
    }

    [Fact]
    public void Decrypt_Throws_If_Wrong_Password()
    {
        var salt = RandomNumberGenerator.GetBytes(Crypto.SaltBytes);
        var iterations = Crypto.DefaultIterations;

        var crypto1 = new Crypto("something", salt, iterations);
        var crypto2 = new Crypto("else", salt, iterations);

        var encrypted = crypto1.Encrypt("Hello world"u8);

        Assert.Throws<AuthenticationTagMismatchException>(
            () => crypto2.Decrypt(encrypted));
    }
}
