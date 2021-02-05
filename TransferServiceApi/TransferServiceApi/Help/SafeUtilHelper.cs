using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TransferServiceApi.Help
{
    public class SafeUtilHelper
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="info">待加密文本</param>
        /// <param name="key">des_key</param>
        /// <returns></returns>
        public string GetEncryptDES(string info, string key)
        {
            byte[] toEncrypt = Encoding.UTF8.GetBytes(info);
            MemoryStream mStream = new MemoryStream();
            TripleDESCryptoServiceProvider tripdes = new TripleDESCryptoServiceProvider();
            tripdes.Key = Convert.FromBase64String(key);
            tripdes.Mode = CipherMode.CBC;
            tripdes.Padding = PaddingMode.PKCS7;
            tripdes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            CryptoStream cstream = new CryptoStream(mStream, tripdes.CreateEncryptor(), CryptoStreamMode.Write);
            cstream.Write(toEncrypt, 0, toEncrypt.Length);
            cstream.FlushFinalBlock();
            byte[] ret = mStream.ToArray();
            cstream.Close();
            mStream.Close();
            string Baseret = Convert.ToBase64String(ret);
            return Baseret;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="info">待解密文本</param>
        /// <param name="key">des_key</param>
        /// <returns></returns>
        public string GetDecryptDES(string info, string key)
        {
            MemoryStream mStream = new MemoryStream();
            TripleDESCryptoServiceProvider tripdes = new TripleDESCryptoServiceProvider();
            tripdes.Key = Convert.FromBase64String(key);
            tripdes.Mode = CipherMode.CBC;
            tripdes.Padding = PaddingMode.PKCS7;
            tripdes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            CryptoStream cstream = new CryptoStream(mStream, tripdes.CreateDecryptor(), CryptoStreamMode.Write);
            byte[] toEncrypt = Convert.FromBase64String(info);
            cstream.Write(toEncrypt, 0, toEncrypt.Length);
            cstream.FlushFinalBlock();
            byte[] ret = mStream.ToArray();
            cstream.Close();
            mStream.Close();
            string Baseret = Encoding.UTF8.GetString(ret);
            return Baseret;
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="data">原文：UTF8编码</param>
        /// <param name="privateKeyPath">证书路径：D:/certs/mycert.key</param>
        /// <returns>验签sign</returns>
        public string Sign(string data, string privateKeyPath)
        {
            RSACryptoServiceProvider rsaCsp = LoadCertificateFile(privateKeyPath);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signatureBytes = rsaCsp.SignData(dataBytes, "SHA1");
            return Convert.ToBase64String(signatureBytes);
        }

        private byte[] GetPem(string type, byte[] data)
        {
            string pem = Encoding.UTF8.GetString(data);
            string header = String.Format("-----BEGIN {0}-----", type);
            string footer = String.Format("-----END {0}-----", type);
            int start = pem.IndexOf(header) + header.Length;
            int end = pem.IndexOf(footer, start);

            string base64 = pem.Substring(start, (end - start));

            return Convert.FromBase64String(base64);
        }

        private RSACryptoServiceProvider LoadCertificateFile(string filename)
        {
            using (System.IO.FileStream fs = System.IO.File.OpenRead(filename))
            {
                byte[] data = new byte[fs.Length];
                byte[] res = null;
                fs.Read(data, 0, data.Length);
                if (data[0] != 0x30)
                {
                    res = GetPem("RSA PRIVATE KEY", data);
                }
                try
                {
                    RSACryptoServiceProvider rsa = DecodeRSAPrivateKey(res);
                    return rsa;
                }
                catch (Exception)
                {
                }
                return null;
            }
        }

        private RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------  
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading  
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)  
                    binr.ReadByte();    //advance 1 byte  
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes  
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number  
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;

                //------ all private key components are Integer sequences ----  
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----  
                CspParameters CspParameters = new CspParameters();
                CspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024, CspParameters);
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }

        private int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)     //expect integer  
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();    // data size in next byte  
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes  
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size  
            }

            while (binr.ReadByte() == 0x00)
            {   //remove high order zeros in data  
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);       //last ReadByte wasn't a removed zero, so back up a byte  
            return count;
        }
    }
}
