using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.Models;

namespace It_Supporter.Interfaces
{
    public interface INotiFication
    {
        Task<Notification> pubNotification();
    }
}