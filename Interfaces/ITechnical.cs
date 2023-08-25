using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.Models;

namespace It_Supporter.Interfaces
{
    public interface ITechnical
    {
        Task<TechnicalEvents> createTech(TechnicalEvents technicalEvent);
        Task<IEnumerable<TechnicalEvents>> getAllTech();
        Task<bool> UpdateTech(int id , TechnicalEvents technicalEvents);
        Task<bool> CreateFormUser(int id, formTechUser formTechUser);


         Task<IEnumerable<formTechUser>> getTechUser(int idTech) ;
        Task<bool> TechEnventsExit(int id);


    }
}