using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using It_Supporter.Controllers;
using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using NuGet.DependencyResolver;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;


namespace It_Supporter.Repository
{
    public class Technical : ITechnical
    {
        private readonly ThanhVienContext _context;
        private readonly HttpContext _http;
        public Technical(ThanhVienContext context, IHttpContextAccessor http) {
            _context = context;
            _http = http.HttpContext;
        }

        public async Task<bool> addMachines(Machines addMachines)
        {
            try {
                string MaTV = _http.Session.GetString("MaTV");
                Console.WriteLine(MaTV);
                if(!MaTV.IsNullOrEmpty()) {
                    Machines newmachines = new Machines {
                        customername = addMachines.customername,
                        phonenumber = addMachines.phonenumber,
                        machine_status = addMachines.machine_status,
                        techId = addMachines.techId,
                        services = addMachines.services,
                        machinesgetat = addMachines.machinesgetat.ToLocalTime(),
                        TesterId = addMachines.TesterId,
                        Technical = addMachines.Technical,
                    };
                    //Machine_ThanhvVien
                    _context.Machines.Add(newmachines);
                    int rowAffected = _context.SaveChanges();
                    return rowAffected > 0 ? true : false;
                }
                return false;
            } catch {
                return false;
            }
        }

        public async Task<bool> assignMachines(int machinesId, string technicalId)
        {
            try {
                var machines = _context.Machines.FirstOrDefault(p => p.id == machinesId);
                machines.Technical = technicalId;
                return _context.SaveChanges() > 0 ? true : false;
            } catch {
                return false;
            }
        }
        // tao tech
        public async Task<bool> create(TechEvents techEvents)
        {
            try {
                TechEvents tech = new TechEvents {
                    address = techEvents.address,
                    timestart = techEvents.timestart.ToLocalTime(),
                    timeend = techEvents.timeend.ToLocalTime(),
                    techday = techEvents.techday,
                    status = techEvents.status
                };
                _context.TechEvents.Add(tech);
                int rowAffected = _context.SaveChanges();
                if(rowAffected == 0) {
                    return false;
                } else return true;
            } catch {
                return false;
            }
        }

        public async Task<bool> deleteMachines(List<int> machineIds)
        {
            try {
                foreach(int i in machineIds) {
                    var machines = _context.Machines.FirstOrDefault(p => p.id == i);
                    machines.deleted = 1;
                }
                return _context.SaveChanges() > 0 ? true : false;
            } catch {
                return false;
            }
        }

        public async Task<int?> machineTech(int idTech)
        {
            try {
                int countMachines = _context.Machines.Where(p => p.techId == idTech).ToList().Count;
                return countMachines;
            } catch {
                return null;
            }
        }


        public async Task<decimal?> manager_money(int techId)
        {
            try {
                List<Machines> machines = _context.Machines.Where(p => p.serviceCharger >= 0).ToList();
                decimal total = 0;
                foreach(var machine in machines) {
                    total += machine.serviceCharger;
                }
                return total;
            } catch {
                return null;
            }
        }
        public async Task<bool> restoreMachines(List<int> machinIds)
        {
            try {
                foreach(int i in machinIds) {
                    var item = _context.Machines.FirstOrDefault(p => p.id == i);
                    item.deleted = 0;
                }
                return _context.SaveChanges() > 0 ? true : false;
            } catch {
                return false;
            }
        }
        public async Task<bool> updateInfor(string key, int machineId)
        {
            try {

                return true;
            } catch {
                return false;
            }
        }

        public async Task<bool> UpdateStatusTechEv(int techId, string statusevents)
        {
            try {
                var techEvents =  _context.TechEvents.SingleOrDefault(p => p.id == techId);
                techEvents.status = statusevents;
                _context.SaveChanges();
                return true;
            } catch {
                return false;
            }
        }
    }
}