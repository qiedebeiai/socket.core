using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace socket.core.Client
{
    public class TcpPackClient
    {
        /// <summary>
        /// 基础类
        /// </summary>
        private TcpClients tcpClients;
        /// <summary>
        /// 连接成功事件
        /// </summary>
        public event Action<bool> OnAccept;
        /// <summary>
        /// 接收通知事件
        /// </summary>
        public event Action<byte[]> OnReceive;
        /// <summary>
        /// 断开连接通知事件
        /// </summary>
        public event Action OnClose;
        /// <summary>
        /// 接收到的数据缓存
        /// </summary>
        private List<byte> queue;
        /// <summary>
        /// 包头标记
        /// </summary>
        private int headerFlag;
        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="headerFlag">包头标记范围0~1023(0x3FF),当包头标识等于0时，不校验包头</param>
        public TcpPackClient( int receiveBufferSize,int headerFlag)
        {
            if(headerFlag<0|| headerFlag>1023)
            {
                headerFlag = 0;
            }
            this.headerFlag = headerFlag;
            Thread thread = new Thread(new ThreadStart(() =>
            {
                queue = new List<byte>();
                tcpClients = new TcpClients( receiveBufferSize);
                tcpClients.OnAccept += TcpServer_eventactionAccept;
                tcpClients.OnReceive += TcpServer_eventactionReceive;
                tcpClients.OnClose += TcpServer_eventClose;
            }));
            thread.Start();
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ip">ip地址或域名</param>
        /// <param name="port">端口</param>
        public void Connect(string ip,int port)
        {
            while (tcpClients == null)
            {
                Thread.Sleep(2);
            }
            tcpClients.Connect(ip,port);
        }

        /// <summary>
        /// 连接成功事件方法
        /// </summary>
        /// <param name="success">是否成功连接</param>
        private void TcpServer_eventactionAccept(bool success)
        {
            if (OnAccept != null)
                OnAccept(success);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        /// <param name="length">长度</param>
        public void Send(byte[] data, int offset, int length)
        {
            data = AddHead(data.Skip(offset).Take(length).ToArray());
            tcpClients.Send(data, 0, data.Length);
        }

        /// <summary>
        /// 接收通知事件方法
        /// </summary>
        /// <param name="data"></param>
        private void TcpServer_eventactionReceive( byte[] data)
        {
            if (OnReceive != null)
            {               
                queue.AddRange(data);
                byte[] datas=Read();
                if(datas != null&& datas.Length>0)
                {
                    OnReceive( datas);
                }
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Close()
        {
            tcpClients.Close();
        }

        /// <summary>
        /// 断开连接通知事件方法
        /// </summary>
        private void TcpServer_eventClose()
        {
            queue.Clear();
            if (OnClose != null)
                OnClose();
        }

        /// <summary>
        /// 在数据起始位置增加4字节包头
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] AddHead(byte[] data)
        {
            int len = data.Length;
            int header = (headerFlag << 22) | len;
            byte[] head=System.BitConverter.GetBytes(header);
            return head.Concat(data).ToArray();           
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns></returns>
        private byte[] Read()
        {
            int header=BitConverter.ToInt32(queue.ToArray(), 0);        
            if(headerFlag!= (header >> 22))
            {
                return null;
            }
            int len = header & 0x3fffff;
            if (len > queue.Count-4)
            {
                return null;
            }
            byte[] f = queue.Skip(4).Take(len).ToArray();
            queue.RemoveRange(0,len+4);
            return f;
        }


    }
}
