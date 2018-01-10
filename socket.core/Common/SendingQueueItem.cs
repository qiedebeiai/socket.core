using System;
using System.Collections.Generic;
using System.Text;

namespace socket.core.Common
{
    /// <summary>
    /// 发送消息体
    /// </summary>
    internal class SendingQueue
    {
        /// <summary>
        /// 连接标记
        /// </summary>
        internal Guid connectId { get; set; }
        /// <summary>
        /// 发送的数据
        /// </summary>
        internal byte[] data { get; set; }
        /// <summary>
        /// 偏移位
        /// </summary>
        internal int offset { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        internal int length { get; set; }
    }
}
