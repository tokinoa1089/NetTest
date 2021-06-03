using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace socketC
{
    class C
    {
        public static void Main()
        {
            Console.WriteLine("[SimpleClientSelectHost]");
            //今回送るHello World!
            string st = "Hello World!";
            SocketClient(st);
        }


        public static void SocketClient(string st)
        {
            //IPアドレスやポートを設定(自PC、ポート:11000）
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            Console.Write("Input IP address to connect:");
            var ipAddress = IPAddress.Parse(Console.ReadLine());
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);


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
                return;
            }
            //Sendで送信している。
            byte[] msg = Encoding.UTF8.GetBytes(st + "<EOF>");
            int sent = socket.Send(msg);

            //Receiveで受信している。
            byte[] bytes = new byte[1024];
            int bytesRec = socket.Receive(bytes);
            string data1 = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            Console.WriteLine(data1);

            //ソケットを終了している。
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}

