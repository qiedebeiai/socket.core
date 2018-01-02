
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using test.linux.server.Server;

namespace test.linux.server
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            int port = int.Parse(Configuration["port"]);
            int numConnections = int.Parse(Configuration["numConnections"]);
            int receiveBufferSize = int.Parse(Configuration["receiveBufferSize"]);
            int overtime = int.Parse(Configuration["overtime"]);

            //Push push = new Push(numConnections, receiveBufferSize, overtime, port);
            //Pull pull = new Pull(numConnections, receiveBufferSize, overtime, port);
            Pack pack = new Pack(numConnections, receiveBufferSize, overtime, port, 0xff);
            Console.WriteLine("服务端已准备好!");
            Console.Read();
        }
    }
}
