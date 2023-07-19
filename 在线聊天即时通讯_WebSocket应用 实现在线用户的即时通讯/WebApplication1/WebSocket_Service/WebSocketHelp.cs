using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.WebSocket_Service
{
    public class WebSocketHelp
    {

        static List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>(); //所有连接了当前WebSocket服务器的客户
        /// <summary>
        /// 创建WebSocket服务器
        /// 同时跟踪用户上线和用户下线
        /// </summary>
        public static void WebSocketServerHost()
        {
            #region WebSocketServer
            FleckLog.Level = LogLevel.Debug;
            var server = new WebSocketServer("ws://0.0.0.0:2017"); //实例化WebSocket服务器
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    //Console.WriteLine("Open!");
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    //Console.WriteLine("Close!");
                    allSockets.Remove(socket);
                };
                //接收所有用户发的消息
                socket.OnMessage = message =>
                {
                    //Console.WriteLine(message);
                    //allSockets.ToList().ForEach(s => s.Send("Echo: " + message));

                    //保存聊天信息到表
                    IM_Content dot = new IM_Content();

                    var entity = JsonConvert.DeserializeObject<IM_Content>(message);
                    entity.Addtime = DateTime.Now;
                    dot.ToSendMes(entity);


                    // 在服务器集中找一下，当前接收人在不在不线/ 如果接收人在线，把消息马上传递给接收人
                    var fid = entity.Form_empid; //接收消息人ID
                    foreach (var sock in allSockets)
                    {
                        var  empid  = Convert.ToInt32( sock.ConnectionInfo.Path.Split('=')[1]);
                        if(fid==empid)
                        {
                            sock.Send(entity.Mes_content);
                            break;
                        }
                    }

                };
            });

            //var input = Console.ReadLine();
            //while (input != "exit")
            //{
            //    foreach (var socket in allSockets.ToList())
            //    {
            //        socket.Send(input);
            //    }
            //    input = Console.ReadLine();
            //}
            #endregion
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sid">员工ID</param>
        /// <param name="dbinfo">消息内容</param>
        public static void Send(string sid, string dbinfo)
        {
            foreach (var socket in allSockets.ToList())
            {

                // string logpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.ToLower(), "log");
                //System.IO.File.Create(Path.Combine(logpath, $"{sid}.txt"));

                if (socket.ConnectionInfo.Path.Split('=')[1] == sid)
                {
                    socket.Send(dbinfo);
                }
            }
        }
    }
}