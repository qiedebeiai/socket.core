using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System.Collections.Concurrent;
using socket.core.Common;

namespace socket.core.Server
{
    /// <summary>
    /// tcp Socket监听基库
    /// </summary>
    internal class TcpServer
    {
        /// <summary>
        /// 同时处理的最大连接数
        /// </summary>
        private int m_numConnections;
        /// <summary>
        /// 用于每个套接字I/O操作的缓冲区大小
        /// </summary>
        private int m_receiveBufferSize;
        /// <summary>
        /// 所有套接字接收操作的一个可重用的大型缓冲区集合。
        /// </summary>
        private BufferManager m_bufferManager;
        /// <summary>
        /// 用于监听传入连接请求的套接字
        /// </summary>
        private Socket listenSocket;
        /// <summary>
        /// 接受端SocketAsyncEventArgs对象重用池，接受套接字操作
        /// </summary>
        private SocketAsyncEventArgsPool m_receivePool;
        /// <summary>
        /// 发送端SocketAsyncEventArgs对象重用池，发送套接字操作
        /// </summary>
        private SocketAsyncEventArgsPool m_sendPool;
        /// <summary>
        /// 连接到服务器的客户端总数
        /// </summary>
        private int m_numConnectedSockets;
        /// <summary>
        /// 超时，如果超时，服务端断开连接，客户端需要重连操作
        /// </summary>
        private int overtime;
        /// <summary>
        /// 超时检查间隔时间(秒)
        /// </summary>
        public int overtimecheck = 10;
        /// <summary>
        /// 能接到最多客户端个数的原子操作
        /// </summary>
        public Semaphore m_maxNumberAcceptedClients;
        /// <summary>
        /// 已经连接的对象池
        /// </summary>
        public ConcurrentBag<ConnectClient> connectClient;
        /// <summary>
        /// 连接成功事件
        /// </summary>
        public event Action<Guid> OnAccept;
        /// <summary>
        /// 接收通知事件
        /// </summary>
        public event Action<Guid, byte[]> OnReceive;
        /// <summary>
        /// 断开连接通知事件
        /// </summary>
        public event Action<Guid> OnClose;
        public Semaphore semaphore;
        /// <summary>
        /// 设置基本配置
        /// </summary>   
        /// <param name="numConnections">同时处理的最大连接数</param>
        /// <param name="receiveBufferSize">用于每个套接字I/O操作的缓冲区大小(接收端)</param>
        /// <param name="overtime">超时时长,单位秒.(每10秒检查一次)，当值为0时，不设置超时</param>
        internal TcpServer(int numConnections, int receiveBufferSize, int overTime)
        {
            overtime = overTime;
            m_numConnectedSockets = 0;
            m_numConnections = numConnections;
            m_receiveBufferSize = receiveBufferSize;
            m_bufferManager = new BufferManager(receiveBufferSize * numConnections, receiveBufferSize);
            m_receivePool = new SocketAsyncEventArgsPool(numConnections);
            m_sendPool = new SocketAsyncEventArgsPool(numConnections);
            m_maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);
            semaphore = new Semaphore(numConnections, numConnections);
            Init();
        }

        /// <summary>
        /// 初始化服务器通过预先分配的可重复使用的缓冲区和上下文对象。这些对象不需要预先分配或重用，但这样做是为了说明API如何可以易于用于创建可重用对象以提高服务器性能。
        /// </summary>
        private void Init()
        {
            connectClient = new ConcurrentBag<ConnectClient>();
            //分配一个大字节缓冲区，所有I/O操作都使用一个。这个侍卫对内存碎片
            m_bufferManager.InitBuffer();
            //预分配的接受对象池socketasynceventargs，并分配缓存
            SocketAsyncEventArgs saea_receive;
            //分配的发送对象池socketasynceventargs，不分配缓存
            SocketAsyncEventArgs saea_send;
            for (int i = 0; i < m_numConnections; i++)
            {
                //预先接受端分配一组可重用的消息
                saea_receive = new SocketAsyncEventArgs();
                saea_receive.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                saea_receive.UserToken = new AsyncUserToken();
                //分配缓冲池中的字节缓冲区的socketasynceventarg对象
                m_bufferManager.SetBuffer(saea_receive);
                m_receivePool.Push(saea_receive);
                //预先发送端分配一组可重用的消息
                saea_send = new SocketAsyncEventArgs();
                saea_send.UserToken = new AsyncUserToken();
                m_sendPool.Push(saea_send);
            }
        }

