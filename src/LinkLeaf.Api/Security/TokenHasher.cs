using System.Security.Cryptography;
using System.Text;

namespace LinkLeaf.Api.Security;

public class TokenHasher : ITokenHasher
{
    public string Hash(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public bool Verify(string rawToken, string hashedToken)
    {
        var computed = Hash(rawToken);
        return computed == hashedToken;
    }
}
