using System;
using System.Net;

namespace Reptile
{
    public class NewWebClient : WebClient
    {
        /// <summary>
        /// 超时时间默认十秒
        /// </summary>
        private readonly int _outTime;

        public NewWebClient()
        {
            _outTime = 10000;
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="outTime">设置的超时时间</param>
        public NewWebClient(int outTime)
        {
            _outTime = outTime;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest result = base.GetWebRequest(address);
            result.Timeout = _outTime;
            return result;

        }
    }
}
