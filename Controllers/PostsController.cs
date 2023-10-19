using It_Supporter.DataContext;
using It_Supporter.DataRes;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using It_Supporter.Repository;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Owin.Security.Provider;
using StackExchange.Redis;

namespace It_Supporter.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class PostsController : ControllerBase
    {
        private ThanhVienContext _context;
        private readonly IPost _post;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public PostsController(ThanhVienContext context, IPost post, ILogger<ThanhVienContext> logger, IConfiguration configuration) {
            _logger = logger;
            _post = post;
            _context = context;
            _configuration = configuration;
        }
        //tao 1 post
        [Authorize(Roles = "Admin, Member")]
        [HttpPost("create")]
        public async Task<IActionResult> addPost([FromForm] Posts post)
        {
            try {
                var result = await _post.addNewPost(post);
                ProducerResAddPost producerRes = new ProducerResAddPost {
                    statuscode = 200,
                    message = "Add new posts succesfully!"
                };
                return Ok(producerRes);
            } catch( Exception ex) {
                return BadRequest(ex.Message);
            }
            
        }
        //an di 1 post
        [HttpPost("post-hiden")]
        [Authorize(Roles = "Admin,Member")]
        public async Task<IActionResult> hiden ([FromQuery] int PostId)
        {
            try {
                var result = await _post.hidenPost(PostId);
                ProducerResAddPost response = new ProducerResAddPost();
                if(result) {
                    response.statuscode = 200;
                    response.message ="Delete post succesfully!";
                } else {
                    response.statuscode = 204;
                    response.message = "Delete post unsuccsesfully!";
                }
                return Ok(response);
            } catch(Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        //hien 1 post
        [Authorize(Roles = "Admin,Member")]
        [HttpPost("show")]
        public async Task<IActionResult> show([FromQuery] int PostId)
        {
            try {
                var result = await _post.showPost(PostId); 
                ProducerResAddPost response = new ProducerResAddPost();
                if(result) {
                    response.statuscode = 200;
                    response.message = "Publish post succesfully!";
                    return Ok(response);
                };
                response.statuscode = 400;
                response.message = "Publish post unsuccessfully!";
                return Ok(response);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        //loc post theo thoi gian
        [Authorize(Roles = "Admin")]
        [HttpPost("sort")]
        public async Task<IActionResult> sortPostDate([FromBody] DateSortPost dateSortPost ) {
            try { 
                var result = await _post.sortPost(dateSortPost);
                ProducerPostManager producer = new ProducerPostManager();
                if(result.Count > 0) {
                    producer.statuscode = 200;
                    producer.message = "Get Post Succesfylly!";
                    producer.counPosts = result.Count;
                    producer.data = result;
                } else if(result == null) {
                    producer.statuscode = 400;
                    producer.message = "Not found information";
                } else {
                    producer.statuscode = 204;
                    producer.counPosts = result.Count; 
                    producer.message = "Not found post in that times!";
                }
                return Ok(producer);
            } catch(Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
