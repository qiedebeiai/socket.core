using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace socket.core.Common
{
    /// <summary>
    /// Token
    /// </summary>
    internal class AsyncUserToken
    {
        /// <summary>
        /// 所使用的套接字
        /// </summary>
        internal Socket Socket { get; set; }
    }
}
