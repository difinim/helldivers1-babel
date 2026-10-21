using System.Security.Cryptography;
using Babel.Core.Interfaces;
using Babel.Core.Models;
using Babel.Core.Services;

namespace Babel.Core.Tests;

public class EncryptionTests
{
    [Fact]
    public void EncryptionService_Roundtrip_EncryptsAndDecryptsSuccessfully()
    {
        var service = new EncryptionService();
        string secret = "AIzaSyD-SecretApiKeyForTesting123456789";

        string encrypted = service.Encrypt(secret);

        Assert.False(string.IsNullOrWhiteSpace(encrypted));
        Assert.NotEqual(secret, encrypted);

        string decrypted = service.Decrypt(encrypted);
        Assert.Equal(secret, decrypted);
    }

    [Fact]
    public void EncryptionService_EmptyOrNullInput_HandlesSafely()
    {
        var service = new EncryptionService();

        Assert.Equal(string.Empty, service.Encrypt(""));
        Assert.Equal(string.Empty, service.Encrypt(null!));
        Assert.Equal(string.Empty, service.Decrypt(""));
        Assert.Equal(string.Empty, service.Decrypt(null!));
        Assert.Equal(string.Empty, service.Decrypt("   "));
    }

    [Fact]
    public void EncryptionService_TamperedCiphertextBytes_ThrowsCryptographicException()
    {
        var service = new EncryptionService();
        string secret = "AIzaSyTamperTestKey987654321";
        string encrypted = service.Encrypt(secret);

        // Tamper with the raw ciphertext bytes
        byte[] rawBytes = Convert.FromBase64String(encrypted);
        rawBytes[rawBytes.Length / 2] ^= 0xFF;
        string tamperedBase64 = Convert.ToBase64String(rawBytes);

        Assert.Throws<CryptographicException>(() => service.Decrypt(tamperedBase64));
    }

    [Fact]
    public void EncryptionService_InvalidBase64_ThrowsCryptographicException()
    {
        var service = new EncryptionService();
        string invalidBase64 = "!!!ThisIsNotValidBase64String$$$";

        Assert.Throws<CryptographicException>(() => service.Decrypt(invalidBase64));
    }

    [Fact]
    public void ConfigurationService_PlainTextIsNeverPersistedToDisk()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"babel_enc_test_{Guid.NewGuid():N}.json");
        try
        {
            var configService = new ConfigurationService(tempFile);
            var config = new BabelConfig();
            string secretKey = "AIzaSySuperSecretKeyNeverPersistedInPlaintext";
            config.GeminiApiKey = secretKey;

            configService.Save(config);

            Assert.True(File.Exists(tempFile));

            string savedJson = File.ReadAllText(tempFile);

            // Plain text must NEVER be found in the file
            Assert.DoesNotContain(secretKey, savedJson);

            // Encrypted property must be present in JSON
            Assert.Contains("GeminiApiKeyEncrypted", savedJson);

            // Plain text property name should not be serialized
            Assert.DoesNotContain("\"GeminiApiKey\":", savedJson);

            // Loading the saved file restores the key in runtime memory
            var loaded = configService.Load();
            Assert.Equal(secretKey, loaded.GeminiApiKey);
            Assert.Equal(secretKey, loaded.ApiKeys["Gemini"]);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public void ConfigurationService_LegacyPlainTextMigration_MigratesToEncryptedAndClearsPlainText()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"babel_mig_test_{Guid.NewGuid():N}.json");
        try
        {
            string legacyKey = "AIzaSyLegacyKeyToBeMigrated12345";
            string legacyJson = $$"""
            {
              "ActiveProvider": "Gemini",
              "GeminiApiKey": "{{legacyKey}}",
              "TargetLanguage": "Spanish"
            }
            """;

            File.WriteAllText(tempFile, legacyJson);

            var configService = new ConfigurationService(tempFile);

            // Loading reads legacy plain text key
            var loaded = configService.Load();
            Assert.Equal(legacyKey, loaded.GeminiApiKey);
            Assert.Equal(legacyKey, loaded.ApiKeys["Gemini"]);

            // Saving must encrypt the key and clear plain text
            configService.Save(loaded);

            string reSavedJson = File.ReadAllText(tempFile);
            Assert.DoesNotContain(legacyKey, reSavedJson);
            Assert.Contains("GeminiApiKeyEncrypted", reSavedJson);
            Assert.DoesNotContain("\"GeminiApiKey\":", reSavedJson);

            // Subsequent load decrypts properly from encrypted property
            var reloaded = configService.Load();
            Assert.Equal(legacyKey, reloaded.GeminiApiKey);
            Assert.Equal(legacyKey, reloaded.ApiKeys["Gemini"]);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public void ConfigurationService_CustomEncryptionProviderStub_WorksCorrectly()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"babel_stub_test_{Guid.NewGuid():N}.json");
        try
        {
            var stubProvider = new TestEncryptionStub();
            var configService = new ConfigurationService(tempFile, stubProvider);

            var config = new BabelConfig();
            config.GeminiApiKey = "AIzaSyCustomStubKey";

            configService.Save(config);

            string fileText = File.ReadAllText(tempFile);
            Assert.Contains("STUB_ENC_", fileText);
            Assert.DoesNotContain("AIzaSyCustomStubKey", fileText);

            var loaded = configService.Load();
            Assert.Equal("AIzaSyCustomStubKey", loaded.GeminiApiKey);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    private class TestEncryptionStub : IEncryptionProvider
    {
        public string Encrypt(string plainText) => $"STUB_ENC_{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainText))}";
        public string Decrypt(string cipherText) => cipherText.StartsWith("STUB_ENC_")
            ? System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cipherText.Substring("STUB_ENC_".Length)))
            : cipherText;
    }
}
