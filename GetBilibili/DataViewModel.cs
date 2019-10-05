using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reptile;
using UC_Pub.Bases;
using UC_Pub.Enums;
using static UC_Pub.Enums.Enums;

namespace GetBilibili
{
    public class DataViewModel : VMBase
    {
        /// <summary>
        /// b站图片搜索地址常量
        /// </summary>
        private const string URL = "https://search.bilibili.com/photo?keyword=";
        /// <summary>
        /// 模拟请求的Header列表
        /// </summary>
        public ObservableCollection<Header> HeaderList { set; get; }
        /// <summary>
        /// 解析出的下载详细信息
        /// </summary>
        public ObservableCollection<DownLoadData> UrlList { set; get; }

        protected Dictionary<string, string> DefaultDic;
        /// <summary>
        /// 构造函数
        /// </summary>
        public DataViewModel()
        {
            UrlList = new ObservableCollection<DownLoadData>();
            HeaderList = new ObservableCollection<Header>
            {
                new Header
                {
                    Key = "Cookie",
                }
            };
            DefaultDic = new Dictionary<string, string>
            {
                {
                    "User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36"
                },
                {
                    "Cookie",
                     "main_confirmation=Grsb02RN5rDryyITyPSgQ5GU8fa5+PNpmeaF48WgCmE="
                },
            };

        }

        private bool downLoading = true;
        /// <summary>
        /// 是否是下载中
        /// </summary>
        public bool DownLoading
        {
            get { return downLoading; }
            set { downLoading = value; OnPropertyChanged(nameof(DownLoading)); }
        }

        private bool isDefault = true;
        /// <summary>
        /// 是否使用默认Header
        /// </summary>
        public bool IsDefault
        {
            get { return isDefault; }
            set { isDefault = value; OnPropertyChanged(nameof(IsDefault)); }
        }

