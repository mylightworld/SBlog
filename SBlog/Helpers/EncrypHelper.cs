using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security; 

namespace SBlog.Helpers
{
    public class EncrypHelper
    {
        private static string _encrypKey { get { return (string)ConfigurationManager.AppSettings["EncrypKey"]; } }
        private static string _encrypIv { get { return (string)ConfigurationManager.AppSettings["EncrypIv"]; } }

        public static string Encode(string data)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_encrypKey);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_encrypIv);

            //byte[] byKey = Encoding.Default.GetBytes(_encrypKey);
            //byte[] byIV = Encoding.Default.GetBytes(_encrypIv);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();

            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }

        public static string Decode(string data)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_encrypKey);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_encrypIv);
            byte[] byEnc;

            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);

            return sr.ReadToEnd();
        }


        //MD5不可逆加密 
        //32位加密 
        public static string MD5(string data, string inputCharset = "utf-8")
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(inputCharset).GetBytes(data));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }

        public static string Sha1(string data)
        {
            byte[] StrRes = Encoding.Default.GetBytes(data);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }

            return EnText.ToString();
        }

        /// <summary>
        /// SHA256加密，不可逆转
        /// </summary>
        /// <param name="str">string str:被加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        private string SHA256Encrypt(string str)
        {
            System.Security.Cryptography.SHA256 s256 = new System.Security.Cryptography.SHA256Managed();
            byte[] byte1;
            byte1 = s256.ComputeHash(Encoding.Default.GetBytes(str));
            s256.Clear();
            return Convert.ToBase64String(byte1);
        }

        /// <summary>
        /// SHA384加密，不可逆转
        /// </summary>
        /// <param name="str">string str:被加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        private string SHA384Encrypt(string str)
        {
            System.Security.Cryptography.SHA384 s384 = new System.Security.Cryptography.SHA384Managed();
            byte[] byte1;
            byte1 = s384.ComputeHash(Encoding.Default.GetBytes(str));
            s384.Clear();
            return Convert.ToBase64String(byte1);
        }

        /// <summary>
        /// SHA512加密，不可逆转
        /// </summary>
        /// <param name="str">string str:被加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        private string SHA512Encrypt(string str)
        {
            System.Security.Cryptography.SHA512 s512 = new System.Security.Cryptography.SHA512Managed();
            byte[] byte1;
            byte1 = s512.ComputeHash(Encoding.Default.GetBytes(str));
            s512.Clear();
            return Convert.ToBase64String(byte1);
        }
    }
}