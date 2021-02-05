using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TransferServiceApi.Common
{
    public class DataSerialize
    {
        /// <summary>
        /// 将对象返回Json或Xml
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="obj"></param>
        /// <param Name="RETID"></param>
        /// <returns></returns>
        public static string StringOfObject<T>(T obj, int RETID)
        {
            string res = RETID switch
            {
                1 => JsonSerialize(obj),
                2 => XmlSerialize(obj),
                _ => JsonSerialize(obj),
            };
            return res;
        }

        /// <summary>
        /// 对象序列化成 XML String  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string XmlSerialize<T>(T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            XmlSerializerNamespaces xn = new XmlSerializerNamespaces();
            xn.Add("", "");
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xs = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = false,
                Indent = true
            };
            XmlWriter xw = XmlWriter.Create(sb, xs);
            xmlSerializer.Serialize(xw, obj, xn);
            string xmlString = sb.ToString();
            return xmlString;
        }

        /// <summary>
        /// 对象序列化成 Json String
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="obj"></param>
        /// <returns></returns>
        public static string JsonSerialize<T>(T obj)
        {
            string res = JsonConvert.SerializeObject(obj);
            return res;
        }
    }
}
