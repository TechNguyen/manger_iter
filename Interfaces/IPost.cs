using It_Supporter.Models;
using It_Supporter.Repository;

namespace It_Supporter.Interfaces
{
    public interface IPost
    {
        //admin + member
        Task<bool> addNewPost(PostsUpload post);
        Task<bool> hidenPost(int PostId);
        Task<bool> showPost(int PostId);
        Task<Posts> GetPosts();
        Task<string> UploadPost(FileUpload filename);
        // admin  
        Task<ICollection<Posts>> sortPost(DateSortPost dateSortPost);
    }
}
