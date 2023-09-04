using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Services
{
    public interface ISendingMesage
    {
        void SendingMessage<T>(T message);
    }
}