using System.Security.Cryptography;
using System.Text;
using Babel.Core.Interfaces;

namespace Babel.Core.Services;

public class EncryptionService : IEncryptionProvider
{
    private static readonly Lazy<EncryptionService> _lazyInstance = new(() => new EncryptionService());
    public static EncryptionService Instance => _lazyInstance.Value;

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return string.Empty;
        }

        if (OperatingSystem.IsWindows())
        {
            try
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] cipherBytes = ProtectedData.Protect(
                    plainBytes,
                    optionalEntropy: null,
                    scope: DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(cipherBytes);
            }
            catch (PlatformNotSupportedException)
            {
                return FallbackProtect(plainText);
            }
        }
        else
        {
            return FallbackProtect(plainText);
        }
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
        {
            return string.Empty;
        }

        if (OperatingSystem.IsWindows())
        {
            if (cipherText.StartsWith("FALLBACK:", StringComparison.Ordinal))
            {
                return FallbackUnprotect(cipherText);
            }

            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] plainBytes = ProtectedData.Unprotect(
                    cipherBytes,
                    optionalEntropy: null,
                    scope: DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (FormatException ex)
            {
                throw new CryptographicException("Ciphertext is not valid base64 or was tampered.", ex);
            }
            catch (PlatformNotSupportedException)
            {
                return FallbackUnprotect(cipherText);
            }
        }
        else
        {
            return FallbackUnprotect(cipherText);
        }
    }

    private static string FallbackProtect(string plainText)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(plainText);
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] ^= 0x5A;
        }
        return "FALLBACK:" + Convert.ToBase64String(bytes);
    }

    private static string FallbackUnprotect(string cipherText)
    {
        if (cipherText.StartsWith("FALLBACK:", StringComparison.Ordinal))
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(cipherText.Substring("FALLBACK:".Length));
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] ^= 0x5A;
                }
                return Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException ex)
            {
                throw new CryptographicException("Fallback ciphertext is not valid base64 or was tampered.", ex);
            }
        }
        throw new CryptographicException("Ciphertext cannot be decrypted: platform unsupported and fallback header missing.");
    }
}
