using System.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using It_Supporter.DataContext;
using It_Supporter.DataRes;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace It_Supporter.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/v1/[controller]")]
    public class TechnicalController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ITechnical _technical ;

        private ThanhVienContext _thanhVienContext;
        public TechnicalController(ILogger<ThanhVienContext> logger,ITechnical technical, ThanhVienContext thanhVienContext) {
            _thanhVienContext = thanhVienContext;
            _logger = logger;
            _technical = technical;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> createTechEvent([FromBody] TechEvents techEvents) {
            try {

                TechEvents tev = new TechEvents {
                    address = techEvents.address,
                    timeend = DateTime.Parse(techEvents.timeend.ToString()),
                    timestart = DateTime.Parse(techEvents.timestart.ToString()),
                    techday = techEvents.techday,
                    status = techEvents.status
                };
                var rs = await _technical.create(tev);
                ProducerResponse producer = new ProducerResponse();
                if(rs) {
                    producer.statuscode = 200;
                    producer.message = "Create a new tech events successfully!";
                }
                else {
                    producer.statuscode = 404;
                    producer.message = "Create a new tech events unsuccessfully!";
                }
                return Ok(producer); 

            } catch(Exception ex) {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("update-status")]
        public async Task<IActionResult> updateStatusTech([FromQuery] int techId, [FromQuery] string statusevents) {
            try {
                bool rs = await _technical.UpdateStatusTechEv(techId,statusevents);
                if(rs) {
                    ProducerResponse producer = new ProducerResponse {
                        statuscode = 200,
                        message = "Update status TechEvents successfully!"
                    }; 
                    return Ok(producer);
                } else return BadRequest("Update status TechEnvents unsuccessfully!");
            } catch(Exception ex) {
                return BadRequest(ex.Message); 
            }
        }


        [Authorize(Roles = "Admin,Member")]
        [HttpPost("registerTech")]
        public async Task<IActionResult> RegisterTech(int techId) {
            try {
                // var rs = _technical.registerTech(techId);
                return Ok();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        [Authorize( Roles ="Admin,Member")]
        [HttpPost("machines/create")]
        public async Task<IActionResult> addMachinesTech([FromBody] Machines machines) {
            try {
                var rs = await _technical.addMachines(machines);
                if(rs) {
                    ProducerResponse producer = new ProducerResponse {
                        statuscode = 200,
                        message = "Create a machines successfully!"
                    }; 
                     return Ok(producer);
                } else {
                    ProducerResponse producer = new ProducerResponse {
                        statuscode = 400,
                        message = "Create a new machines unsuccessfully!"
                    };
                    return Ok(producer);
                }
            } catch( Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("machines/delete")]
        public async Task<IActionResult> deleteMachines([FromBody] List<int> machineIds) {
            try {
                var rs = await _technical.deleteMachines(machineIds);
                ProducerResponse producer = new ProducerResponse();
                if(rs) {
                    producer.statuscode = 200;
                    producer.message = "Delete machine successfully!";
                } else {
                    producer.statuscode = 400;
                    producer.message = "Delete machine unsuccessfly!";
                }
                return Ok(producer);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("machines/restore")]
        public async Task<IActionResult> restoreMachines([FromBody] List<int> machineIds) {
            try {
                var rs = await _technical.restoreMachines(machineIds);
                ProducerResponse producer = new ProducerResponse();
                if(rs) {
                    producer.statuscode = 200;
                    producer.message = "Restore machine successfully!";
                } else {
                    producer.statuscode = 400;
                    producer.message = "Restore machine unsuccessfly!";
                }
                return Ok(producer);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("machines/assign")]
        public async Task<IActionResult> assignMachines([FromQuery] int machineId, [FromBody] string TechnicalId) {
            try {
                var rs = await _technical.assignMachines(machineId,TechnicalId);

                ProducerResponse producer = new ProducerResponse();
                if(rs) {
                    producer.statuscode = 200;
                    producer.message = "Assign machine successfully!";
                } else {
                    producer.statuscode = 400;
                    producer.message = "Assign machine unsuccessfully!";
                }
                return Ok(producer);
            } catch (Exception ex){
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("managerMoney")]
        public async Task<IActionResult> getManager([FromQuery] int idTech) {
            try {
                var rs = await _technical.manager_money(idTech);
                var countMachines = await _technical.machineTech(idTech);
                ProcedureManagerMoney producer = new ProcedureManagerMoney();
                Console.WriteLine(rs.Value);
                if(rs != null) {
                    producer.statuscode = 200;
                    producer.message = "Get manager money tech successfully!";
                    producer.totalMoney = rs.Value;
                    producer.countMachine = countMachines.Value;
                } else {
                    producer.statuscode = 400;
                    producer.message = "Get manager money unsuccessfully!";
                    producer.countMachine = countMachines.Value;
                }
                return Ok(producer);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}