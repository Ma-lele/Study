using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Fleck
{
    public interface IFleckSpecial
    {
        /// <summary>
        /// 特种设备编号列表
        /// </summary>
        /// <returns></returns>
        public List<string> SeCodes { get; }

        /// <summary>
        /// 启动Websocket服务端
        /// </summary>
        public void Start();

        /// <summary>
        /// 转发实时数据
        /// </summary>
        /// <param name="realData">实时数据</param>
        public void Distpatch<T>(T realData);
    }
}
