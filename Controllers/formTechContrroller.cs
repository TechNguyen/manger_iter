using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using Microsoft.AspNetCore.Mvc;
using It_Supporter.Services;

namespace It_Supporter.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class formTech : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ITechnical _technical;

        private readonly ISendingMesage _sendingMesage;
        private readonly INotiFication _noti;
        private readonly ThanhVienContext _thanhVienContext;
        public formTech(ITechnical technical, ILogger<ThanhVienContext> logger, INotiFication noti, ThanhVienContext thanhVienContext, ISendingMesage sendingMesage ) {
            _technical = technical;
            _thanhVienContext = thanhVienContext;
            _logger = logger;
            _sendingMesage = sendingMesage;
            _noti = noti;

        }
        [HttpPost("create")]
        [ProducesResponseType(200, Type = typeof(formTechUsers))]
        public async Task<IActionResult> createForm([FromQuery] int id, [FromBody] formTechUsers formTechUser) {
            try {
                formTechUser.IdTech = id;
                var userform = await _technical.CreateFormUser(formTechUser);
                ProducerResAddPost result = new ProducerResAddPost {
                    statuscode = 200,
                    message = "Phieu cua " + formTechUser.username + " da duoc them"
                };
                _sendingMesage.SendingMessage<formTechUsers>(formTechUser);
                return Ok(userform);
            } catch (Exception ex) {
                return NotFound(ex.Message);
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