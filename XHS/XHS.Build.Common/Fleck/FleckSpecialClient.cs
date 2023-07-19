using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;

namespace XHS.Build.Common.Fleck
{
    /// <summary>
    /// 特种设备WebSocket
    /// </summary>
    public class FleckSpecialClient : IFleckSpecialClient
    {
        private readonly ILogger<FleckSpecialClient> _logger;
        private readonly WsConfig _wsConfig;
        private WSocketClientHelp _wSocketClient;
        private List<string> _secodes = new List<string>();  //有效设备列表
        private DateTime errtime = DateTime.Now;
        private bool iserr = true;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        public FleckSpecialClient(WsConfig wsConfig, ILogger<FleckSpecialClient> logger)
        {
            _wsConfig = wsConfig;
            _logger = logger;
        }


        /// <summary>
        /// 服务启动
        /// </summary>
        public void Start()
        {
            try
            {
                _wSocketClient = new WSocketClientHelp(_wsConfig.wsAddress);
                _wSocketClient.OnOpen -= WSocketClient_OnOpen;
                _wSocketClient.OnMessage -= WSocketClient_OnMessage;
                _wSocketClient.OnClose -= WSocketClient_OnClose;
                _wSocketClient.OnError -= WSocketClient_OnError;

                _wSocketClient.OnOpen += WSocketClient_OnOpen;
                _wSocketClient.OnMessage += WSocketClient_OnMessage;
                _wSocketClient.OnClose += WSocketClient_OnClose;
                _wSocketClient.OnError += WSocketClient_OnError;
                Open();
            }
            catch (WebSocketException ex)
            {
                _logger.LogError($"[WSocketClient_Start]: {ex.Message}");
                errtime = DateTime.Now;
                iserr = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[WSocketClient_Start]: {ex.Message}");
            }

        }
       
        private void WSocketClient_OnError(object sender, Exception ex)
        {
            _logger.LogError($"[WSocketClient_OnError]: {ex.Message}");
            errtime = DateTime.Now;
            iserr = true;
        }

        private void WSocketClient_OnClose(object sender, EventArgs e)
        {
            _logger.LogInformation($"[WSocketClient_OnClose]: 链接断开了。");
            Open();
        }

        private void WSocketClient_OnMessage(object sender, string data)
        {
            //处理的消息错误将会忽略
            try
            {
                _logger.LogInformation($"[WSocketClient_OnMessage]: {data}");
                var result = JsonConvert.DeserializeObject<DeviceResponse<dynamic>>(data);
                if (result.success && result.msg != null && result.msg.ToLower() == "devicelist")
                {
                    _secodes = JsonConvert.DeserializeObject<List<string>>(result.data.ToString());
                    iserr = false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"[WSocketClient_OnMessage]: {ex.Message}");
                errtime = DateTime.Now;
                iserr = true;
            }

        }

        private void WSocketClient_OnOpen(object sender, EventArgs e)
        {

        }

        public void Close()
        {
            _wSocketClient.Close();
        }

        public void Open()
        {
            try { 
                DateTime now = DateTime.Now;
                JObject mJObj = new JObject();
                mJObj.Add("DeviceCode", "pusher");
                mJObj.Add("DateTime", now);
                string encryptStr = UEncrypter.EncryptByRSA16(mJObj.ToString(), _wsConfig.publicKey);
                JObject mJObj2 = new JObject();
                mJObj2.Add("Action", "pusher");
                mJObj2.Add("Data", encryptStr);
                _wSocketClient.Open(mJObj2.ToString());
            }
            catch (WebSocketException ex)
            {
                _logger.LogError($"[WSocketClient_Open]: {ex.Message}");
                errtime = DateTime.Now;
                iserr = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[WSocketClient_Open]: {ex.Message}");
            }
        }

        public void Send(string msg)
        {
            _wSocketClient.Send(msg);
        }

        public WebSocketState? GetState()
        {
            return _wSocketClient.State;
        }

        public List<string> Secodes()
        {
            if(iserr && DateTime.Now.AddMinutes(-10).CompareTo(errtime) > 0)
            {
                Start();
                errtime = DateTime.Now;
            }
            return _secodes;
        }
    }

}
