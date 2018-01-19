using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    interface IAsyncListener
    {
        void BeginAccept(AsyncCallback cb);
        IAsyncListener EndAccept(IAsyncResult asyncResult);
    }
}
