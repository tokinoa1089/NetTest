using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatHostAnyListen
{

    class S
    {
        const string eof = "<EOF>";

        public static void Main()
        {
            Console.WriteLine("ChatHost Any listen");
            SocketServer();
            Console.ReadKey();
        }

        public static void SocketServer()
        {
            //ここからIPアドレスやポートの設定
            // 着信データ用のデータバッファー。
            byte[] bytes = new byte[10];
            string hostName = Dns.GetHostName();


            Console.WriteLine($"this hostName is {hostName}.");
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);

            foreach (var addresslist in ipHostInfo.AddressList)
            {
                Console.WriteLine($"found own address:{addresslist.ToString()}");
            }
            Console.Write($"Select address to listen(0 - {ipHostInfo.AddressList.Length - 1}):");
            IPAddress ipAddress = ipHostInfo.AddressList[int.Parse(Console.ReadLine())];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            //ここまでIPアドレスやポートの設定

            //ソケットの作成
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //通信の受け入れ準備
            listener.Bind(localEndPoint);
            listener.Listen(10);

            //通信の確立
            Socket handler = listener.Accept();

            // 任意の処理
            //データの受取をReceiveで行う。
            string gotEof = "";
            while (true)
            {
                int bytesRec;
                try
                {
                    bytesRec = handler.Receive(bytes);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
                    return;
                }
                if (bytesRec == 0)
                {
                    Console.WriteLine("異常なデータ受信（0バイト）");
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    return;
                }
                string data1 = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                Console.WriteLine($"Client:{data1}");
                var i = 0;
                int data1Id = 0;
                do
                {
                    i = data1.Substring(data1Id, data1.Length - data1Id).IndexOf(eof.Substring(gotEof.Length, 1));
                    if (i != -1)
                    {   //文字が見つかった
                        gotEof += data1.Substring(data1Id+i, 1);
                        data1Id += i;
                        if (gotEof == eof)
                        {
                            break;
                        }
                    }
                    else
                    {   //文字が見つからない
                        if (data1Id == 0)
                        {   //全体で見つかっていない
                            break;
                        }
                        else if (gotEof.Length!=0)
                        {   //EOFが半端に見つかっている
                            if (data1Id == data1.Length - 1)
                            {   //最後に見たのが受信文字列の最後
                                break;
                            }
                            else
                            {
                                gotEof = "";
                            }
                        }
                    }
                }
                while (true);

                if (gotEof.Contains(eof))
                {
                    break;
                }
            }
            //文字列を入力
            Console.Write("Host：");
            string inputSt = Console.ReadLine();
            byte[] msg = Encoding.UTF8.GetBytes(inputSt + eof);
            //クライアントにSendで返す。
            handler.Send(msg);

            //ソケットの終了
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}


