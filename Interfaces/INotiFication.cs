using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.DataRes;
using It_Supporter.Models;

namespace It_Supporter.Interfaces
{
    public interface INotiFication
    {
        Task<Notification> pubNotification();

        Task<bool> pushNotiWhenPost(Notification noty);

        Task<ProducerResNotification?> getNoti(string id);

        //delete
        Task<bool> deleteNoti(int notiid);
    }
}