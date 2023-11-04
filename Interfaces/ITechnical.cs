using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace It_Supporter.Interfaces
{
    public interface ITechnical
    {
        //create new tech event
        Task<bool> create(TechEvents techEvents);
        //update status events by admin
        Task<bool> UpdateStatusTechEv(int techId,string statusevents);
        //create a machine for tech
        Task<bool> addMachines(Machines addMachines);
        //Update techinical machines
        Task<bool> updateInfor(string key, int machineId);
        //Delete a machiens
        Task<bool> deleteMachines(List<int> machineIds);
        //restore machine 
        Task<bool> restoreMachines(List<int> machineIds);
        //add Task for technical
        Task<bool> assignMachines(int machinesId, string technicalId);
        //manager
        Task<int?> machineTech(int idTech);
        //total money
        Task<decimal?> manager_money(int techId);
    }
}