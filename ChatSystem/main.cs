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

        static void Main(string[] args)
        {
            chatSystem = new ChatSystem();
            Console.WriteLine($"this hostName is {chatSystem.hostName}.");
            ChatSystem.ConnectMode connectMode= SelectMode();
            chatSystem.SetConnectMode(connectMode);

        }
        static ChatSystem.ConnectMode  SelectMode()
        {
            ChatSystem.ConnectMode connectMode= ChatSystem.ConnectMode.host;
            Console.Write("Select Mode:\n0=Host,1=Client");
            int select = int.Parse(Console.ReadLine());
            switch (select)
            {
                case 0: //Host
                    connectMode = ChatSystem.ConnectMode.host;
                    Console.WriteLine("Running Host mode");
                    InitializeHost();
                    break;
                case 1: //Client
                    connectMode = ChatSystem.ConnectMode.client;
                    Console.WriteLine("Running Client mode");
                    InitializeClient();
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
            chatSystem.InitializeHost(ipAddress, portNo);
        }
        static void InitializeClient()
        {
            Console.Write("Input IP address to connect:");
            var ipAddress = IPAddress.Parse(Console.ReadLine());
            bool suceed =chatSystem.InitializeClient(ipAddress, 11000);
            if (suceed)
            {
                Console.WriteLine($"Connected host {ipAddress.ToString()}");
            }
            else
            {
                Console.WriteLine("faled to connect to host");
            }
        }
    }
}
