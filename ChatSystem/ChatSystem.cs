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
        private Socket _connectSocet = null;
        private Socket _chatSocket=null;
        private int _maxChatLength;

        public ChatSystem(int maxChatLength)
        {
            _maxChatLength = maxChatLength;
            _hostName = Dns.GetHostName();
        }
        /// <summary>
        /// Initialize as a Host
        /// </summary>
        /// <param name="ipAddress">ip Addressa</param>
        /// <param name="portNo"> port No</param>
        public (bool sucess, Exception e) InitializeHost(IPAddress ipAddress,Int32 portNo )
        {
            _connectMode = ConnectMode.host;
            _ipAddress = ipAddress;
            _portNo = portNo;
             _localEndPoint = new IPEndPoint(ipAddress, portNo);
            //接続のためのソケットを作成
            _connectSocet = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //通信の受け入れ準備
            try
            {
                _connectSocet.Bind(_localEndPoint);
            }
            catch (Exception e)
            {
                return (false, e);
            }
            try
            {
                _connectSocet.Listen(10);
            }
            catch (Exception e)
            {
                return (false, e);
            }
            //通信の確立
            try
            {
                _chatSocket = _connectSocet.Accept();
            }
            catch (Exception e)
            {
                return (false, e);
            }
            return (true, null);
        }
        /// <summary>
        /// Initialize as a Client
        /// </summary>
        /// <param name="ipAddress">ipAddress</param>
        /// <param name="portNo">portNo</param>
        /// <param name="e">Exception</param>
        /// <returns>bool result</returns>
        public (bool sucess,Exception e) InitializeClient(IPAddress ipAddress, Int32 portNo)
        {
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
               
                return( false,err);
            }
            _chatSocket = _connectSocet;
            return (true, null);
        }
        /// <summary>
        /// Receive connected Socket
        /// </summary>
        /// <returns>Suceed ,received string or ErrorMessage</returns>
        public (bool sucess, SocketException e,string buffrer) Receive( int bufferSize)
        {
            byte[] bytes = new byte[bufferSize];
            if (_chatSocket != null)
            {   // 初期化済み
                int bytesRec;
                try
                {
                    bytesRec = _chatSocket.Receive(bytes);
                }
                catch (SocketException e)
                {
                    return(false, e,null);
                }
                // 正常に受信
                return (true, null, bytes.ToString());
            }
            else
            {
                return (false, null, "not Initialize");
            }
        }
        public (bool sucess, SocketException e) Send(byte[] msg)
        {
            try
            {
                _chatSocket.Send(msg);
            }
            catch(SocketException e)
            {   //ソケットへのアクセスを試行しているときにエラーが発生しました。
                return (false, e);
            }
            return (true, null);

        }

        public void ShutDownColse()
        {
            _connectSocet.Shutdown(SocketShutdown.Both);
            _connectSocet.Close();
            _connectSocet = null;
            return;
        }
        
    }

}
