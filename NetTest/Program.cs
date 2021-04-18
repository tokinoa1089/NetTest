using System;
using System.Net;
using System.Net.Sockets;

namespace NetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            DisplayLocalHostName();
            Console.ReadKey();
        }
        static public void DisplayLocalHostName()
        {
            try {
                // Get the local computer host name.
                String hostName = Dns.GetHostName();
                Console.WriteLine("Computer name :" + hostName);
            }
            catch (SocketException e) {
                Console.WriteLine("SocketException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            catch (Exception e) {
                Console.WriteLine("Exception caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
        }

    }
}
