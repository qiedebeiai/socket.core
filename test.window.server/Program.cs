using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.window.server.Server;

namespace test.window.server
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = int.Parse(ConfigurationSettings.AppSettings["port"]);
            int numConnections = int.Parse(ConfigurationSettings.AppSettings["numConnections"]);
            int receiveBufferSize = int.Parse(ConfigurationSettings.AppSettings["receiveBufferSize"]);
            int overtime = int.Parse(ConfigurationSettings.AppSettings["overtime"]);

            //Push push = new Push(numConnections, receiveBufferSize, overtime, port);            
            Pull pull = new Pull(numConnections, receiveBufferSize, overtime, port);
            //Pack pack = new Pack(numConnections, receiveBufferSize, overtime, port, 0xff);
            Console.WriteLine("服务端已准备好!");
            Console.Read();
        }
    }
}
