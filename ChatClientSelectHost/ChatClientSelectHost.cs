using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatClientSelectHost
{
    class ChatClientSelectHost
    {
        const string eof = "<EOF>";

        public static void Main()
        {
            Console.WriteLine("[ChatClientSelectHost]");
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

            //文字列を入力
            Console.Write("Client：");
            string inputSt = Console.ReadLine();
            //Sendで送信している。
            byte[] msg = Encoding.UTF8.GetBytes(inputSt +eof);
            socket.Send(msg);

            //Receiveで受信している。
            byte[] bytes = new byte[1024];
            int bytesRec;
            try
            {
                bytesRec = socket.Receive(bytes);
            }
            catch (SocketException e)
            {
                Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
                return;
            }
            if (bytesRec == 0)
            {
                Console.WriteLine("異常なデータ受信（0バイト）");
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                return;
            }
            string data1 = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            Console.WriteLine($"Host:{data1}");

            //ソケットを終了している。
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
