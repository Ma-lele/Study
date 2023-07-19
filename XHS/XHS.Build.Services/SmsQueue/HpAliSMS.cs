using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using XHS.Build.Common;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Util;
using XHS.Build.Common.Wechat;
using XHS.Build.Repository.SystemSetting;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Services.SmsQueue
{
    public class HpAliSMS
    {

        private readonly IHpSystemSetting _hpSystemSetting;
        public HpAliSMS(IHpSystemSetting hpSystemSetting)
        {
            _hpSystemSetting = hpSystemSetting;
        }

        //产品名称:云通信短信API产品,开发者无需替换
        private const String product = "Dysmsapi";
        //产品域名,开发者无需替换
        private const String domain = "dysmsapi.aliyuncs.com";
        //private readonly IHpSystemSetting _hpSystemSetting;
        // TODO 此处需要替换成开发者自己的AK(在阿里云访问控制台寻找)
        private static String ACCESSKEYID = "LTAINshPgBjroNGr"; //Properties.Settings.Default.SmsKey;
        private static String ACCESSKEYSECRET = "V0XCywvbk5dzHl0tY7TRBq43BRilNp"; //Properties.Settings.Default.SmsSecret;
        private static String SIGNNAME = "智慧工地综合管理平台"; //Properties.Settings.Default.SmsSignName;
        private static IClientProfile PROFILE = DefaultProfile.GetProfile("cn-hangzhou", ACCESSKEYID, ACCESSKEYSECRET);
        private static IAcsClient ACSCLIENT = new DefaultAcsClient(PROFILE);
        public string URL_DOMAIN
        {
            get
            {
                return _hpSystemSetting.getSettingValue(Const.Setting.S019);
            }
        }
        public const string URL_WARN = "http://{0}:9027/Services/WSTemplateMessage.svc";
        private const string STR_WARN = "调用了{0}的{1}报警服务！[{2}]";
        public string Code = string.Empty;
        public string Message = string.Empty;
        /// <summary>
        /// 手机验证码
        /// </summary>
        public static string MOTION_VERIFYCODE = "SMS_228115581";
        /// <summary>
        /// 设备离线报警
        /// </summary>
        public static string TPL_OFFLINE = "SMS_171114637";
        /// <summary>
        /// 设备离线超时报警
        /// </summary>
        public static string TPL_OFFLINE_TIMEOUT = "SMS_180342236";
        /// <summary>
        /// 污染超标报警
        /// </summary>
        public static string TPL_OVERFLOW = "SMS_158944656";
        /// <summary>
        /// 车辆冲洗设备离线报警
        /// </summary>
        public static string TPL_WASHOFFLINE = "SMS_179150148";
        /// <summary>
        /// 车辆冲洗设备离线超时报警
        /// </summary>
        public static string TPL_WASHOFFLINE_TIMEOUT = "SMS_180352083";
        /// <summary>
        /// 车辆未冲洗报警
        /// </summary>
        public static string TPL_UNWASHED = "SMS_179150230";
        /// <summary>
        /// 特种设备限位报警
        /// </summary>
        public static string TPL_SPECIALEQP = "SMS_168415088";
        /// <summary>
        /// 特种设备倾翻报警
        /// </summary>
        public static string TPL_SPECIALEQPTIP = "SMS_183765076";
        /// <summary>
        /// 特种设备未安装提醒
        /// </summary>
        public static string TPL_SPECIALEQPUNINSTALL = "SMS_185570338";
        /// <summary>
        /// 钢丝绳损伤报警
        /// </summary>
        public static string TPL_CABLE = "SMS_185842833";
        /// <summary>
        /// 卸料报警
        /// </summary>
        public static string TPL_UNLOAD = "SMS_186575388";
        /// <summary>
        /// 升降机人数超载识别
        /// </summary>
        public static string TPL_OVERLOAD = "SMS_192720004";
        /// <summary>
        /// 临边围挡报警
        /// </summary>
        public static string TPL_DEFENCE = "SMS_174725237";
        /// <summary>
        /// 安全帽佩戴识别
        /// </summary>
        public static string TPL_HELMET = "SMS_190265662";
        /// <summary>
        /// 陌生人进场识别
        /// </summary>
        public static string TPL_STRANGER = "SMS_192710054";
        /// <summary>
        /// 人车分流识别
        /// </summary>
        public static string TPL_TRESPASSER = "SMS_192542866";
        /// <summary>
        /// 堆场火警识别
        /// </summary>
        public static string TPL_FIRE = "SMS_192542867";
        /// <summary>
        /// 摄像头离线报警
        /// </summary>
        public static string TPL_CAMERA_OFF = "SMS_190277278";

        /// <summary>
        /// 设备离线次数超限报警
        /// </summary>
        public static string TPL_TIMES_OFFLINE = "SMS_179606497";
        /// <summary>
        /// 污染超标次数超限报警
        /// </summary>
        public static string TPL_TIMES_OVERFLOW = "SMS_179616515";
        /// <summary>
        /// 车辆冲洗设备离线次数超限报警
        /// </summary>
        public static string TPL_TIMES_WASHOFFLINE = "SMS_179606506";
        /// <summary>
        /// 车辆未冲洗次数超限报警
        /// </summary>
        public static string TPL_TIMES_UNWASHED = "SMS_179606508";
        /// <summary>
        /// 特种设备限位次数超限报警
        /// </summary>
        public static string TPL_TIMES_SPECIALEQP = "SMS_179616530";
        /// <summary>
        /// 特种设备倾翻次数超限报警
        /// </summary>
        public static string TPL_TIMES_TIPOVER = "SMS_189611536";
        /// <summary>
        /// 临边围挡次数超限报警
        /// </summary>
        public static string TPL_TIMES_DEFENCE = "SMS_179601461";
        /// <summary>
        /// 报警次数超限报警
        /// </summary>
        public static string TPL_TIMES_OVER = "SMS_189611559";
        /// <summary>
        /// 移动巡检新增提醒
        /// </summary>
        public static string TPL_ROUND_NEW = "SMS_189613231";
        /// <summary>
        /// 告警更新提醒
        /// </summary>
        public static string TPL_WARN_UPDATE = "SMS_195226820";
        /// <summary>
        /// 摄像头离线次数超限报警
        /// </summary>
        public static string TPL_TIMES_CAMERA_OFF = "SMS_190278829";
        /// <summary>
        /// 移动巡检更新提醒
        /// </summary>
        public static string TPL_ROUND_UPDATE = "SMS_189613238";

        //public static Dictionary<WTemplateMessager.MType, string> DicTpl = new Dictionary<WTemplateMessager.MType, string>()
        //{
        //    {MType.OfflineAlert, "SMS_171114637"},      //设备离线报警
        //    {MType.PlutionAlert, "SMS_158944656"},      //污染超标报警
        //    {MType.WashOffLineAlert, "SMS_179150148"},  //车辆冲洗设备离线报警
        //    {MType.UnwashedAlert, "SMS_179150230"},     //车辆未冲洗报警
        //    {MType.SpecialEqpAlert, "SMS_168415088"},   //特种设备限位报警
        //    {MType.SpecialEqpTipAlert, "SMS_183765076"},//特种设备倾翻报警
        //    {MType.CablelAlert, "SMS_185842833"},       //钢丝绳损伤报警
        //    {MType.UnloadAlert, "SMS_186575388"},       //卸料报警
        //    {MType.DefenceAlert, "SMS_174725237"},      //临边围挡警告
        //};

        //public static Dictionary<WTemplateMessager.MType, string> DicTimesTpl = new Dictionary<WTemplateMessager.MType, string>()
        //{
        //    {MType.OfflineAlert, "SMS_179606497"},              //设备离线次数超限报警
        //    {MType.OfflineAlertTimeout2, "SMS_180342236"},      //离线超时2级警告
        //    {MType.OfflineAlertTimeout3, "SMS_180342236"},      //离线超时3级警告
        //    {MType.PlutionAlert, "SMS_179616515"},              //污染超标次数超限报警
        //    {MType.WashOffLineAlert, "SMS_179606506"},  //车辆冲洗设备离线次数超限报警
        //    {MType.WashOffLineAlertTimeout2, "SMS_180352083"},  //车辆冲洗设备离线超时2级报警
        //    {MType.WashOffLineAlertTimeout3, "SMS_180352083"},  //车辆冲洗设备离线超时3级报警
        //    {MType.UnwashedAlert, "SMS_179606508"},      //车辆未冲洗次数超限报警
        //    {MType.SpecialEqpAlert, "SMS_179616530"},//特种设备限位次数超限警告
        //    {MType.SpecialEqpTipAlert, "SMS_189611536"},//特种设备倾翻次数超限警告
        //    {MType.CablelAlert, "SMS_185842833"},//钢丝绳损伤报警
        //    {MType.UnloadAlert, "SMS_189611559"},//报警次数超限报警 卸料报警
        //    {MType.DefenceAlert, "SMS_179601461"},//临边围挡次数超限警告
        //};


        /// <summary>
        /// 发送短信
        /// </summary> 
        /// <param name="PhoneNumber">电话号码</param>
        /// <param name="TemplateCode">模板Code</param>
        /// <param name="JsonParam">Json参数</param>
        /// <returns></returns>
        public static string SendSms(string PhoneNumber, string TemplateCode, string JsonParam)
        {
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            SendSmsRequest request = new SendSmsRequest();
            SendSmsResponse response = null;
            string result = string.Empty;
            try
            {

                //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
                request.PhoneNumbers = PhoneNumber;
                //必填:短信签名-可在短信控制台中找到
                request.SignName = SIGNNAME;
                //必填:短信模板-可在短信控制台中找到
                request.TemplateCode = TemplateCode;
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                request.TemplateParam = JsonParam;
                //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                //request.OutId = "yourOutId";
                //请求失败这里会抛ClientException异常
                response = ACSCLIENT.GetAcsResponse(request);

                result = response.Message;

            }
            catch (ServerException ex)
            {
                result = ex.Message;
                //ULog.WriteError("ServerException " + result, Properties.Settings.Default.AppName);
            }
            catch (ClientException ex)
            {
                result = ex.Message;
                //ULog.WriteError("ClientException " + result, Properties.Settings.Default.AppName);
            }
            catch (Exception ex)
            {
                result = ex.Message;
                //ULog.WriteError(result, Properties.Settings.Default.AppName);
            }

            return result;

        }

        /// <summary>
        /// 根据WARNID即时发送报警
        /// </summary>
        /// <param name="WARNID">AI报警ID</param>
        /// <param name="appName">应用名称</param>
        /// <returns></returns>
        public int SendWarnById(int WARNID, string appName)
        {
            int ret = 0;
            try
            {
                object result = HpWcfInvoker.ExecuteMethod<IWSTemplateMessage>(string.Format(URL_WARN, URL_DOMAIN), "sendById", new Dictionary<string, string>() { { "WARNID", WARNID.ToString() } });

                if (result != null && !Convert.ToInt32(result).Equals(0))
                {//如果发生了发信才写日志
                    //ULog.Write(string.Format(STR_WARN, URL_DOMAIN, "单发", result), appName);
                }
                else
                    ret = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message + Environment.NewLine + ex.StackTrace, appName);
            }
            return ret;
        }

        /// <summary>
        /// 根据WARNID即时发送AI报警
        /// </summary>
        /// <param name="WARNID">AI报警ID</param>
        /// <param name="appName">应用名称</param>
        /// <returns></returns>
        public int SendWarnAIById(int WARNID, string appName)
        {
            int ret = 0;
            try
            {
                string XML46 = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Header/><s:Body><sendAIById xmlns = \"http://tempuri.org/\"><WARNID>{0}</WARNID></sendAIById></s:Body></s:Envelope>";
                string xml = string.Format(XML46, WARNID);
                WebHeaderCollection header = new WebHeaderCollection();
                header["SOAPAction"] = "http://tempuri.org/IWSTemplateMessage/sendAIById";
                string result = UHttp.Post(string.Format(URL_WARN, URL_DOMAIN), xml, UHttp.CONTENT_TYPE_TEXT_XML, header);
                //object result = HpWcfInvoker.ExecuteMethod<IWSTemplateMessage>(string.Format(URL_WARN, URL_DOMAIN), "sendAIById", WARNID);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);//Load加载XML文件，LoadXML加载XML字符串
                string value = xmlDoc.HasChildNodes && xmlDoc.DocumentElement["s:Body"].HasChildNodes ? xmlDoc.DocumentElement["s:Body"]["sendAIByIdResponse"]["sendAIByIdResult"].InnerXml : "";

                if (value != null && !Convert.ToInt32(value).Equals(1))
                {
                    //如果发生了发信才写日志
                    //ULog.Write(string.Format(STR_WARN, URL_DOMAIN, "AI单发", result), appName);
                }
                else
                    ret = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message + Environment.NewLine + ex.StackTrace, appName);
            }
            return ret;
        }

        /// <summary>
        /// 即时发送所有报警
        /// </summary>
        /// <param name="appName">应用名称</param>
        public int SendWarnAll(string appName)
        {
            int ret = 0;
            try
            {
                //string uri = "http://test.xhs-sz.com:9027/Services/WSTemplateMessage.svc";
                object result = HpWcfInvoker.ExecuteMethod<IWSTemplateMessage>(string.Format(URL_WARN, URL_DOMAIN), "sendByType", new Dictionary<string, string>() { { "type", WTemplateMessager.MType.All.ToString() } });

                if (result != null && !Convert.ToInt32(result).Equals(0))
                {//如果发生了发信才写日志
                    //ULog.Write(string.Format(STR_WARN, URL_DOMAIN, "全类别", result), appName);
                }
                else
                    ret = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message + Environment.NewLine + ex.StackTrace, appName);
            }
            return ret;
        }

    }



    public interface IWSTemplateMessage
    {
        /// <summary>
        /// 群发模板消息
        /// </summary>
        /// <param name="type">微信模板消息种类</param>
        /// <returns></returns>
        int sendByType(WTemplateMessager.MType type);
        /// <summary>
        /// 单发模板消息
        /// </summary>
        /// <param name="WARNID">警告ID</param>
        /// <returns></returns>
        int sendById(int WARNID);

        /// <summary>
        /// 单发模板消息
        /// </summary>
        /// <param name="WARNID">AI警告ID</param>
        /// <returns></returns>
        int sendAIById(int WARNID);

        /// <summary>
        /// 特种设备未安装提醒
        /// </summary>
        /// <returns></returns>
        int sendSeAlert();

    }
}
