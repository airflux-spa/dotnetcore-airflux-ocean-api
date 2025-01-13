using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class EncryptionHelper
{
    private static readonly string EncryptionKey = "X1c2Y3z4V5w6L7k8N9j0F1g2T3h4B5v8"; // Clave de 32 caracteres para 256 bits
    private static readonly byte[] Iv = Encoding.UTF8.GetBytes("abcdefghijklmnop"); // IV de 16 bytes

    public static string EncryptString(string plainText)
    {
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
            aes.IV = Iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                    {
                        streamWriter.Write("TEST123456"); // Agregar 10 caracteres de prueba
                        streamWriter.Write(plainText);
                    }
                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }
}
