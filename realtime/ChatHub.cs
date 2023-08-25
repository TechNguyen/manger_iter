using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace It_Supporter.realtime
{
    public sealed class ChatHub :Hub
    {
        public Task SendMessageToAll(string message)
        {
            return Clients.All.SendAsync("ReciveMessage" , message );
        }

    }
}