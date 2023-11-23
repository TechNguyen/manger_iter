using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.BackGroundJob;
using It_Supporter.DataContext;
using It_Supporter.DataRes;
using It_Supporter.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace It_Supporter.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NotificationController : ControllerBase
    {
         private ThanhVienContext _context;
        private readonly INotiFication _notifi;
        private readonly ILogger _logger;


        private readonly IbirthDay _birthday;

        public NotificationController(ILogger<NotificationController> logger,
            INotiFication notiFication, 
            ThanhVienContext context,
            IbirthDay birthday) {
            _context = context;
            _notifi = notiFication;
            _logger = logger;
            _birthday = birthday;
        }
        // get all Notification
        [HttpPost("noties")]
        public async Task<IActionResult> getNotification([FromBody] string id) {
            try {
                var newnoti = await _notifi.getNoti(id);
                return Ok(newnoti);
            } catch(Exception ex) {
                return BadRequest(ex.Message);
            } 
        }
        [HttpPost("read")]
        public async Task<IActionResult> readNotification([FromBody] int id)
        {
            try
            {
                ProducerResponse producer = new ProducerResponse();
                bool rs = await _notifi.deleteNoti(id);
                if(rs)
                {
                    producer.statuscode = 200;
                    producer.message = "You read notifications successfullyy!";
                }
                return Ok(producer);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}