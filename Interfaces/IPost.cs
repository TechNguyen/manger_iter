using It_Supporter.Models;
using It_Supporter.Repository;

namespace It_Supporter.Interfaces
{
    public interface IPost
    {
        ProducerResAddPost addPost(IConfiguration builder, string context);
        ProducerResAddPost hidenPost(int PostId, IConfiguration builder);

        ProducerResAddPost showPost(int PostId, IConfiguration builder);
        
        ICollection<Posts> sortPost(DateTime fromdate, DateTime todate, IConfiguration builder);
    }
}
