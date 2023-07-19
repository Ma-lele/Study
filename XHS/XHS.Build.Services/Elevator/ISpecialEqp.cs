using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace XHS.Build.Services.Elevator
{
    /// <summary>
    /// 特种设备全局工具
    /// </summary>
    public interface ISpecialEqp
    {
        ConcurrentDictionary<string, SpecialEqpBean> List { get; }

        /// <summary>
        /// 更新重置
        /// </summary>
        void reset();

        /// <summary>
        /// 初始化
        /// </summary>
        void init();

        /// <summary>
        /// 设置设备
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        SpecialEqpBean set(DataRow dr);
    }

}
