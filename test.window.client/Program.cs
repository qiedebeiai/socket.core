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
