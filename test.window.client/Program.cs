using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using test.window.client.Client;

namespace test.window.client
{
    class Program
    {
        static void Main(string[] args)
        {

            int port = int.Parse(ConfigurationSettings.AppSettings["port"]);
            string ip = ConfigurationSettings.AppSettings["ip"];
            int receiveBufferSize = int.Parse(ConfigurationSettings.AppSettings["receiveBufferSize"]);
            int number = int.Parse(ConfigurationSettings.AppSettings["number"]);
            int sendnumber = int.Parse(ConfigurationSettings.AppSettings["sendnumber"]);
            string senddata = ConfigurationSettings.AppSettings["senddata"];
            byte[] data = Encoding.UTF8.GetBytes(senddata);

            List<Push> listPush = new List<Push>();
            List<Pull> listPull = new List<Pull>();
            List<Pack> listPack = new List<Pack>();

            for (int i = 0; i < number; i++)
            {
                //Push push = new Push(receiveBufferSize, ip, port);
                //listPush.Add(push);

                Pull pull = new Pull(receiveBufferSize, ip, port);
                listPull.Add(pull);

                //Pack pack = new Pack(receiveBufferSize, ip, port, 0xff);
                //listPack.Add(pack);

                Thread.Sleep(2);
            }

            for (int i = 0; i < sendnumber; i++)
            {
                //foreach (var item in listPush)
                foreach (var item in listPull)
                //foreach (var item in listPack)
                {
                    item.Send(data, 0, data.Length);
                    Thread.Sleep(1);
                }
            }

            Thread.Sleep(1000*1);
            Console.WriteLine("发送已经完成！");
            //foreach (var item in listPush)
            foreach (var item in listPull)
            //foreach (var item in listPack)
            {
                item.Close();
            }
            Console.WriteLine("客户端已完成");


            Console.Read();
        }
    }
}
