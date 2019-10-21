using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UC_Pub.Enums;

namespace Reptile
{
    public class InternetWorm
    {
        public async Task<string> GetHtmlPostAsync(string url, Dictionary<string, string> headers, Dictionary<string, string> parameter)
        {
            byte[] content = null;
            NewWebClient newWebClient = new NewWebClient();
            if (headers?.Count > 0)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    newWebClient.Headers.Add(header.Key, header.Value);
                }
            }
            NameValueCollection nvc = new NameValueCollection();
            if (parameter?.Count > 0)
            {
                foreach (KeyValuePair<string, string> pair in parameter)
                {
                    nvc.Add(pair.Key, pair.Value);
                }
            }
            content = await newWebClient.UploadValuesTaskAsync(url, "Post", nvc);
            return Encoding.GetEncoding("utf-8").GetString(content);
            //.Replace("\n", string.Empty)
            //.Replace("\r", string.Empty)
            //.Replace("\r\n", string.Empty);

        }
        public string GetHtmlPost(string url, Dictionary<string, string> headers, Dictionary<string, string> parameter, out string html)
        {
            string result = Enums.成功;
            try
            {
                byte[] content = null;
                NewWebClient newWebClient = new NewWebClient();
                if (headers?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> pair in headers)
                    {
                        newWebClient.Headers.Add(pair.Key, pair.Value);
                    }
                }
                NameValueCollection nvc = new NameValueCollection();
                if (parameter?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> pair in parameter)
                    {
                        nvc.Add(pair.Key, pair.Value);
                    }
                }
                content = newWebClient.UploadValues(url, "Post", nvc);
                html = Encoding.GetEncoding("utf-8").GetString(content);
                //.Replace("\n", string.Empty)
                //.Replace("\r", string.Empty)
                //.Replace("\r\n", string.Empty);
            }
            catch (Exception e)
            {
                result = e.Message;
                html = string.Empty;
            }
            return result;
        }

        /// <summary>
        /// 根据url获取html
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="headers">headers表头类容</param>
        /// <param name="html">返回获取的html</param>
        /// <returns></returns>
        public string GetHtml(string url, Dictionary<string, string> headers, out string html)
        {
            string result = Enums.成功;
            try
            {
                byte[] content = null;
                NewWebClient newWebClient = new NewWebClient();
                if (headers?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> pair in headers)
                    {
                        newWebClient.Headers.Add(pair.Key, pair.Value);
                    }
                }
                content = newWebClient.DownloadData(url);
                html = Encoding.GetEncoding("utf-8").GetString(content);
                //.Replace("\n", string.Empty)
                //.Replace("\r", string.Empty)
                //.Replace("\r\n", string.Empty);
            }
            catch (Exception e)
            {
                result = e.Message;
                html = string.Empty;
            }
            return result;
        }
        /// <summary>
        /// 根据url获取html
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="headers">headers表头类容</param>
        /// <returns></returns>
        public async Task<string> GetHtmlAsync(string url, Dictionary<string, string> headers)
        {
            string result;
            byte[] content = null;
            NewWebClient newWebClient = new NewWebClient();
            if (headers?.Count > 0)
            {
                foreach (KeyValuePair<string, string> pair in headers)
                {
                    newWebClient.Headers.Add(pair.Key, pair.Value);
                }
            }
            content = await newWebClient.DownloadDataTaskAsync(new Uri(url));
            result = Encoding.GetEncoding("utf-8").GetString(content);
            return result;
            //.Replace("\n", string.Empty)
            //.Replace("\r", string.Empty)
            //.Replace("\r\n", string.Empty);
        }
        /// <summary>
        /// 通过正则(zz)解析文本(html)
        /// </summary>
        /// <param name="zz">正则</param>
        /// <param name="did">正则替换位置</param>
        /// <param name="html">文本</param>
        /// <returns></returns>
        public string[] Analysis(string zz, string did, string html)
        {
            MatchCollection matchs = new Regex(zz).Matches(html);
            string[] result = new string[matchs.Count];
            for (var i = 0; i < matchs.Count; i++)
            {
                result[i] = matchs[i].Result(did)
                    .Replace("<em>", string.Empty)
                    .Replace("</em>", string.Empty);
            }
            return result;
        }
        /// <summary>
        /// 保存网络图片资源到本地
        /// </summary>
        /// <param name="url">网络地址</param>
        /// <param name="filePath">保存本地位置</param>
        /// <param name="fileName">保存本地文件名</param>
        public void SaveImage(string url, string filePath, string fileName)
        {
            Image image;
            filePath = $"{filePath}/{fileName}{url.Substring(url.LastIndexOf(".", StringComparison.Ordinal), url.Length - url.LastIndexOf(".", StringComparison.Ordinal))}";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse rep = (HttpWebResponse)req.GetResponse();
            using (Stream str = rep.GetResponseStream())
            {
                image = Image.FromStream(str);
                str.Close();
            }
            image.Save(filePath);
            GC.Collect();
        }

        public void SaveFile(string url, string savePath, string fileName)
        {
            savePath = $"{savePath}/{fileName}";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse rep = (HttpWebResponse)req.GetResponse();
            using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                rep.GetResponseStream()?.CopyTo(fileStream);
                fileStream.Close();
            }
            GC.Collect();
        }


        public string GetUA()
        {
            string[] USER_AGENTS = {
                    "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; AcooBrowser; .NET CLR 1.1.4322; .NET CLR 2.0.50727)",
                    "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; Acoo Browser; SLCC1; .NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.0.04506)",
                    "Mozilla/4.0 (compatible; MSIE 7.0; AOL 9.5; AOLBuild 4337.35; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)",
                    "Mozilla/5.0 (Windows; U; MSIE 9.0; Windows NT 9.0; en-US)",
                    "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET CLR 2.0.50727; Media Center PC 6.0)",
                    "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET CLR 1.0.3705; .NET CLR 1.1.4322)",
                    "Mozilla/4.0 (compatible; MSIE 7.0b; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 3.0.04506.30)",
                    "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN) AppleWebKit/523.15 (KHTML, like Gecko, Safari/419.3) Arora/0.3 (Change: 287 c9dfb30)",
                    "Mozilla/5.0 (X11; U; Linux; en-US) AppleWebKit/527+ (KHTML, like Gecko, Safari/419.3) Arora/0.6",
                    "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.2pre) Gecko/20070215 K-Ninja/2.1.1",
                    "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9) Gecko/20080705 Firefox/3.0 Kapiko/3.0",
                    "Mozilla/5.0 (X11; Linux i686; U;) Gecko/20070322 Kazehakase/0.4.5",
                    "Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.9.0.8) Gecko Fedora/1.9.0.8-1.fc10 Kazehakase/0.5.6",
                    "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.56 Safari/535.11",
                    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_3) AppleWebKit/535.20 (KHTML, like Gecko) Chrome/19.0.1036.7 Safari/535.20",
                    "Opera/9.80 (Macintosh; Intel Mac OS X 10.6.8; U; fr) Presto/2.9.168 Version/11.52",
                    "Mozilla/5.0 (Linux; Android 4.1.1; Nexus 7 Build/JRO03D) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166  Safari/535.19",
                    "Mozilla/5.0 (Linux; Android 4.1.1; Nexus 7 Build/JRO03D) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166 Safari/535.19",
                    "Mozilla/5.0 (Linux; U; Android 4.0.4; en-gb; GT-I9300 Build/IMM76D) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30",
                    "Mozilla/5.0 (Linux; U; Android 2.2; en-gb; GT-P1000 Build/FROYO) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1",
                    "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:21.0) Gecko/20100101 Firefox/21.0",
                    "Mozilla/5.0 (Android; Mobile; rv:14.0) Gecko/14.0 Firefox/14.0",
                    "Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19",
                    "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.94 Safari/537.36",
                    "Mozilla/5.0 (iPad; CPU OS 5_0 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9A334 Safari/7534.48.3",
                    "Mozilla/5.0 (iPod; U; CPU like Mac OS X; en) AppleWebKit/420.1 (KHTML, like Gecko) Version/3.0 Mobile/3A101a Safari/419.3",
            };
            return USER_AGENTS[new Random().Next(0, USER_AGENTS.Length)];
        }
    }
}
