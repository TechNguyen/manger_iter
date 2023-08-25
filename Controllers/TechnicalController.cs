using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace It_Supporter.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
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
        [HttpPost("createTech")]
        [ProducesResponseType(200, Type = typeof(TechnicalEvents))]
        public async Task<IActionResult> CreateNewTech( [FromQuery] string Address,[FromQuery] DateTime createAt, [FromQuery] int startime, [FromQuery] int endTime){
            try {
                TechnicalEvents technicalEvent =  new TechnicalEvents {
                    Address = Address,
                    createAt = createAt,
                    startTime = startime,
                    endTime = endTime
                };
                await _technical.createTech(technicalEvent);
                return Ok(technicalEvent);
            } catch(Exception ex) {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("")]
        public async Task<IActionResult> GetTech() {
            try {
                var ListTech = await _technical.getAllTech();
                return Ok(ListTech);
            } catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> updateTechInfor(int id, [FromBody] TechnicalEvents technical) {
            try {
                technical.IdTech = id;
                var updateTech = await _technical.UpdateTech(id,technical);
                return Ok(updateTech);
            } catch(Exception ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("techCount")]
        public async Task<IActionResult> statistical([FromQuery] int IdTech) {
            try {
                return Ok();
            } catch(Exception ex) {
                return NotFound(ex.Message);
            }
        } 
    }
}