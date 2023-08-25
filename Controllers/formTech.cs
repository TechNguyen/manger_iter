using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using Microsoft.AspNetCore.Mvc;

namespace It_Supporter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class formTech : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ITechnical _technical;

        private readonly ThanhVienContext _thanhVienContext;
        public formTech(ITechnical technical, ILogger<ThanhVienContext> logger, ThanhVienContext thanhVienContext ) {
            _technical = technical;
            _thanhVienContext = thanhVienContext;
            _logger = logger;

        }
        [HttpPost("create")]
        [ProducesResponseType(200, Type = typeof(formTechUsers))]
        public async Task<IActionResult> createForm([FromQuery] int id, [FromBody] formTechUsers formTechUser) {
            try {
                formTechUser.IdTech = id;
                var userform = await _technical.CreateFormUser(formTechUser);
                ProducerResAddPost result = new ProducerResAddPost {
                    returncode = 200,
                    returnmessage = "Phieu cua " + formTechUser.username + " da duoc them"
                };
                return Ok(userform);
            } catch (Exception ex) {
                return NotFound(ex);
            }
        }
        // get so luong
        [HttpGet("{idTech}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<formTechUsers>))]
        public async Task<IActionResult> getformTech(int idTech) {
            try {
                var listForm = await _technical.getTechUser(idTech);
                return Ok(listForm);
            } catch(Exception ex) {
                return NotFound(ex.Message);
            }
        }


        // cap nhat trang thai
        [HttpPut("{phone}")]
        [ProducesResponseType(200, Type =  typeof(formTechUsers))]
        public async Task<IActionResult> Updatestate(string phone, [FromQuery] string state) {
            try {
                var user = await _technical.updateStatus(phone, state);
                return Ok(user);
            } catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }
    }
}