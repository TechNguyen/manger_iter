using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.DataContext;
using It_Supporter.DataRes;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace It_Supporter.Controllers
{
    [ApiController ]
    [Route("api/v1/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IComment _comment; 
        private readonly ThanhVienContext _thanhVienContext;
        public CommentController(ILogger<CommentController> logger, IComment comment, ThanhVienContext thanhVienContext) { 
            _logger = logger;
            _comment = comment;
            _thanhVienContext = thanhVienContext;
        }

        // [Authorize(Roles = "Admin, Member")]
        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Member")]
        [HttpPost("create")]
        public async Task<IActionResult> addComment([FromForm] Comments comment) {
            try {
                var rs = await _comment.createComment(comment);  
                ProducerResComment resComment = new ProducerResComment();
                if(rs != null) {
                    resComment.statuscode = 200;
                    resComment.message = "Create new comment successfully!";
                    return Ok(resComment);
                } else {
                    resComment.statuscode = 400;
                    resComment.message = "Create new comment unsuccessfully!";
                }
                return Ok(resComment);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        // [Authorize(Roles = "Admin, Member")]
        [HttpPut("update")]
        public async Task<IActionResult> update([FromQuery] int CommentId, [FromBody] string content) {
            try {
                var rs = await _comment.updateComment(content,CommentId);
                ProducerResponse producer = new ProducerResponse();
                if(rs != null) {
                    producer.statuscode = 200;
                    producer.message = "Update comment successfully!";
                } else {
                    producer.statuscode = 404;
                    producer.message = "Update comment unsuccessfully!";
                }
                return Ok(rs);
            } catch(Exception ex) {
                return BadRequest(ex.Message);
            }
        } 
        // [Authorize(Roles = "Admin, Member")]
        [HttpDelete("delete")]
        public async Task<IActionResult> delete([FromQuery]  int CommentId) {
            try { 
                var rs =  await _comment.deleteComment(CommentId);
                ProducerResponse producer = new ProducerResponse();
                if(rs) {
                    producer.statuscode = 200;
                    producer.message = "Delete comment successfully!";
                } else {
                    producer.statuscode = 404;
                    producer.message = "Delete comment unsuccesfully";
                }
                return Ok(producer);
            } catch(Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    } 
}