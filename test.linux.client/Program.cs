using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using test.linux.client.Client;

namespace test.linux.client
{
    class Program
    {
        private  IConfigurationRoot Configuration { get; set; }
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Test();
            Console.Read();
        }


        private void Test()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            int port = int.Parse(Configuration["port"]);
            string ip = Configuration["ip"];
            int receiveBufferSize = int.Parse(Configuration["receiveBufferSize"]);
           
            int sendnumber = int.Parse(Configuration["sendnumber"]);
            string senddata = Configuration["senddata"];
            byte[] data = Encoding.UTF8.GetBytes(senddata);


            Push client = new Push(receiveBufferSize, ip, port);
            //Pull client = new Pull(receiveBufferSize, ip, port);
            //Pack client = new Pack(receiveBufferSize, ip, port, 0xff);
            for (int i = 0; i < sendnumber; i++)
            {
                client.Send(data, 0, data.Length);
            }
        }
    }
}
