using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatSystem
{
    class main
    {
        static ChatSystem chatSystem;
        const Int32 portNo = 11000;
        const int maxLength = 200;
        static ChatSystem.ConnectMode connectMode;
        static string buffer = null;

        static void Main(string[] args)
        {
            chatSystem = new ChatSystem(maxLength);
            Console.WriteLine($"this hostName is {chatSystem.hostName}.");
            connectMode = SelectMode();
            InChat();

        }
        static ChatSystem.ConnectMode SelectMode()
        {
            ChatSystem.ConnectMode connectMode = ChatSystem.ConnectMode.host;
            Console.Write("Select Mode: 0=Host,1=Client\n");
            int select = int.Parse(Console.ReadLine());
            switch (select)
            {
                case 0: //Host
                    Console.WriteLine("Running Host mode");
                    InitializeHost();
                    connectMode = ChatSystem.ConnectMode.host;
                    break;
                case 1: //Client
                    Console.WriteLine("Running Client mode");
                    InitializeClient();
                    connectMode = ChatSystem.ConnectMode.client;
                    break;
                default:
                    Console.WriteLine("ERROR undefind");
                    break;
            }
            return connectMode;
        }
        static void InitializeHost()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(chatSystem.hostName);
            foreach (var addresslist in ipHostInfo.AddressList)
            {
                Console.WriteLine($"found own address:{addresslist.ToString()}");
            }
            Console.Write($"Select address to listen(0 - {ipHostInfo.AddressList.Length - 1}):");
            IPAddress ipAddress = ipHostInfo.AddressList[int.Parse(Console.ReadLine())];
            ChatSystem.Result re = chatSystem.InitializeHost(ipAddress, portNo);
            if (re.result!=ChatSystem.Result.eresult.success)
            {
                Console.WriteLine($"faled to initialize,ERROR={re.ToString()}");
            }
        }
        static void InitializeClient()
        {
            Console.Write("Input IP address to connect:");
            var ipAddress = IPAddress.Parse(Console.ReadLine());
            ChatSystem.Result re = chatSystem.InitializeClient(ipAddress, 11000);
            if (re.result == ChatSystem.Result.eresult.success)
            {
                Console.WriteLine($"Connected host {ipAddress.ToString()}");
            }
            else
            {
                Console.WriteLine($"faled to connect to host,ERROR={re.ToString()}");
            }
        }
        static void InChat()
        {
            bool turn = (connectMode == ChatSystem.ConnectMode.host);
            while (true)
            {
                if (turn)
                {   // 受信
                    (ChatSystem.Result re, string s) = chatSystem.Receive(maxLength);
                    if (re.result == ChatSystem.Result.eresult.success)
                    {
                        if (s.Length != 0)
                        {   // 正常にメッセージを受信
                            Console.WriteLine($"受信メッセージ：{s}");
                        }
                        else
                        {   // 正常に終了を受信
                            Console.WriteLine("相手から終了を受信");
                            break;
                        }
                    }
                    else
                    {   //　受信エラー
                        Console.WriteLine($"受信エラー：{re.ToString()} Error code: {re.ErrorCode}.");
                        break;
                    }
                }
                else
                {   // 送信
                    Console.Write("送るメッセージ：");
                    string inputSt = Console.ReadLine();
                    //Sendで送信
                    byte[] msg = Encoding.UTF8.GetBytes(inputSt);
                    ChatSystem.Result re = chatSystem.Send(msg);
                    if (re.result!=ChatSystem.Result.eresult.success)
                    {
                        Console.WriteLine($"送信エラー：{re.ToString()} Error code: {re.ErrorCode}.");
                        break;
                    }
                }
                turn = !turn;
            }
            chatSystem.ShutDownColse();
        }
    }
}
