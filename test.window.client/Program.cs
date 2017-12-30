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
            List<Push> listPush = new List<Push>();
            int port = int.Parse(ConfigurationSettings.AppSettings["port"]);
            string ip = ConfigurationSettings.AppSettings["ip"];
            int receiveBufferSize = int.Parse(ConfigurationSettings.AppSettings["receiveBufferSize"]);
            int number = int.Parse(ConfigurationSettings.AppSettings["number"]);
            int sendnumber = int.Parse(ConfigurationSettings.AppSettings["sendnumber"]);
            string senddata = ConfigurationSettings.AppSettings["senddata"];
            byte[] data = Encoding.UTF8.GetBytes(senddata);
            for (int i = 0; i < number; i++)
            {
                Push push = new Push(receiveBufferSize, ip, port);
                listPush.Add(push);
                Thread.Sleep(2);
            }

            for (int i = 0; i < sendnumber; i++)
            {
                foreach (var item in listPush)
                {
                    item.Send(data, 0, data.Length);
                    Thread.Sleep(1);
                }
            }

            foreach (var item in listPush)
            {
                item.Close();
            }

            Console.WriteLine("已完成");
            Console.Read();
        }
    }
}
