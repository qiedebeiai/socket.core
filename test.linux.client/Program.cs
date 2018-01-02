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
        public static IConfigurationRoot Configuration { get; set; }
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            int port = int.Parse(Configuration["port"]);
            string ip = Configuration["ip"];
            int receiveBufferSize = int.Parse(Configuration["receiveBufferSize"]);
            int number = int.Parse(Configuration["number"]);
            int sendnumber = int.Parse(Configuration["sendnumber"]);
            string senddata = Configuration["senddata"];
            byte[] data = Encoding.UTF8.GetBytes(senddata);

            List<Push> listPush = new List<Push>();
            List<Pull> listPull = new List<Pull>();
            List<Pack> listPack = new List<Pack>();

            for (int i = 0; i < number; i++)
            {
                //Push push = new Push(receiveBufferSize, ip, port);
                //listPush.Add(push);

                //Pull pull = new Pull(receiveBufferSize, ip, port);
                //listPull.Add(pull);

                Pack pack = new Pack(receiveBufferSize, ip, port, 0xff);
                listPack.Add(pack);

                Thread.Sleep(2);
            }

            for (int i = 0; i < sendnumber; i++)
            {
                //foreach (var item in listPush)
                //foreach (var item in listPull)
                foreach (var item in listPack)
                {
                    item.Send(data, 0, data.Length);
                    Thread.Sleep(1);
                }
            }

            Thread.Sleep(1000 * 10);
            Console.WriteLine("发送已经完成！");
            //foreach (var item in listPush)
            //foreach (var item in listPull)
            foreach (var item in listPack)
            {
                item.Close();
            }
            Console.WriteLine("客户端已完成");


            Console.Read();
        }
    }
}
