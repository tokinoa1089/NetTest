using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ChatSystem
{
    class ChatSystem
    {
        private string _hostName;
        public string hostName
        {
            get => _hostName;
        }
        public enum ConnectMode { host = 0, client = 1 };
        private ConnectMode _connectMode;
        public ConnectMode connectMode
        {
            get => _connectMode;
        }

        private IPAddress _ipAddress;
        private Int32 _portNo;
        private IPEndPoint _localEndPoint;
        private Socket _connectSocet;
        private Socket _chatSocket;

        public ChatSystem()
        {
            _hostName = Dns.GetHostName();
        }

        public void InitializeHost(IPAddress ipAddress,Int32 portNo )
        {
            _connectMode = ConnectMode.host;
            _ipAddress = ipAddress;
            _portNo = portNo;
             _localEndPoint = new IPEndPoint(ipAddress, portNo);
            //接続のためのソケットを作成
            _connectSocet = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //通信の受け入れ準備
            _connectSocet.Bind(_localEndPoint);
            _connectSocet.Listen(10);
            //通信の確立
            _chatSocket = _connectSocet.Accept();
        }
        public bool InitializeClient(IPAddress ipAddress, Int32 portNo,out Exception e)
        {
            e = null;
            _connectMode = ConnectMode.client;
            _ipAddress = ipAddress;
            _portNo = portNo;
            _localEndPoint = new IPEndPoint(ipAddress, portNo);
            //ソケットを作成
            _connectSocet = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //接続する。失敗するとエラーで落ちる。
            try
            {
                _connectSocet.Connect(_localEndPoint);
            }
            catch (Exception err)
            {
                e = err;
                return false;
            }
            _chatSocket = _connectSocet;
            return true;
        }
    }

}
