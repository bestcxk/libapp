using Bing.Extensions;
using Bing.Text;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Mijin.Library.App.Driver.Drivers.Sudo;
public class SudoEncrypt
{

    /// <summary>
    /// 128位0向量
    /// </summary>
    private static byte[] _iv;

    /// <summary>
    /// 128位0向量
    /// </summary>
    private static byte[] Iv
    {
        get
        {
            if (_iv == null)
            {
                var size = 16;
                _iv = new byte[size];
                for (int i = 0; i < size; i++)
                {
                    _iv[i] = 0;
                }
            }
            return _iv;
        }
    }

    /// <summary>
    /// AES密钥
    /// </summary>
    public static string AesKey { get; set; } = "tecsun1234567890";

    /// <summary>
    /// 创建RijndaelManaged
    /// </summary>
    /// <param name="key">密钥</param>
    private static RijndaelManaged CreateRijndaelManaged(string key) =>
        new()
        {
            //Key = Convert.FromBase64String(key),
            Key = key.ToBytes(),
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7,
            IV = Iv
        };

    /// <summary>
    /// AES解密
    /// </summary>
    /// <param name="value">待解密的值</param>
    /// <param name="key">密钥</param>
    /// <param name="encoding">字符编码</param>
    public static string AesDecrypt(string value, string key, Encoding encoding)
    {
        if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
            return string.Empty;
        var rijndaelManaged = CreateRijndaelManaged(key);
        using (var transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV))
            return GetDecryptResult(value, encoding, transform);
    }

    /// <summary>
    /// 获取解密结果
    /// </summary>
    /// <param name="value">待解密的值</param>
    /// <param name="encoding">字符编码</param>
    /// <param name="transform">加密器</param>
    private static string GetDecryptResult(string value, Encoding encoding, ICryptoTransform transform)
    {
        var bytes = Convert.FromBase64String(value);
        var result = transform.TransformFinalBlock(bytes, 0, bytes.Length);
        return encoding.GetString(result);
    }
}