        /// <summary>
        /// 启动tcp服务侦听
        /// </summary>       
        /// <param name="port">端口</param>
        public void Start(int port)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            //创建listens是传入的连接插座。
            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //绑定端口
            listenSocket.Bind(localEndPoint);
            //启动服务器100连接，监听
            listenSocket.Listen(100);
            //在监听套接字上接受
            StartAccept(null);
            //超时机制
            if (overtime > 0)
            {
                Thread heartbeat = new Thread(new ThreadStart(() =>
                {
                    Heartbeat();
                }));
                heartbeat.Priority = ThreadPriority.Lowest;
                heartbeat.Start();
            }
        }

        /// <summary>
        /// 超时机制
        /// </summary>
        private void Heartbeat()
        {
            //计算超时次数 ，超过count就当客户端断开连接。服务端清除该连接资源
            int count = overtime / overtimecheck;
            while (true)
            {
                ConnectClient client = connectClient.FirstOrDefault(P => P.keep_alive >= count);
                if (client != null)
                {
                    client.keep_alive = 0;
                    CloseClientSocket(client.saea_receive);
                }
                foreach (var item in connectClient)
                {
                    item.keep_alive++;
                }
                Thread.Sleep(overtimecheck * 1000);
            }
        }


        #region Accept

        /// <summary>
        /// 开始接受客户端的连接请求的操作。
        /// </summary>
        /// <param name="acceptEventArg">发布时要使用的上下文对象服务器侦听套接字上的接受操作</param>
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                // 套接字必须被清除，因为上下文对象正在被重用。
                acceptEventArg.AcceptSocket = null;
            }
            m_maxNumberAcceptedClients.WaitOne();
            bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArg);
            }
        }

        /// <summary>
        /// 当异步连接完成时调用此方法
        /// </summary>
        /// <param name="e"></param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            Interlocked.Increment(ref m_numConnectedSockets);
            Console.WriteLine("客户端连接已接受有{0}客户端连接到服务器", m_numConnectedSockets);
            //从接受端重用池获取一个新的SocketAsyncEventArgs对象
            SocketAsyncEventArgs receiveEventArgs = m_receivePool.Pop();
            ((AsyncUserToken)receiveEventArgs.UserToken).Socket = e.AcceptSocket;
            //一旦客户机连接，就向连接发送一个接收。
            bool willRaiseEventReceive = e.AcceptSocket.ReceiveAsync(receiveEventArgs);
            if (!willRaiseEventReceive)
            {
                ProcessReceive(receiveEventArgs);
            }
            //从发送端重用池获取一个新的SocketAsyncEventArgs对象
            SocketAsyncEventArgs sendEventArgs = m_sendPool.Pop();
            ((AsyncUserToken)sendEventArgs.UserToken).Socket = e.AcceptSocket;
            //把连接到的客户端信息添加到集合中
            ConnectClient connecttoken = new ConnectClient();
            connecttoken.connectId = Guid.NewGuid();
            connecttoken.socket = e.AcceptSocket;
            connecttoken.saea_receive = receiveEventArgs;
            connecttoken.saea_send = sendEventArgs;
            connectClient.Add(connecttoken);
            //回调
            if (OnAccept != null)
            {
                OnAccept(connecttoken.connectId);
            }
            // 接受第二连接的要求
            StartAccept(e);
        }

        /// <summary>
        /// 这种方法与socket.acceptasync回调方法操作，并在接受操作完成时调用。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        /// <summary>
        /// 客户端断开一个连接
        /// </summary>
        /// <param name="e"></param>
        protected void CloseClientSocket(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = e.UserToken as AsyncUserToken;
            if (!token.Socket.Connected)
            {
                return;
            }
            // 关闭与客户端关联的套接字
            try
            {
                token.Socket.Shutdown(SocketShutdown.Both);
            }
            // 抛出客户端进程已经关闭
            catch (Exception) { }
            token.Socket.Close();
            // 减少计数器跟踪连接到服务器的客户端总数
            Interlocked.Decrement(ref m_numConnectedSockets);
            m_maxNumberAcceptedClients.Release();
            Console.WriteLine("客户端已与服务器断开连接。有连接到服务器的{0}客户机", m_numConnectedSockets);
            //释放SocketAsyncEventArgs，以便其他客户端可以重用它们
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                m_receivePool.Push(e);
                ConnectClient conn = connectClient.FirstOrDefault(P => P.saea_receive == e);
                if (conn != null)
                {
                    m_sendPool.Push(conn.saea_send);
                    connectClient.TryTake(out conn);
                    if (OnClose != null)
                    {
                        OnClose(conn.connectId);
                    }
                }
            }
        }

        /// <summary>
        /// 客户端断开一个连接
        /// </summary>
        /// <param name="connectId"></param>
        public void Close(Guid connectId)
        {
            ConnectClient conn = connectClient.FirstOrDefault(P => P.connectId == connectId);
            if (conn == null)
            {
                return;
            }
            try
            {
                CloseClientSocket(conn.saea_receive);
            }
            catch (Exception) { }
        }

        #endregion

        /// <summary>
        /// 每当套接字上完成接收或发送操作时，都会调用此方法。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">与完成的接收操作关联的SocketAsyncEventArg</param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            //确定刚刚完成哪种类型的操作并调用相关的处理程序
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("套接字上完成的最后一个操作不是接收或发送。");
            }
            int a = 0;
        }

        #region 接受处理 receive

        /// <summary>
        /// 接受处理回调
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            //检查远程主机是否关闭连接
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                byte[] data = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
                //回调               
                if (OnReceive != null)
                {
                    ConnectClient connect = connectClient.FirstOrDefault(P => P.saea_receive == e);
                    if (connect != null)
                    {
                        OnReceive(connect.connectId, data);
                    }
                }
                //如果接收到数据，超时记录设置为0
                if (overtime > 0)
                {
                    ConnectClient client = connectClient.FirstOrDefault(P => P.saea_receive == e);
                    if (client != null)
                    {
                        client.keep_alive = 0;
                    }
                }
                //将收到的数据回显给客户端             
                bool willRaiseEvent = token.Socket.ReceiveAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessReceive(e);
                }
                int a = 0;
            }
            else
            {
                CloseClientSocket(e);
            }
            int b = 1;
        }

        #endregion


        #region 发送处理 send

        /// <summary>
        /// 异步发送消息 
        /// </summary>
        /// <param name="connectId">连接ID</param>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移位</param>
        /// <param name="length">长度</param>
        public void Send(Guid connectId, byte[] data, int offset, int length)
        {
            //semaphore.WaitOne();
            ConnectClient connect = connectClient.FirstOrDefault(P => P.connectId == connectId);
            if (connect != null)
            {
                AsyncUserToken token = (AsyncUserToken)connect.saea_send.UserToken;
                // 读取从客户端发送的下一个数据块
                connect.saea_send.SetBuffer(data, offset, length);
                bool willRaiseEvent = token.Socket.SendAsync(connect.saea_send);
                if (!willRaiseEvent)
                {
                    ProcessSend(connect.saea_send);
                }
            }
            //semaphore.Release();
        }

        /// <summary>
        /// 异常发送回调
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                //完成将数据回传给客户端
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                // 读取从客户端发送的下一个数据块
                bool willRaiseEvent = token.Socket.SendAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessSend(e);
                }
            }
            else
            {
                CloseClientSocket(e);
            }
        }


        #endregion




    }

}