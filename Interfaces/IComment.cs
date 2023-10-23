using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.Models;
using Microsoft.EntityFrameworkCore;

namespace It_Supporter.Interfaces
{
    public interface IComment
    {
        Task<Comments> createComment(Comments cmt);
        //delete one comment 
        Task<bool> deleteComment(int CommentId);
        Task<Comments> updateComment(string commentup, int CommentId);
    }
}