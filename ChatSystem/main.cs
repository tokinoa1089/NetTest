using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ChatSystem;

namespace ChatSystem
{
    class main
    {
        static ChatSystem chatSystem;
        const Int32 portNo = 11000;
        const string EOF = "<EOF>";
        static readonly int maxLength = 200 + EOF.Length;
        static ChatSystem.ConnectMode connectMode;
        enum FunctionMode { chat, bot, janken, shiritori };
        static FunctionMode functionMode = FunctionMode.chat;

        static void Main(string[] args)
        {
            chatSystem = new ChatSystem(maxLength);
            Console.WriteLine($"this hostName is {chatSystem.hostName}.");
            functionMode = SelectFunction();
            connectMode = SelectMode();
            switch (functionMode)
            {
                case FunctionMode.chat:
                    InChat();
                    break;
                case FunctionMode.bot:
                    InChatBot();
                    break;
                case FunctionMode.janken:
                    Injanken();
                    break;
                default:
                    Console.WriteLine("not suported");
                    break;
            }
        }
        static FunctionMode SelectFunction()
        {
            Console.WriteLine("Select Function\n0= chat\n1=bot\n2=janken\n3=shiritori ");
            int select = int.Parse(Console.ReadLine());
            FunctionMode[] function = { FunctionMode.chat, FunctionMode.bot, FunctionMode.janken, FunctionMode.shiritori };
            return function[select];
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
            ChatSystem.EResult re = chatSystem.InitializeHost(ipAddress, portNo);
            if (re != ChatSystem.EResult.success)
            {
                Console.WriteLine($"failed to initialize,ERROR={re.ToString()}");
            }
        }
        static void InitializeClient()
        {
            Console.Write("Input IP address to connect:");
            var ipAddress = IPAddress.Parse(Console.ReadLine());
            ChatSystem.EResult re = chatSystem.InitializeClient(ipAddress, portNo);
            if (re == ChatSystem.EResult.success)
            {
                Console.WriteLine($"Connected host {ipAddress.ToString()}");
            }
            else
            {
                Console.WriteLine($"failed to connect to host,ERROR={chatSystem.resultMessage}");
            }
        }
        static void InChatBot()
        {
            ChatSystem.Buffer buffer = new ChatSystem.Buffer(maxLength);
            bool turn = (connectMode == ChatSystem.ConnectMode.host);
            string received = string.Empty;
            while (true)
            {
                if (turn)
                {   // 受信
                    received = string.Empty;
                    buffer = new ChatSystem.Buffer(maxLength);
                    ChatSystem.EResult re = chatSystem.Receive(buffer);
                    if (re == ChatSystem.EResult.success)
                    {
                        received = Encoding.UTF8.GetString(buffer.content).Replace(EOF, "");
                        int l = received.Length;
                        if (received[0] != '\0')
                        {   // 正常にメッセージを受信
                            Console.WriteLine($"受信メッセージ：{received}");
                        }
                        else
                        {   // 正常に終了を受信
                            Console.WriteLine("相手から終了を受信");
                            break;
                        }
                    }
                    else
                    {   //　受信エラー
                        Console.WriteLine($"受信エラー：{chatSystem.resultMessage} ");
                        break;
                    }
                }
                else
                {   // 送信
                    string inputSt = string.Empty;
                    Console.Write("送るメッセージ：");
                    if(connectMode == ChatSystem.ConnectMode.host)
                    { // Host
                        if (received.Contains("生きたいと言え"))
                        {
                             inputSt = "い゛き゛た゛い゛！！！！";
                             Console.Write(inputSt);
                        }
                        else if (received.Contains("おはよう"))
                        {
                             inputSt = "おはてぃうす";
                             Console.Write(inputSt);
                        }
                        else if (received.Contains("こんにちは"))
                        {
                             inputSt = "こん＾＾";
                             Console.Write(inputSt);
                        }
                        else if (received.Contains("こんばんは"))
                        {
                             inputSt = "( ˘ω˘)ｽﾔｧ";
                             Console.Write(inputSt);
                        }
                        else if (received.Contains("すき"))
                        {
                             inputSt = "いっぱいちゅき";
                             Console.Write(inputSt);
                        }
                        else
                        {
                             inputSt = "生きたいと言え";
                             Console.Write(inputSt);
                        }
                    }
                    else
                    {   // Client
                        inputSt = Console.ReadLine();    // 入力文字で送信
                        if (inputSt.Length > maxLength)
                        {
                            inputSt = inputSt.Substring(0, maxLength - EOF.Length);
                        }
                    }

                    inputSt += EOF;
                    buffer.content = Encoding.UTF8.GetBytes(inputSt);
                    buffer.length = buffer.content.Length;
                    ChatSystem.EResult re = chatSystem.Send(buffer);
                    if (re != ChatSystem.EResult.success)
                    {
                        Console.WriteLine($"送信エラー：{re.ToString()} Error code: {chatSystem.resultMessage}");
                        break;
                    }
                }
                turn = !turn;
            }
            chatSystem.ShutDownColse();
        }

