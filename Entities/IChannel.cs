using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    interface IChannel
    {
        void PostMessage(byte[] data);

        void BeginPostMessage(byte[] data, AsyncCallback cb);
    }
}
