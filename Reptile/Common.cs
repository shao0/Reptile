using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UC_Pub.Bases;
using UC_Pub.Enums;

namespace Reptile
{
    public static class Common
    {
        /// <summary>
        /// 根据url获取html
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="headrs">headrs表头类容</param>
        /// <param name="html">返回获取的html</param>
        /// <returns></returns>
        public static string GetHtml(string url, Dictionary<string, string> headrs, out string html)
        {
            string result = Enums.成功;
            try
            {
                byte[] content = null;
                WebClient webClient = new WebClient();
                foreach (KeyValuePair<string, string> headr in headrs)
                {
                    webClient.Headers.Add(headr.Key, headr.Value);
                }
                content = webClient.DownloadData(url);
                html = Encoding.GetEncoding("utf-8").GetString(content);
            }
            catch (Exception e)
            {
                result = e.Message;
                html = string.Empty;
            }
            return result;
        }
        /// <summary>
        /// 通过正则(zz)解析文本(html)
        /// </summary>
        /// <param name="zz">正则</param>
        /// <param name="did">正则替换位置</param>
        /// <param name="html">文本</param>
        /// <returns></returns>
        public static string[] Analysis(string zz,string did, string html)
        {
            MatchCollection matchs = new Regex(zz).Matches(html);
            string[] result = new string[matchs.Count];
            for (var i = 0; i < matchs.Count; i++)
            {
                result[i] = matchs[i].Result(did);
            }
            return result;
        }
        /// <summary>
        /// 批量解析
        /// </summary>
        /// <param name="ZzDic">正则字典</param>
        /// <param name="html">解析文本</param>
        /// <returns></returns>
        public static Dictionary<string,string[]> AnalysisBatch(Dictionary<string,string> ZzDic,string html)
        {
            Dictionary<string, string[]> resultDic = new Dictionary<string, string[]>();
            foreach (KeyValuePair<string, string> keyValuePair in ZzDic)
            {
                resultDic.Add(keyValuePair.Key,Analysis(keyValuePair.Key, keyValuePair.Value, html));
            }
            return resultDic;
        }
        /// <summary>
        /// 保存网络图片资源到本地
        /// </summary>
        /// <param name="url">网络地址</param>
        /// <param name="filePath">保存本地位置</param>
        public static void SaveImage(string url , string filePath,string fileName)
        {
            filePath = $"{filePath}/{fileName}{url.Substring(url.LastIndexOf("."), url.Length - url.LastIndexOf("."))}";
            Image image;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse rep = (HttpWebResponse)req.GetResponse();
            using (Stream str = rep.GetResponseStream())
            {
                image =  Image.FromStream(str);
                str.Close();
            }
            image.Save(filePath);
            GC.Collect();
        }
    }
}
