using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using XHS.Build.Common.Util;
using XHS.Build.Model.WJWechat;

namespace XHS.Build.Common.Wechat
{
    /// <summary>
    /// 微信模板消息
    /// </summary>
    public class WTemplateMessager : BaseWechat
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public string errcode { get; set; }
        /// <summary>
        /// 错误内容
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 成功
        /// </summary>
        public const string SUCCESS = "0";

        /// <summary>
        /// 模板数据数组
        /// </summary>
        private static Dictionary<MType, string> _tpls = null;
        /// <summary>
        /// 模板ID数组
        /// </summary>
        private static Dictionary<MType, string> _tplIds = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="wToken"></param>
        public WTemplateMessager(Dictionary<MType, string> tpls, Dictionary<MType, string> tplIds)
        {
            _tpls = tpls;
            _tplIds = tplIds;
        }

        /// <summary>
        /// 发送微信模板消息
        /// </summary>
        /// <param name="info">绑定数据</param>
        /// <param name="type">微信模板消息种类</param>
        /// <returns></returns>
        public bool Send(BnTplBase info, MType type)
        {
            string jsonData = _tpls[type];
            //模板类型为其他
            if (!MType.Other.Equals(type))
                info.templateid = _tplIds[type];

            //利用反射，根据属性动态绑定模板数据
            Type t = info.GetType();
            foreach (PropertyInfo p in t.GetProperties())
            {
                if (p.GetValue(info, null) == null)
                    jsonData = jsonData.Replace("{{" + p.Name + "}}", string.Empty);
                else
                    jsonData = jsonData.Replace("{{" + p.Name + "}}", p.GetValue(info, null).ToString());
            }

            bool ret = Send(jsonData);

            return ret;

        }

        /// <summary>
        /// 发送微信模板消息
        /// </summary>
        /// <param name="jsonData">Json数据</param>
        /// <returns></returns>
        public bool Send(string jsonData)
        {
            string tokenUrl = string.Format(WConst.Url.TEMPLATE_SEND, AccessToken);

            string url = UHttp.Post(tokenUrl, jsonData);

            JToken jtoken = JToken.Parse(url);
            if (jtoken == null)
            {
                //ULog.WriteError(url, AppName);
                return false;
            }
            errcode = jtoken["errcode"].ToString();
            errmsg = jtoken["errmsg"].ToString();

            if (SUCCESS.Equals(errcode))
                return true;
            else
                //ULog.WriteError(jtoken.ToString(), AppName);

            return false;
        }

        /// <summary>
        /// 微信模板消息种类
        /// </summary>
        public enum MType
        {
            All = 0,//全部警告
            OfflineAlert = 1,//离线警告
            OfflineAlertTimeout2 = 111,//离线超过XX小时警告2级
            OfflineAlertTimeout3 = 112,//离线超过XX小时警告3级
            PlutionAlert = 12,//污染超标警告
            WashOffLineAlert = 31,//车辆冲洗设备离线报警
            UnwashedAlert = 32,//车辆未冲洗报警
            WashOffLineAlertTimeout2 = 311,//车辆冲洗设备离线超过XX小时报警2级
            WashOffLineAlertTimeout3 = 312,//车辆冲洗设备离线超过XX小时报警3级
            SpecialEqpAlert = 4,//特种设备报警
            SpecialEqpTipAlert = 42,//特种设备倾翻报警
            SpecialEqpUninstallAlert = 43,//特种设备未安装提醒
            CablelAlert = 44,//钢丝绳损伤报警
            UnloadAlert = 45,//卸料报警
            OverLoadAlert = 46,//升降机人数超载报警
            DefenceAlert = 5,//临边围挡报警
            HelmetAlert = 61,//安全帽佩戴识别
            StrangerAlert = 62,//陌生人进场识别
            TrespasserAlert = 63,//人车分流识别
            FireAlert = 64,//堆放火险
            CameraOfflineAlert = 71,//摄像头设备离线
            Other = 999,//其他
        }
    }
}
