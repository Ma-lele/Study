using Fleck;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XHS.Build.Common.Configs;
using XHS.Build.Common.Response;
using XHS.Build.Model.NetModels.Dtos;

namespace XHS.Build.Common.Fleck
{
    /// <summary>
    /// 特种设备WebSocket
    /// </summary>
    public class FleckSpecial : IFleckSpecial
    {
        private readonly ILogger<FleckSpecial> _logger;
        private readonly WsConfig _wsConfig;
        private static WebSocketServer _server;
        private static List<WSConnection> _sockets = new List<WSConnection>();//配置地址
        private static List<string> _seCodes = new List<string>();//有效设备列表
        public List<string> SeCodes { get => _seCodes; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        public FleckSpecial(WsConfig wsConfig, ILogger<FleckSpecial> logger)
        {
            _wsConfig = wsConfig;
            _logger = logger;
        }

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="action">处理</param>
        /// <param name="output">内容</param>
        /// <param name="isOk">是否OK</param>
        /// <returns></returns>
        private string _Output(string action = null, object output = null, bool isOk = false)
        {
            string result = JsonConvert.SerializeObject(ResponseOutput.NotOk(action));
            try
            {
                if (isOk)
                    result = JsonConvert.SerializeObject(ResponseOutput.Ok(output, action));
                else
                    result = JsonConvert.SerializeObject(ResponseOutput.NotOk(action, output));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[_Output]: {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// 服务启动
        /// </summary>
        public void Start()
        {
            if (_wsConfig == null || string.IsNullOrEmpty(_wsConfig.wsAddress))
                return;

            try
            {
                //管理Socket
                _server = new WebSocketServer(_wsConfig.wsAddress);

                //出错后进行重启
                _server.RestartAfterListenError = true;

                //开始监听
                _server.Start(socket =>
                {
                    //关联连接建立事件
                    socket.OnOpen = () =>
                        {
                            _logger.LogInformation($"[OnOpen] {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort}({socket.ConnectionInfo.Id}) has been connected!");
                            var wsc = new WSConnection(socket);
                            socket.Send(_Output(null, socket.ConnectionInfo.Id, true));
                            _sockets.Add(wsc);
                        };

                    //关联连接关闭事件
                    socket.OnClose = () =>
                        {
                            var wsc = _sockets.Find(wsc => wsc.Connection == socket);
                            var deviceCode = wsc.DeviceCode;
                            if (string.IsNullOrEmpty(deviceCode))
                                _logger.LogInformation($"[OnClose] {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort} [{deviceCode}] has been closed!");
                            else
                                _logger.LogInformation($"[OnClose] {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort} has been closed!");

                            //先移除该连接
                            _sockets.Remove(wsc);

                            if (!wsc.IsPusher)
                            {   //看看还有没有相同设备的连接，没了，就清除。
                                wsc = _sockets.Find(wsc => wsc.DeviceCode == deviceCode);
                                if (wsc == null)
                                {
                                    _seCodes.Remove(deviceCode);
                                    _UpdateDeviceList();
                                }
                            }
                            _logger.LogInformation($"[OnClose] :{JsonConvert.SerializeObject(_seCodes)}");
                        };

                    //接受客户端消息事件
                    socket.OnMessage = message =>
                        {
                            try
                            {
                                var wsc = _sockets.Find(wsc => wsc.Connection == socket);
                                if (wsc == null)
                                {
                                    socket.Send(_Output());
                                    return;
                                }
                                _logger.LogInformation($"[OnMessage] {socket.ConnectionInfo.Id}:{message}");

                                var request = JsonConvert.DeserializeObject<WSRequest>(message);

                                if (string.Equals(request.Action, Convert.ToString(Actions.Client), StringComparison.OrdinalIgnoreCase))
                                {//客户端
                                    var deviceCode = _ClientRegist(Convert.ToString(request.Data));
                                    if (string.IsNullOrEmpty(deviceCode))
                                        socket.Send(_Output(request.Action));
                                    else
                                    {
                                        if(!string.IsNullOrEmpty(wsc.DeviceCode) && _seCodes.Contains(wsc.DeviceCode))
                                        {//切换设备的链接,从列表中移除原设备编号
                                            _seCodes.Remove(wsc.DeviceCode);
                                        }
                                        wsc.DeviceCode = deviceCode;
                                        socket.Send(_Output(request.Action, socket.ConnectionInfo.Id, true));
                                        if (!_seCodes.Contains(deviceCode))
                                        {
                                            _seCodes.Add(deviceCode);
                                            _UpdateDeviceList();
                                        }
                                        _logger.LogInformation($"[OnMessage-client] :{JsonConvert.SerializeObject(_seCodes)}");
                                    }
                                    return;
                                }
                                else if (string.Equals(request.Action, Convert.ToString(Actions.Pusher), StringComparison.OrdinalIgnoreCase))
                                {//数据来源
                                    wsc.IsPusher = _PusherRegist(Convert.ToString(request.Data));
                                    if (wsc.IsPusher)
                                    {
                                        socket.Send(_Output(request.Action, socket.ConnectionInfo.Id, true));
                                        socket.Send(_Output(Actions.DeviceList.ToString(), _seCodes, true));
                                    }
                                    else
                                        socket.Send(_Output(request.Action, "Who the FAY!"));
                                    //打印现有设备列表
                                    _logger.LogInformation($"[OnMessage-IsPusher] :{JsonConvert.SerializeObject(_seCodes)}");
                                    return;
                                }
                                else if (string.Equals(request.Action, Convert.ToString(Actions.RealData), StringComparison.OrdinalIgnoreCase))
                                {//实时数据
                                    if (wsc.IsPusher)
                                        Distpatch(request.Data);
                                    else
                                        socket.Send(_Output(request.Action, "你的身份不对头!"));
                                    return;
                                }
                                else
                                    socket.Send(_Output(request.Action));
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex.Message);
                                socket.Send(_Output(null, "哎呀...不好说唉..."));
                            }
                        };

                    socket.OnPing = message =>
                    {
                        _logger.LogInformation($"OnPing {socket.ConnectionInfo.Id}:{Encoding.UTF8.GetString(message)}");
                    };

                    socket.OnPong = message =>
                    {
                        _logger.LogInformation($"OnPong {socket.ConnectionInfo.Id}:{Encoding.UTF8.GetString(message)}");
                    };

                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Start]: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新设备列表
        /// </summary>
        private void _UpdateDeviceList()
        {
            var wsList = _sockets.FindAll(wsc => wsc.IsPusher);
            string jsonString = JsonConvert.SerializeObject(_seCodes);
            foreach (var wsc in wsList)
            {
                try
                {
                    wsc.Connection.Send(_Output(Actions.DeviceList.ToString(), _seCodes, true));
                    _logger.LogInformation($"{jsonString} send to {wsc.Connection.ConnectionInfo.ClientIpAddress}!");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[_UpdateDeviceList]: {ex.Message}");
                    continue;
                }
            }
        }

        /// <summary>
        /// 注册客户端
        /// </summary>
        /// <param name="data">验证数据</param>
        /// <returns></returns>
        private string _ClientRegist(string data)
        {
            try
            {
                var client = JsonConvert.DeserializeObject<ClientEntity>(UEncrypter.DecryptByRSA16(data, _wsConfig.privateKey));
                if (DateTime.Now.AddMinutes(-10) < client.DateTime && DateTime.Now.AddMinutes(10) > client.DateTime)
                    return client.DeviceCode;//10分钟之内有效
            }
            catch (Exception ex)
            {
                _logger.LogError($"[_ClientRegist]: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// 注册推手
        /// </summary>
        /// <param name="data">验证数据</param>
        /// <returns></returns>
        private bool _PusherRegist(string data)
        {
            try
            {
                var client = JsonConvert.DeserializeObject<ClientEntity>(UEncrypter.DecryptByRSA16(data, _wsConfig.privateKey));
                if (DateTime.Now.AddMinutes(-10) < client.DateTime && DateTime.Now.AddMinutes(10) > client.DateTime)
                    return true;//10分钟之内有效
            }
            catch (Exception ex)
            {
                _logger.LogError($"[_PusherRegist]: {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// 转发实时数据
        /// </summary>
        /// <param name="realData">实时数据</param>
        public void Distpatch<T>(T realData)
        {
            JObject jdata;
            try
            {
                if (realData is string)
                {
                    if (string.IsNullOrEmpty(realData as string))
                        return;
                    jdata = JObject.Parse(realData as string);
                }
                else
                {
                    jdata = JObject.FromObject(realData);
                }

                if (jdata["SeCode"] == null)
                    return;
                //var tcData = JsonConvert.DeserializeObject<TowerCraneRealDataInput>(data);
                var wsList = _sockets.FindAll(wsc => wsc.DeviceCode == jdata["SeCode"].ToString());
                foreach (var wsc in wsList)
                {
                    try
                    {
                        wsc.Connection.Send(_Output(Actions.RealData.ToString(), jdata, true));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[_RealDataTrans]: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// WebSocketConnection
    /// </summary>
    public class WSConnection
    {
        public WSConnection(IWebSocketConnection connection, string deviceCode = null)
        {
            Connection = connection;
            DeviceCode = deviceCode;
        }

        /// <summary>
        /// 设备唯一编号
        /// </summary>
        public string DeviceCode { get; set; }
        /// <summary>
        /// WebSocketConnection
        /// </summary>
        public IWebSocketConnection Connection { get; set; }
        /// <summary>
        /// 是否为数据来源
        /// </summary>
        public bool IsPusher { get; set; }

    }

    /// <summary>
    /// WebSocket请求
    /// </summary>
    public class WSRequest
    {
        /// <summary>
        /// 方法
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 1:塔吊,2:升降机
        /// </summary>
        //public int SeType { get; set; } = 1;
        //public string DeviceCode { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }
    }

    /// <summary>
    /// WebSocket请求
    /// </summary>
    public class WSResponse
    {
        /// <summary>
        /// 方法
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 1:塔吊,2:升降机
        /// </summary>
        //public int SeType { get; set; } = 1;
        //public string DeviceCode { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }
    }

    /// <summary>
    /// 客户端认证信息
    /// </summary>
    public class ClientEntity
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceCode { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime DateTime { get; set; }
    }

    /// <summary>
    /// 操作枚举
    /// </summary>
    public enum Actions
    {
        /// <summary>
        /// 注册客户端
        /// </summary>
        Client = 1,
        /// <summary>
        /// 注册推手
        /// </summary>
        Pusher = 2,
        /// <summary>
        /// 实时数据中转
        /// </summary>
        RealData = 3,
        /// <summary>
        /// 设备列表(没用)
        /// </summary>
        DeviceList = 4,
    }

}
