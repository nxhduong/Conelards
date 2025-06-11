using System.Security.Cryptography;
using System.Text;

namespace Conelards.Helpers;

class SHA256TextHasher
{
    public static string Hash(string inputString)
    {
        var strBuild = new StringBuilder();

        foreach (byte b in SHA256.HashData(Encoding.UTF8.GetBytes(inputString)))
            strBuild.Append(b.ToString("X2"));

        return strBuild.ToString();
    }
}