using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static string ToHex(byte[] data)
    {
        var sb = new StringBuilder(data.Length * 2);
        foreach (byte b in data)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    static byte[] EncryptAes(string plainText, byte[] key, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var ms = new MemoryStream();
        using var encryptor = aes.CreateEncryptor();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        cs.Write(plainBytes, 0, plainBytes.Length);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }

    static string DecryptAes(byte[] cipherText, byte[] key, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var ms = new MemoryStream(cipherText);
        using var decryptor = aes.CreateDecryptor();
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.UTF8);
        return sr.ReadToEnd();
    }

    static byte[] ComputeSHA512(string input)
    {
        return SHA512.HashData(Encoding.UTF8.GetBytes(input));
    }

    static bool VerifyIntegrity(string message, byte[] storedHash)
    {
        byte[] currentHash = ComputeSHA512(message);
        return CryptographicOperations.FixedTimeEquals(currentHash, storedHash);
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        const string message = "Charadnichenka";

        Console.WriteLine("=== AES-256 ===");
        using Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        aes.GenerateIV();

        byte[] key = aes.Key;
        byte[] iv = aes.IV;
        byte[] encrypted = EncryptAes(message, key, iv);
        string decrypted = DecryptAes(encrypted, key, iv);

        File.WriteAllBytes("aes_key.bin", key);
        File.WriteAllBytes("aes_iv.bin", iv);
        File.WriteAllBytes("encrypted.bin", encrypted);

        Console.WriteLine($"Исходное сообщение: {message}");
        Console.WriteLine($"Ключ (HEX): {ToHex(key)}");
        Console.WriteLine($"IV (HEX): {ToHex(iv)}");
        Console.WriteLine($"Шифротекст (HEX): {ToHex(encrypted)}");
        Console.WriteLine($"Расшифрованное сообщение: {decrypted}");

        Console.WriteLine("\n=== SHA512 ===");
        byte[] hash = ComputeSHA512(message);
        string hashHex = ToHex(hash);
        File.WriteAllText("hash.txt", hashHex, Encoding.UTF8);

        Console.WriteLine($"SHA512 (HEX): {hashHex}");

        Console.WriteLine("\n=== Проверка целостности ===");
        bool ok1 = VerifyIntegrity(message, hash);
        Console.WriteLine($"Исходное сообщение: {(ok1 ? "ЦЕЛОСТНОСТЬ ПОДТВЕРЖДЕНА" : "ЦЕЛОСТНОСТЬ НАРУШЕНА")}");

        string tamperedMessage = "Charadnichenka1";
        bool ok2 = VerifyIntegrity(tamperedMessage, hash);
        Console.WriteLine($"Изменённое сообщение: {(ok2 ? "ЦЕЛОСТНОСТЬ ПОДТВЕРЖДЕНА" : "ЦЕЛОСТНОСТЬ НАРУШЕНА")}");

        byte[] tamperedHash = (byte[])hash.Clone();
        tamperedHash[0] ^= 0xFF;
        bool ok3 = VerifyIntegrity(message, tamperedHash);
        Console.WriteLine($"Изменённый хеш: {(ok3 ? "ЦЕЛОСТНОСТЬ ПОДТВЕРЖДЕНА" : "ЦЕЛОСТНОСТЬ НАРУШЕНА")}");
    }
}
