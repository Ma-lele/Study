using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace XHS.Build.Common
{
    /// <summary>
    /// 加密、解密
    /// </summary>
    public sealed class UEncrypter
    {
        //DES默认密钥向量
        private static byte[] DES_IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        //AES默认密钥向量   
        public static readonly byte[] AES_IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        private const string RSA_XML_STR = "<RSAKeyValue>";
        private const string SPACE = " ";
        private static Encoding ENCODING = Encoding.UTF8;
        #region Hmac
        /// <summary>
        /// HMAC SHA256
        /// </summary>
        /// <param name="message">原文</param>
        /// <param name="secret">秘钥</param>
        /// <returns></returns>
        public static string HmacSHA256(string message, string secret)
        {
            secret = secret ?? string.Empty;
            //var encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = ENCODING.GetBytes(secret);
            byte[] messageBytes = ENCODING.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        /// <summary>
        ///  HMAC-SHA1
        /// </summary>
        /// <param name="message">原文</param>
        /// <param name="secret">秘钥</param>
        /// <returns></returns>
        public static string HmacSHA1(string message, string secret)
        {
            var hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(secret));
            var dataBuffer = Encoding.UTF8.GetBytes(message);
            var hashBytes = hmacsha1.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes);
        }
        #endregion

        #region MD5
        /// <summary>
        /// MD5加密为32字符长度的16进制字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EncryptByMD5(string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();
            //将每个字节转为16进制
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
        #endregion

        #region SHA1
        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EncryptBySHA1(string input)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] bytes = ENCODING.GetBytes(input);
            byte[] result = sha.ComputeHash(bytes);
            return BitConverter.ToString(result);
        }
        #endregion

        #region SHA256
        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="original">原始文字</param>
        /// <returns></returns>
        public static string SHA256(string original)
        {
            return SHA256(original, ENCODING);
        }

        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="original">原始文字</param>
        /// <param name="encode">字符编码方案</param>
        /// <returns></returns>
        public static string SHA256(string original, Encoding encode)
        {
            byte[] bytes = encode.GetBytes(original);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }

            return builder.ToString();
        }
        #endregion

        #region SHA512
        /// <summary>
        /// SHA512加密
        /// </summary>
        /// <param name="original">原始文字</param>
        /// <returns></returns>
        public static string SHA512(string original)
        {
            return SHA512(original, ENCODING);
        }

        /// <summary>
        /// SHA512加密
        /// </summary>
        /// <param name="original">原始文字</param>
        /// <param name="encode">字符编码方案</param>
        public static string SHA512(string original, Encoding encode)
        {
            byte[] bytes = ENCODING.GetBytes(original);
            byte[] hash = SHA512Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }

            return builder.ToString();
        }
        #endregion

        #region DES
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EncryptByDES(string input, string key)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input); //Encoding.UTF8.GetBytes(input);
            byte[] keyBytes = ASCIIEncoding.UTF8.GetBytes(key);
            byte[] encryptBytes = EncryptByDES(inputBytes, keyBytes, keyBytes);
            //string result = Encoding.UTF8.GetString(encryptBytes); //无法解码,其加密结果中文出现乱码：d\"�e����(��uπ�W��-��,_�\nJn7 
            //原因：如果明文为中文，UTF8编码两个字节标识一个中文字符，但是加密后，两个字节密文，不一定还是中文字符。
            using (DES des = new DESCryptoServiceProvider())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cs))
                        {
                            writer.Write(inputBytes);
                        }
                    }
                }
            }

            string result = Convert.ToBase64String(encryptBytes);

            return result;
        }
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="inputBytes">输入byte数组</param>
        /// <param name="key">密钥，只能是英文字母或数字</param>
        /// <param name="IV">偏移向量</param>
        /// <returns></returns>
        public static byte[] EncryptByDES(byte[] inputBytes, byte[] key, byte[] IV)
        {
            DES des = new DESCryptoServiceProvider();
            //建立加密对象的密钥和偏移量
            des.Key = key;
            des.IV = IV;
            string result = string.Empty;

            //1、如果通过CryptoStreamMode.Write方式进行加密，然后CryptoStreamMode.Read方式进行解密，解密成功。
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputBytes, 0, inputBytes.Length);
                }
                return ms.ToArray();
            }
            //2、如果通过CryptoStreamMode.Write方式进行加密，然后再用CryptoStreamMode.Write方式进行解密，可以得到正确结果
            //3、如果通过CryptoStreamMode.Read方式进行加密，然后再用CryptoStreamMode.Read方式进行解密，无法解密，Error：要解密的数据的长度无效。
            //4、如果通过CryptoStreamMode.Read方式进行加密，然后再用CryptoStreamMode.Write方式进行解密,无法解密，Error：要解密的数据的长度无效。
            //using (MemoryStream ms = new MemoryStream(inputBytes))
            //{
            //    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Read))
            //    {
            //        using (StreamReader reader = new StreamReader(cs))
            //        {
            //            result = reader.ReadToEnd();
            //            return Encoding.UTF8.GetBytes(result);
            //        }
            //    }
            //}
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecryptByDES(string input, string key)
        {
            //UTF8无法解密，Error: 要解密的数据的长度无效。
            //byte[] inputBytes = Encoding.UTF8.GetBytes(input);//UTF8乱码，见加密算法
            byte[] inputBytes = Convert.FromBase64String(input);

            byte[] keyBytes = ASCIIEncoding.UTF8.GetBytes(key);
            byte[] resultBytes = DecryptByDES(inputBytes, keyBytes, keyBytes);

            string result = Encoding.UTF8.GetString(resultBytes);

            return result;
        }
        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="inputBytes"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] DecryptByDES(byte[] inputBytes, byte[] key, byte[] iv)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //建立加密对象的密钥和偏移量，此值重要，不能修改
            des.Key = key;
            des.IV = iv;

            //通过write方式解密
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
            //    {
            //        cs.Write(inputBytes, 0, inputBytes.Length);
            //    }
            //    return ms.ToArray();
            //}

            //通过read方式解密
            using (MemoryStream ms = new MemoryStream(inputBytes))
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cs))
                    {
                        string result = reader.ReadToEnd();
                        return Encoding.UTF8.GetBytes(result);
                    }
                }
            }

            //错误写法,注意哪个是输出流的位置，如果范围ms，与原文不一致。
            //using (MemoryStream ms = new MemoryStream(inputBytes))
            //{
            //    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read))
            //    {
            //        cs.Read(inputBytes, 0, inputBytes.Length);
            //    }
            //    return ms.ToArray();
            //}
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string EncryptString(string input, string sKey)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                ICryptoTransform desencrypt = des.CreateEncryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return BitConverter.ToString(result);
            }
        }
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string DecryptString(string input, string sKey)
        {
            string[] sInput = input.Split("-".ToCharArray());
            byte[] data = new byte[sInput.Length];
            for (int i = 0; i < sInput.Length; i++)
            {
                data[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
            }
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                ICryptoTransform desencrypt = des.CreateDecryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return Encoding.UTF8.GetString(result);
            }
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="input">原文</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string EncryptDecECB(string input, string key)
        {
            byte[] inputByte = Encoding.UTF8.GetBytes(input);
            byte[] keyByte = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                //des.Key = HashMD5(salt);
                des.Key = keyByte;
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;
                return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(inputByte, 0, inputByte.Length));
            }
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="input">密文</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string DecryptDecECB(string input, string key)
        {
            byte[] inputByte = Convert.FromBase64String(input);
            byte[] keyByte = Encoding.UTF8.GetBytes(key.Substring(0, 8));

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = keyByte;
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;
                return Encoding.UTF8.GetString(des.CreateDecryptor().TransformFinalBlock(inputByte, 0, inputByte.Length));
            }
        }

        #endregion

        #region AES
        /// <summary>
        /// 有密码的AES加密
        /// </summary>
        /// <param name="text">加密字符</param>
        /// <param name="password">加密的密码</param>
        /// <returns></returns>
        public static string EncryptAES128ECBPKCS5Padding(string toEncrypt, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="password"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DecryptAES128ECBPKCS5Padding(string toDecrypt, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>  
        /// AES加密算法  
        /// </summary>  
        /// <param name="input">明文字符串</param>  
        /// <param name="key">密钥</param>  
        /// <returns>字符串</returns>  
        public static string EncryptByAES(string input, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 32));
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = AES_IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }
                        byte[] bytes = msEncrypt.ToArray();
                        //return Convert.ToBase64String(bytes);//此方法不可用
                        return BitConverter.ToString(bytes);
                    }
                }
            }
        }
        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="input">密文字节数组</param>  
        /// <param name="key">密钥</param>  
        /// <returns>返回解密后的字符串</returns>  
        public static string DecryptByAES(string input, string key)
        {
            //byte[] inputBytes = Convert.FromBase64String(input); //Encoding.UTF8.GetBytes(input);
            string[] sInput = input.Split("-".ToCharArray());
            byte[] inputBytes = new byte[sInput.Length];
            for (int i = 0; i < sInput.Length; i++)
            {
                inputBytes[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
            }
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 32));
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = AES_IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream(inputBytes))
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srEncrypt = new StreamReader(csEncrypt))
                        {
                            return srEncrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        /// <summary> 
        /// AES加密        
        /// </summary> 
        /// <param name="inputdata">输入的数据</param>         
        /// <param name="iv">向量128位</param>         
        /// <param name="strKey">加密密钥</param>         
        /// <returns></returns> 
        public static byte[] EncryptByAES(byte[] inputdata, byte[] key, byte[] iv)
        {
            ////分组加密算法 
            //Aes aes = new AesCryptoServiceProvider();          
            ////设置密钥及密钥向量 
            //aes.Key = key;
            //aes.IV = iv;
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            //    {
            //        using (StreamWriter writer = new StreamWriter(cs))
            //        {
            //            writer.Write(inputdata);
            //        }
            //        return ms.ToArray(); 
            //    }               
            //}

            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(inputdata);
                        }
                        byte[] encrypted = msEncrypt.ToArray();
                        return encrypted;
                    }
                }
            }
        }
        /// <summary>         
        /// AES解密         
        /// </summary> 
        /// <param name="inputdata">输入的数据</param>                
        /// <param name="key">key</param>         
        /// <param name="iv">向量128</param> 
        /// <returns></returns> 
        public static byte[] DecryptByAES(byte[] inputBytes, byte[] key, byte[] iv)
        {
            Aes aes = new AesCryptoServiceProvider();
            aes.Key = key;
            aes.IV = iv;
            byte[] decryptBytes;
            using (MemoryStream ms = new MemoryStream(inputBytes))
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cs))
                    {
                        string result = reader.ReadToEnd();
                        decryptBytes = Encoding.UTF8.GetBytes(result);
                    }
                }
            }

            return decryptBytes;
        }
        #endregion

        #region DSA
        #endregion

        #region RSA
        /// <summary>
        /// RSA加密(Base64)
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <param name="publicKey">公钥</param>
        /// <returns>密文字符串</returns>
        public static string EncryptByRSA(string plaintext, string publicKey)
        {
            if (string.IsNullOrEmpty(plaintext))
                return null;
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plaintext);
            if (publicKey.IndexOf(RSA_XML_STR) < 0)
                publicKey = RSAPublicKeyJava2DotNet(publicKey);//如果不是xml,就转成xml

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.FromXmlString(publicKey);
                byte[] encryptedData = RSA.Encrypt(dataToEncrypt, false);
                return Convert.ToBase64String(encryptedData);
            }
        }

        /// <summary>
        /// RSA加密(16进制)
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <param name="publicKey">公钥</param>
        /// <returns>密文字符串</returns>
        public static string EncryptByRSA16(string plaintext, string publicKey)
        {
            if (string.IsNullOrEmpty(plaintext))
                return null;
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plaintext);
            if (publicKey.IndexOf(RSA_XML_STR) < 0)
                publicKey = RSAPublicKeyJava2DotNet(publicKey);//如果不是xml,就转成xml

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.FromXmlString(publicKey);
                byte[] encryptedData = RSA.Encrypt(dataToEncrypt, false);
                return ByteToString16(encryptedData).Replace(SPACE, string.Empty); ;
            }
        }

        /// <summary>
        /// RSA解密(Base64)
        /// </summary>
        /// <param name="ciphertext">密文</param>
        /// <param name="privateKey">私钥</param>
        /// <returns>明文字符串</returns>
        public static string DecryptByRSA(string ciphertext, string privateKey)
        {
            if (privateKey.IndexOf(RSA_XML_STR) < 0)
                privateKey = RSAPrivateKeyJava2DotNet(privateKey);//如果不是xml,就转成xml

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.FromXmlString(privateKey);
                byte[] encryptedData = Convert.FromBase64String(ciphertext);
                byte[] decryptedData = RSA.Decrypt(encryptedData, false);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }

        /// <summary>
        /// RSA解密(16进制)
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <param name="publicKey">公钥</param>
        /// <returns>密文字符串</returns>
        public static string DecryptByRSA16(string ciphertext, string privateKey)
        {
            if (privateKey.IndexOf(RSA_XML_STR) < 0)
                privateKey = RSAPrivateKeyJava2DotNet(privateKey);//如果不是xml,就转成xml

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.FromXmlString(privateKey);
                //byte[] encryptedData = Convert.FromBase64String(ciphertext);
                byte[] encryptedData = String16ToByte(ciphertext);
                byte[] decryptedData = RSA.Decrypt(encryptedData, false);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="InBytes"></param>
        /// <returns></returns>
        public static string ByteToString16(byte[] InBytes)
        {
            string StringOut = "";
            foreach (byte InByte in InBytes)
            {
                StringOut = StringOut + String.Format("{0:X2} ", InByte);
            }
            return StringOut;
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="InString"></param>
        /// <returns></returns>
        public static byte[] String16ToByte(string InString)
        {
            if ((InString.Length % 2) != 0)
                InString += " ";
            byte[] returnBytes = new byte[InString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(InString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 数字签名
        /// </summary>
        /// <param name="plaintext">原文</param>
        /// <param name="privateKey">私钥</param>
        /// <returns>签名</returns>
        public static string HashAndSignString(string plaintext, string privateKey)
        {
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plaintext);
            if (privateKey.IndexOf(RSA_XML_STR) < 0)
                privateKey = RSAPrivateKeyJava2DotNet(privateKey);//如果不是xml,就转成xml

            using (RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider())
            {
                RSAalg.FromXmlString(privateKey);
                //使用SHA1进行摘要算法，生成签名
                byte[] encryptedData = RSAalg.SignData(dataToEncrypt, new SHA1CryptoServiceProvider());
                //string result = Convert.ToBase64String(encryptedData);
                string result = ByteToString16(encryptedData).Replace(SPACE, string.Empty);
                return result;
            }
        }
        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="plaintext">原文</param>
        /// <param name="SignedData">签名</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static bool VerifySigned(string plaintext, string SignedData, string publicKey)
        {
            if (publicKey.IndexOf(RSA_XML_STR) < 0)
                publicKey = RSAPublicKeyJava2DotNet(publicKey);//如果不是xml,就转成xml

            using (RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider())
            {
                RSAalg.FromXmlString(publicKey);
                byte[] dataToVerifyBytes = Encoding.UTF8.GetBytes(plaintext);
                //byte[] signedDataBytes = Convert.FromBase64String(SignedData);
                byte[] signedDataBytes = String16ToByte(SignedData);
                return RSAalg.VerifyData(dataToVerifyBytes, new SHA1CryptoServiceProvider(), signedDataBytes);
            }
        }
        /// <summary>
        /// 获取Key
        /// 键为公钥，值为私钥
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<string, string> CreateRSAKey()
        {
            return CreateRSAKey(false);
        }

        /// <summary>
        /// 获取Key
        /// 键为公钥，值为私钥
        /// </summary>
        /// <param name="isXml">是否是XML格式</param>
        /// <returns></returns>
        public static KeyValuePair<string, string> CreateRSAKey(bool isXml)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            string privateKey = RSA.ToXmlString(true);
            string publicKey = RSA.ToXmlString(false);

            if (isXml)
            {
                privateKey = RSAPrivateKeyDotNet2Java(privateKey);
                publicKey = RSAPublicKeyDotNet2Java(publicKey);
            }

            return new KeyValuePair<string, string>(publicKey, privateKey);
        }

        /// <summary>      
        /// RSA私钥格式转换，java->.net      
        /// </summary>      
        /// <param name="privateKey">java生成的RSA私钥</param>      
        /// <returns></returns>     
        public static string RSAPrivateKeyJava2DotNet(string privateKey)
        {
            RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
            Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
        }

        /// <summary>      
        /// RSA私钥格式转换，.net->java      
        /// </summary>      
        /// <param name="privateKey">.net生成的私钥</param>      
        /// <returns></returns>     
        public static string RSAPrivateKeyDotNet2Java(string privateKey)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(privateKey);
            BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
            BigInteger exp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
            BigInteger d = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("D")[0].InnerText));
            BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("P")[0].InnerText));
            BigInteger q = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Q")[0].InnerText));
            BigInteger dp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DP")[0].InnerText));
            BigInteger dq = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DQ")[0].InnerText));
            BigInteger qinv = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("InverseQ")[0].InnerText));
            RsaPrivateCrtKeyParameters privateKeyParam = new RsaPrivateCrtKeyParameters(m, exp, d, p, q, dp, dq, qinv);
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParam);
            byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetEncoded();
            return Convert.ToBase64String(serializedPrivateBytes);
        }

        /// <summary>      
        /// RSA公钥格式转换，java->.net      
        /// </summary>      
        /// <param name="publicKey">java生成的公钥</param>      
        /// <returns></returns>      
        public static string RSAPublicKeyJava2DotNet(string publicKey)
        {
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
        }

        /// <summary>      
        /// RSA公钥格式转换，.net->java      
        /// </summary>      
        /// <param name="publicKey">.net生成的公钥</param>      
        /// <returns></returns>     
        public static string RSAPublicKeyDotNet2Java(string publicKey)
        {
            XmlDocument doc = new XmlDocument(); doc.LoadXml(publicKey);
            BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
            BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
            RsaKeyParameters pub = new RsaKeyParameters(false, m, p);
            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pub);
            byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
            return Convert.ToBase64String(serializedPublicBytes);
        }
        #endregion

        #region other
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string input)
        {
            string[] sInput = input.Split("-".ToCharArray());
            byte[] inputBytes = new byte[sInput.Length];
            for (int i = 0; i < sInput.Length; i++)
            {
                inputBytes[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
            }
            return inputBytes;
        }
        #endregion

        /// <summary>  
        /// Unix时间戳转为C#格式时间  
        /// </summary>  
        /// <param name="timeStamp">Unix时间戳格式,例如1482115779</param>  
        /// <returns>C#格式时间</returns>  
        public static DateTime GetDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp.Substring(0, 10) + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }


        /// <summary>  
        /// DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time"> DateTime时间格式</param>  
        /// <returns>Unix时间戳格式</returns>  
        public static int GetTimeStamp(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
    }
}
