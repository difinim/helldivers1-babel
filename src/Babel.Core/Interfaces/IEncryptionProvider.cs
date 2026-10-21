namespace Babel.Core.Interfaces;

public interface IEncryptionProvider
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
