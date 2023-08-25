using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using Microsoft.AspNetCore.Mvc;

namespace It_Supporter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        [HttpPost("posts")]
        [ProducesResponseType(200, Type =typeof(ProducerResAddPost))]
        public IActionResult addPost([FromBody] string contentpost)
        {
            var result = _post.addPost(_configuration,contentpost);

            if(!ModelState.IsValid)
            {
                return NotFound(ModelState);
            }
            return Ok(result);
        }
        //an di 1 post
        [HttpPost("posthiden")]
        [ProducesResponseType(200, Type = typeof(ProducerResAddPost))]
        [ProducesResponseType(400)]
        public IActionResult hidenPost(int PostId)
        {
            var result = _post.hidenPost(PostId, _configuration);
            if(result == null)
            {
                return NotFound(ModelState);
            }
            return Ok(result);
        }
        //hien 1 post
        [HttpPost("postshow")]
        [ProducesResponseType(200, Type = typeof(ProducerResAddPost))]
        [ProducesResponseType(400)]
        public IActionResult showPost(int PostId)
        {
            var result = _post.showPost(PostId,_configuration); 
            if(result == null)
            {
                return NotFound(ModelState);
            }
            return Ok(result);
        }
        //loc post theo thoi gian
        [HttpPost("sort")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Posts>))]
        [ProducesResponseType(400)]
        public IActionResult sortPostDate( [FromQuery] DateTime fromdate, [FromQuery] DateTime todate) {
            var result = _post.sortPost(fromdate, todate, _configuration);
            if(result == null) {
                NotFound(ModelState);
            }
            return Ok(result);
        }
    }
}
