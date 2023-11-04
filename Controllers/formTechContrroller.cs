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
      
    }
}