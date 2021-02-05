using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TransferServiceApi.Help
{
    public class HttpClientHelper
    {
        /// <summary>
        /// HttpGet请求
        /// </summary>
        /// <param name="Url">Url地址</param>
        /// <param name="contentType">请求方式</param>
        /// <returns></returns>
        public static string HttpGet(string Url, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = contentType;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(myResponseStream);
            string retString = streamReader.ReadToEnd();
            streamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        /// <summary>
        /// HttpPost请求
        /// </summary>
        /// <param name="Url">Url地址</param>
        /// <param name="postDataStr">参数</param>
        /// <param name="contentType">请求方式</param>
        /// <returns></returns>
        public static string HttpPost(string Url, string postDataStr, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = contentType;
            request.Timeout = 600000;//设置超时时间
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            Stream requestStream = request.GetRequestStream();
            StreamWriter streamWriter = new StreamWriter(requestStream);
            streamWriter.Write(postDataStr);
            streamWriter.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            string retString = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            return retString;
        }
    }
}
