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
        Task<bool> createComment(Comments cmt);
        //delete one comment 
        Task<bool> deleteComment(int CommentId);
        Task<bool> updateComment(string content, int CommentId);
    }
}