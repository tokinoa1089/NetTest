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
        public ChatSystem()
        {
            _hostName = Dns.GetHostName();
        }
        private IPAddress _ipAddress;
        private Int32 _portNo;

        private IPEndPoint _localEndPoint;

        public void SetConnectMode(ConnectMode connectMode)
        {
            _connectMode = connectMode;
        }

        public void InitializeHost(IPAddress ipAddress,Int32 portNo )
        {
            _connectMode = ConnectMode.host;
            _ipAddress = ipAddress;
            _portNo = portNo;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, portNo);
            //ソケットの作成
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //通信の受け入れ準備
            listener.Bind(localEndPoint);
            listener.Listen(10);
            //通信の確立
            Socket handler = listener.Accept();
        }
        public bool InitializeClient(IPAddress ipAddress, Int32 portNo)
        {
            _connectMode = ConnectMode.client;
            _ipAddress = ipAddress;
            _portNo = portNo;
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, portNo);
            //ソケットを作成
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //接続する。失敗するとエラーで落ちる。
            try
            {
                socket.Connect(remoteEP);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Connect Faild{e.ToString()}");
                return false;
            }
            return true;
        }

    }

}
