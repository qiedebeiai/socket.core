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
         byte[] data;
         int receiveBufferSize;
         string ip;
         int port;
         int sendnumber;

        static int aaaa;
        static void Main(string[] args)
        {
            Program program = new Program();

            program.aaa();









            //List<Push> listPush = new List<Push>();
            //List<Pull> listPull = new List<Pull>();
            //List<Pack> listPack = new List<Pack>();

            //for (int i = 0; i < number; i++)
            //{
            //    Push push = new Push(receiveBufferSize, ip, port);
            //    listPush.Add(push);

            //    //Pull pull = new Pull(receiveBufferSize, ip, port);
            //    //listPull.Add(pull);

            //    //Pack pack = new Pack(receiveBufferSize, ip, port, 0xff);
            //    //listPack.Add(pack);

            //    //Thread.Sleep(2);
            //}
            //Thread.Sleep(1000);
            //for (int i = 0; i < sendnumber; i++)
            //{
            //    foreach (var item in listPush)
            //    //foreach (var item in listPull)
            //    //foreach (var item in listPack)
            //    {
            //        item.Send(data, 0, data.Length);
            //        //Thread.Sleep(1);
            //    }
            //}

            ////Thread.Sleep(1000*1);
            //Console.WriteLine("发送已经完成！");
            //foreach (var item in listPush)
            ////foreach (var item in listPull)
            ////foreach (var item in listPack)
            //{
            //    item.Close();
            //}
            //Console.WriteLine("客户端已完成");


            Console.Read();
        }


        private void aaa()
        {
            port = int.Parse(ConfigurationSettings.AppSettings["port"]);
            ip = ConfigurationSettings.AppSettings["ip"];
            receiveBufferSize = int.Parse(ConfigurationSettings.AppSettings["receiveBufferSize"]);
            int number = int.Parse(ConfigurationSettings.AppSettings["number"]);
            sendnumber = int.Parse(ConfigurationSettings.AppSettings["sendnumber"]);
            string senddata = ConfigurationSettings.AppSettings["senddata"];
            data = Encoding.UTF8.GetBytes(senddata);

            ThreadPool.SetMaxThreads(number, number);
            for (int i = 0; i < number; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(test), null);
                //Thread.Sleep(100);
            }
        }

        private  void test(object o)
        {
            Console.WriteLine(aaaa++);
            //Push push = new Push(receiveBufferSize, ip, port);
            Pull pull = new Pull(receiveBufferSize, ip, port);
            //Pack pack = new Pack(receiveBufferSize, ip, port, 0xff);
            for (int i = 0; i < sendnumber; i++)
            {
                //push.Send(data, 0, data.Length);
                pull.Send(data, 0, data.Length);
                //pack.Send(data, 0, data.Length);
                Thread.Sleep(2);
            }
            Console.WriteLine("客户端已完成");
            //Thread.Sleep(1000*10);
            //push.Close();
            //Console.Read();
        }



    }
}
