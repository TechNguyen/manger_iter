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
        [ProducesResponseType(200, Type = typeof(ProducerResAddPost))]
        public async Task<IActionResult> createForm([FromQuery] int idTech, [FromBody] formTechUser formTechUser) {
            try {
                await _technical.CreateFormUser(idTech,formTechUser);
                  ProducerResAddPost result = new ProducerResAddPost {
                    returncode = 200,
                    returnmessage = "Phieu cua " + formTechUser.username + " da duoc them"
                };
                return Ok(result);
            } catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }
        // get so luong
        [HttpGet("{idTech}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<formTechUser>))]
        public async Task<IActionResult> getformTech(int idTech) {
            try {
                var listForm = await _technical.getTechUser(idTech);
                return Ok(listForm);
            } catch(Exception ex) {
                return NotFound(ex.Message);
            }
        }



    }
}