        private string keyWord;
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string KeyWord
        {
            get { return keyWord; }
            set
            {
                keyWord = value;
                Url = $"{URL}{keyWord}";//输入关键字拼接Url
                OnPropertyChanged(nameof(KeyWord));
            }
        }
        private string filePath;
        /// <summary>
        /// 保存文件地址
        /// </summary>
        public string FilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    filePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);//默认桌面
                }
                return filePath;
            }
            set
            {
                filePath = value;
                OnPropertyChanged(nameof(filePath));
            }
        }
        private int page = 1;
        /// <summary>
        /// 下载的页数
        /// </summary>
        public int Page
        {
            get { return page; }
            set
            {
                if (value <= 1) return;
                page = value;
                OnPropertyChanged(nameof(Page));
            }
        }

        private string url;
        /// <summary>
        /// 搜索页的Url
        /// </summary>
        public string Url
        {
            get
            {
                if (string.IsNullOrEmpty(url))
                {
                    url = URL;
                }
                return url;
            }
            set { url = value; OnPropertyChanged(nameof(Url)); }
        }

        private string dataCount;
        /// <summary>
        /// 数据总条数
        /// </summary>
        public string DataCount
        {
            get { return dataCount; }
            set { dataCount = value; OnPropertyChanged(nameof(DataCount)); }
        }

        private string allPage;
        /// <summary>
        /// 数据总页数
        /// </summary>
        public string AllPage
        {
            get { return allPage; }
            set { allPage = value; OnPropertyChanged(nameof(AllPage)); }
        }

        private int loadDataCount;
        /// <summary>
        /// 以下载的数据条数
        /// </summary>
        public int LoadDataCount
        {
            get { return loadDataCount; }
            set { loadDataCount = value; OnPropertyChanged(nameof(LoadDataCount)); }
        }


        /// <summary>
        /// 模拟请求的Header列表转为字典参数
        /// </summary>
        private Dictionary<string, string> HeadDic
        {
            get
            {
                return isDefault ? DefaultDic : 
                    HeaderList.Where(n => n.IsEnable).ToDictionary(header => header.Key, header => header.Value);
            }
        }
        /// <summary>
        /// 通过正则获取html中的UrlId
        /// </summary>
        /// <param name="html">html</param>
        /// <returns></returns>
        public string GetUrlIDByZz(string html)
        {
            string result = 成功;
            string zz = "<a title=\"([\\s\\S]*?)\" href=\"//h.bilibili.com/([\\s\\S]*?)\\?from=search\" target=\"_blank\" class=\"title\">";
            string[] strings = Common.Analysis(zz, "$2", html);//获取UrlId
            string upZz = "<a href=\"([\\s\\S]*?)\" target=\"_blank\" class=\"up-name\">([\\s\\S]*?)</a>";
            string[] ups = Common.Analysis(upZz, "$2", html);//获取Up主
            if (string.IsNullOrWhiteSpace(DataCount))//获取一次数据总条数与总页数
            {
                string countZz = "共([\\s\\S]*?)条数据";
                DataCount = Common.Analysis(countZz, "$1", html)[0];
                int intCount = int.Parse(DataCount);
                AllPage = intCount % 20 > 0 ? (intCount / 20 + 1).ToString() : (intCount / 20).ToString();
            }
            for (var i = 0; i < strings.Length; i++)
            {
                UrlList.Add(
                    new DownLoadData
                    {
                        UpName = ups[i]
                            .Replace("<em class=\"keyword\">", "")
                            .Replace("</em>", ""),
                        UrlId = strings[i],
                        IsSelected = true,
                        State = "未下载"
                    });
            }
            return result;
        }
        /// <summary>
        /// 解析方法
        /// </summary>
        /// <returns></returns>
        public string Analysis()
        {
            string result = Enums.成功;
            UrlList.Clear();//清空解析的Url列表
            for (int i = 0; i < Page; i++)
            {
                string html;
                string res = Common.GetHtml($"{Url}{(i > 0 ? $"&page={i + 1}" : string.Empty)}", HeadDic, out html);
                if (res != Enums.成功)
                {
                    result += $"\r\n获取第{i}页HTML失败\r\n原因:{res}";
                    continue;
                }
                res = GetUrlIDByZz(html);
                if (res != Enums.成功)
                {
                    result += $"\r\n获取第{i}页UrlID失败\r\n原因:{res}";
                    continue;
                }
            }
            LoadDataCount = 0;//清空已下载的数据条数
            return result;
        }
        /// <summary>
        /// 开始下载
        /// </summary>
        public void Start()
        {
            DownLoad();
        }
        /// <summary>
        /// 下载的异步方法
        /// </summary>
        /// <returns></returns>
        public async Task DownLoad()
        {
            await Task.Run(() =>
            {
                DownLoading = false;//下载中让某些控件失效
                string path = $"{FilePath}/{KeyWord}";
                if (!Directory.Exists(path))//文件夹不存在则创建
                {
                    Directory.CreateDirectory(path);
                }
                foreach (DownLoadData data in UrlList.Where(n => n.IsSelected))
                {
                    try
                    {
                        string outString;
                        //获取图片地址的Json
                        Common.GetHtml($"https://api.vc.bilibili.com/link_draw/v1/doc/detail?doc_id={data.UrlId}", HeadDic, out outString);
                        JObject jo = (JObject)JsonConvert.DeserializeObject(outString);
                        JObject jsnoData = (JObject)JsonConvert.DeserializeObject(jo["data"].ToString());
                        JObject item = (JObject)JsonConvert.DeserializeObject(jsnoData["item"].ToString());
                        JArray pictures = (JArray)JsonConvert.DeserializeObject(item["pictures"].ToString());
                        data.AllCount = pictures.Count;//设置本次下载图片的总数
                        for (int i = 0; i < pictures.Count; i++)
                        {
                            JObject urlInfo = (JObject)JsonConvert.DeserializeObject(pictures[i].ToString());
                            Common.SaveImage(urlInfo["img_src"].ToString(), path, $"{data.UrlId}_{i}");
                            data.DownLoadCount = i + 1;//更新已下载的图片数
                        }
                        data.IsSelected = false;//取消是否下载勾选
                        LoadDataCount += 1;//更新已下载数据的条数
                        data.State = "成功!";//更新下载状态
                    }
                    catch (Exception e)
                    {
                        data.IsSelected = true;//勾选是否下载
                        data.State = $"失败!{e.Message}";//更新下载状态
                    }
                }
                DownLoading = true;//下载完成重新启用某些控件
            });
        }
        /// <summary>
        /// 添加Header
        /// </summary>
        public void Addition()
        {
            HeaderList.Add(new Header());
        }
        /// <summary>
        /// 删除最后一个Header
        /// </summary>
        public void Delete()
        {
            HeaderList.RemoveAt(HeaderList.Count - 1);
        }

        /// <summary>
        /// 选择设置文件保存地址
        /// </summary>
        public void SelectFilePath()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "选择保存位置";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                FilePath = fbd.SelectedPath;
            }
        }
    }
    /// <summary>
    /// 模拟请求的Header
    /// </summary>
    public class Header : VMBase
    {
        private string key;
        /// <summary>
        /// Key
        /// </summary>
        public string Key
        {
            get { return key; }
            set { key = value; OnPropertyChanged(nameof(key)); }
        }
        private string _value;
        /// <summary>
        /// Value
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged(nameof(Value)); }
        }
        private bool isEnable;
        /// <summary>
        /// 是否选择启用Header
        /// </summary>
        public bool IsEnable
        {
            get { return isEnable; }
            set { isEnable = value; OnPropertyChanged(nameof(IsEnable)); }
        }

    }
    /// <summary>
    /// 下载数据的详细信息
    /// </summary>
    public class DownLoadData : VMBase
    {
        private string urlId;
        /// <summary>
        /// 地址Url中的UrlId
        /// </summary>
        public string UrlId
        {
            get { return urlId; }
            set { urlId = value; }
        }

        private string state;
        /// <summary>
        /// 数据下载状态
        /// </summary>
        public string State
        {
            get { return state; }
            set { state = value; OnPropertyChanged(nameof(State)); }
        }

        private string upName;
        /// <summary>
        /// UP主的名字
        /// </summary>
        public string UpName
        {
            get { return upName; }
            set { upName = value; OnPropertyChanged(nameof(UpName)); }
        }
        private bool isSelected;
        /// <summary>
        /// 是否选择下载
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }
        private int allCount;
        /// <summary>
        /// 要下载的图片总数
        /// </summary>
        public int AllCount
        {
            get { return allCount; }
            set { allCount = value; OnPropertyChanged(nameof(AllCount)); }
        }
        private int downLoadCount;
        /// <summary>
        /// 已下载的图片数
        /// </summary>
        public int DownLoadCount
        {
            get { return downLoadCount; }
            set { downLoadCount = value; OnPropertyChanged(nameof(DownLoadCount)); }
        }


    }

}