        static void InChat()
        {
            ChatSystem.Buffer buffer = new ChatSystem.Buffer(maxLength);
            bool turn = (connectMode == ChatSystem.ConnectMode.host);
            while (true)
            {
                if (turn)
                {   // 受信
                    buffer = new ChatSystem.Buffer(maxLength);
                    ChatSystem.EResult re = chatSystem.Receive(buffer);
                    if (re == ChatSystem.EResult.success)
                    {
                        string received = Encoding.UTF8.GetString(buffer.content).Replace(EOF, "");
                        int l = received.Length;
                        if (received[0] != '\0')
                        {   // 正常にメッセージを受信
                            Console.WriteLine($"受信メッセージ：{received}");
                        }
                        else
                        {   // 正常に終了を受信
                            Console.WriteLine("相手から終了を受信");
                            break;
                        }
                    }
                    else
                    {   //　受信エラー
                        Console.WriteLine($"受信エラー：{chatSystem.resultMessage} ");
                        break;
                    }
                }
                else
                {   // 送信
                    Console.Write("送るメッセージ：");
                    string inputSt = Console.ReadLine();    // 入力文字で送信
                    if (inputSt.Length > maxLength)
                    {
                        inputSt = inputSt.Substring(0, maxLength - EOF.Length);
                    }
                    inputSt += EOF;
                    buffer.content = Encoding.UTF8.GetBytes(inputSt);
                    buffer.length = buffer.content.Length;
                    ChatSystem.EResult re = chatSystem.Send(buffer);
                    if (re != ChatSystem.EResult.success)
                    {
                        Console.WriteLine($"送信エラー：{re.ToString()} Error code: {chatSystem.resultMessage}");
                        break;
                    }
                }
                turn = !turn;
            }
            chatSystem.ShutDownColse();
        }
        static string Hand()
        {
            bool turn = (connectMode == ChatSystem.ConnectMode.host);
            string select = "";
            while (true)
            {
                Console.WriteLine("What to put out ?[0:グー、1:チョキ、2:パー、それ以外:終了]");
                if (turn)
                {
                    Random te = new Random();
                    select = te.Next(0, 3).ToString();
                    Console.WriteLine(select);
                }
                else
                {
                    select = Console.ReadLine();
                }
                if (select == "0")
                {
                    return "グー";
                }
                else if (select == "1")
                {
                    return "チョキ";
                }
                else if (select == "2")
                {
                    return "パー";
                }
                else
                {
                    return "\0";
                }
            }
        }

        static void Injanken()
        {
            ChatSystem.Buffer buffer = new ChatSystem.Buffer(maxLength);
            bool turn = (connectMode == ChatSystem.ConnectMode.host);
            string received = string.Empty;
            string Hhand;
            string Chand;
            while (true)
            {
                if (turn)
                {   // 受信
                    buffer = new ChatSystem.Buffer(maxLength);
                    ChatSystem.EResult re = chatSystem.Receive(buffer);
                    if (re == ChatSystem.EResult.success)
                    {
                        received = Encoding.UTF8.GetString(buffer.content).Replace(EOF, "");
                        int l = received.Length;
                        if (received[0] != '\0')
                        {   // 正常にメッセージを受信
                            Console.WriteLine($"受信メッセージ：{received}");
                        }
                        else
                        {   // 正常に終了を受信
                            Console.WriteLine("相手から終了を受信");
                            break;
                        }
                    }
                    else
                    {   //　受信エラー
                        Console.WriteLine($"受信エラー：{chatSystem.resultMessage} ");
                        break;
                    }
                }
                else
                {   // 送信
                    string inputSt = string.Empty;
                    //Console.Write("送るメッセージ：");
                    if (connectMode == ChatSystem.ConnectMode.host)
                    {   // Host
                        Hhand = Hand();
                        if (Hhand.Contains("グー"))
                        {
                            if (received.Contains("グー"))
                            {
                                inputSt = "ホストの手：グー\nあいこ";
                                Console.WriteLine("ホストの手：グー\nあいこ");
                            }
                            else if (received.Contains("パー"))
                            {
                                inputSt = "ホストの手：グー\n勝ち";
                                Console.WriteLine("ホストの手：グー\n勝ち");
                            }
                            else
                            {
                                inputSt = "ホストの手：グー\n負け";
                                Console.WriteLine("ホストの手：グー\n負け");
                            }
                        }
                        else if (Hhand.Contains("チョキ"))
                        {
                            if (received.Contains("グー"))
                            {
                                inputSt = "ホストの手：チョキ\n勝ち";
                                Console.WriteLine("ホストの手：チョキ\n勝ち");
                            }
                            else if (received.Contains("パー"))
                            {
                                inputSt = "ホストの手：チョキ\n負け";
                                Console.WriteLine("ホストの手：チョキ\n負け");
                            }
                            else
                            {
                                inputSt = "ホストの手：チョキ\nあいこ";
                                Console.WriteLine("ホストの手：チョキ\nあいこ");
                            }
                        }
                        else
                        {
                            if (received.Contains("グー"))
                            {
                                inputSt = "ホストの手：パー\n負け";
                                Console.WriteLine("ホストの手：パー\n負け");
                            }
                            else if (received.Contains("パー"))
                            {
                                inputSt = "ホストの手：パー\nあいこ";
                                Console.WriteLine("ホストの手：パー\nあいこ");
                            }
                            else
                            {
                                inputSt = "ホストの手：パー\n勝ち";
                                Console.WriteLine("ホストの手：パー\n勝ち");
                            }
                        }
                    }
                    else
                    {   // Client
                        inputSt = Hand(); // 入力文字で送信
                        Chand = inputSt;
                        if (inputSt.Length > maxLength)
                        {
                            inputSt = inputSt.Substring(0, maxLength - EOF.Length);
                        }
                    }

                    inputSt += EOF;
                    buffer.content = Encoding.UTF8.GetBytes(inputSt);
                    buffer.length = buffer.content.Length;
                    ChatSystem.EResult re = chatSystem.Send(buffer);
                    if (re != ChatSystem.EResult.success)
                    {
                        Console.WriteLine($"送信エラー：{re.ToString()} Error code: {chatSystem.resultMessage}");
                        break;
                    }
                }
                turn = !turn;
            }
            chatSystem.ShutDownColse();
        }
    }
}
