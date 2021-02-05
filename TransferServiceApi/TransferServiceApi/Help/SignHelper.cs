using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferServiceApi.Help
{
    public class SignHelper
    {
        /// <summary>
        /// 参数名ASCII字典序排序
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ASCIISort(Dictionary<string, string> dt)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] array = new string[dt.Count];
            int num = 0;
            foreach (KeyValuePair<string, string> current in dt)
            {
                array[num] = current.Key;
                num++;
            }
            Array.Sort<string>(array, new Comparison<string>(string.CompareOrdinal));
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i] + "=" + dt[array[i]] + "&");
            }
            return stringBuilder.ToString().Substring(0, stringBuilder.ToString().Length - 1);
        }
    }
}
