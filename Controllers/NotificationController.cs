using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace It_Supporter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
         private ThanhVienContext _context;
        private readonly INotiFication _notifi;
        private readonly ILogger _logger;

        public NotificationController(ILogger<NotificationController> logger, INotiFication notiFication, ThanhVienContext context ) {
            _context = context;
            _notifi = notiFication;
            _logger = logger;
        }
        // create Notification
        [HttpGet]
        public async Task<IActionResult> getNotification() {
            try {
                var newnoti = await _notifi.pubNotification();
                return Ok(newnoti);
            } catch(Exception ex) {
                return BadRequest(ex.Message);
            } 
        }
    }
}