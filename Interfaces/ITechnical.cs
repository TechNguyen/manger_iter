using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.Models;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace It_Supporter.Interfaces
{
    public interface ITechnical
    {
        Task<TechnicalEvents> createTech(TechnicalEvents technicalEvent);
        Task<IEnumerable<TechnicalEvents>> getAllTech();
        Task<bool> UpdateTech(int id , TechnicalEvents technicalEvents);
        Task<bool> CreateFormUser(formTechUsers formTech) ;

        Task<ICollection<formTechUsers>> getTechUser(int id);
        //update trang thai cap nhat
        Task<formTechUsers> updateStatus(string phone, string state);
        bool TechEnventsExit(int id);


    }
}