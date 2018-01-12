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
            Program program = new Program();
            program.Test();
            Console.Read();
        }

        private void Test()
        {
            int port = int.Parse(ConfigurationSettings.AppSettings["port"]);
            string ip = ConfigurationSettings.AppSettings["ip"];
            int receiveBufferSize = int.Parse(ConfigurationSettings.AppSettings["receiveBufferSize"]);
            int sendnumber = int.Parse(ConfigurationSettings.AppSettings["sendnumber"]);
            string senddata = ConfigurationSettings.AppSettings["senddata"];
            byte[] data = Encoding.UTF8.GetBytes(senddata);


            ////单个实现测试
            Push client = new Push(receiveBufferSize, ip, port);
            //Pull client = new Pull(receiveBufferSize, ip, port);
            //Pack client = new Pack(receiveBufferSize, ip, port, 0xff);
            for (int i = 0; i < sendnumber; i++)
            {
                client.Send(data, 0, data.Length);
            }

            //Thread.Sleep(1000*10);
            //client.Close();

            //多线程测试

            //ThreadPool.QueueUserWorkItem(new WaitCallback((object o) =>
            //{
            //    Push client1 = new Push(receiveBufferSize, ip, port);
            //        //Pull client1 = new Pull(receiveBufferSize, ip, port);
            //        //Pack client1 = new Pack(receiveBufferSize, ip, port, 0xff);
            //        for (int i = 0; i < sendnumber; i++)
            //    {
            //        client1.Send(data, 0, data.Length);
            //    }
            //}));

            //ThreadPool.QueueUserWorkItem(new WaitCallback((object o) =>
            //{
            //    Push client1 = new Push(receiveBufferSize, ip, 5556);
            //        //Pull client1 = new Pull(receiveBufferSize, ip, port);
            //        //Pack client1 = new Pack(receiveBufferSize, ip, port, 0xff);
            //        for (int i = 0; i < sendnumber; i++)
            //    {
            //        client1.Send(data, 0, data.Length);
            //    }
            //}));
            //ThreadPool.QueueUserWorkItem(new WaitCallback((object o) =>
            //{
            //    Push client1 = new Push(receiveBufferSize, ip, 5557);
            //        //Pull client1 = new Pull(receiveBufferSize, ip, port);
            //        //Pack client1 = new Pack(receiveBufferSize, ip, port, 0xff);
            //        for (int i = 0; i < sendnumber; i++)
            //    {
            //        client1.Send(data, 0, data.Length);
            //    }
            //}));
            //ThreadPool.QueueUserWorkItem(new WaitCallback((object o) =>
            //{
            //    Push client1 = new Push(receiveBufferSize, ip, 5558);
            //        //Pull client1 = new Pull(receiveBufferSize, ip, port);
            //        //Pack client1 = new Pack(receiveBufferSize, ip, port, 0xff);
            //        for (int i = 0; i < sendnumber; i++)
            //    {
            //        client1.Send(data, 0, data.Length);
            //    }
            //}));


            //多对象测试
            //for (int j = 0; j < 200; j++)
            //{
            //    Push client2 = new Push(receiveBufferSize, ip, port);
            //    //Pull client2 = new Pull(receiveBufferSize, ip, port);
            //    //Pack client2 = new Pack(receiveBufferSize, ip, port, 0xff);
            //    for (int i = 0; i < sendnumber; i++)
            //    {
            //        client2.Send(data, 0, data.Length);
            //    }
            //}

        }
    }
}
