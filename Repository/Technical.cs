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

namespace It_Supporter.Repository
{
    public class Technical : ITechnical
    {
        private readonly ThanhVienContext _thanhVienContext;
        public Technical(ThanhVienContext thanhVienContext) {
            _thanhVienContext = thanhVienContext;
        }
        // tao tech
        public async Task<TechnicalEvents> createTech(TechnicalEvents techevent) {
            _thanhVienContext.TechnicalEvents.Add(techevent);
            await _thanhVienContext.SaveChangesAsync();
            return techevent;
        }
        //get ra tech
        public async Task<IEnumerable<TechnicalEvents>> getAllTech() {
            var ListTech = _thanhVienContext.TechnicalEvents.ToList();
            return ListTech;
        } 

        public async Task<bool> UpdateTech(int IdTech, TechnicalEvents technical) {
            if(await _thanhVienContext.TechnicalEvents.FindAsync(IdTech) is TechnicalEvents techfound) {
                _thanhVienContext.TechnicalEvents.Entry(techfound).CurrentValues.SetValues(technical);
                await _thanhVienContext.SaveChangesAsync(); 
                return true;
            }
            return false;
        }



        public async Task<bool> CreateFormUser(int id, formTechUser formTechUser) {
            if(await TechEnventsExit(id)) {
                _thanhVienContext.formTechUsers.Add(formTechUser);
                await _thanhVienContext.SaveChangesAsync();
                return true;
            }
            return false; 
        }

        public async Task<IEnumerable<formTechUser>> getTechUser(int idTech) {
            return _thanhVienContext.formTechUsers.Where(p => p.IdTech == idTech).ToList();
        }
        public async Task<bool> TechEnventsExit(int id) {
           
              return  _thanhVienContext.TechnicalEvents.Any(p => p.IdTech == id);            
        }
    }
}