
using System.Collections.Generic;
using System.Net.WebSockets;
using XHS.Build.Common.Helps;

namespace XHS.Build.Common.Fleck
{
    public interface IFleckSpecialClient
    {
        List<string> Secodes();

        WebSocketState? GetState();

        void Start();


        void Close();

        void Open();


        void Send(string msg);
    }
}
