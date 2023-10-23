using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;

namespace It_Supporter.Repository
{
    public class CommentRepo : IComment
    {
        private readonly ThanhVienContext _context;
        
        public CommentRepo(ThanhVienContext context) {
            _context = context;
        }
        public async Task<Comments> createComment(Comments cmt)
        {
            try {
                if(cmt.deleted == null) {
                    cmt.deleted = 0;
                }
                if(cmt.createat == null) {
                    cmt.createat = DateTime.Now;
                }
                Comments conmment = new Comments {
                    authorId = cmt.authorId,
                    createat = cmt.createat,
                    deleteat = cmt.deleteat,
                    parentId = cmt.parentId,
                    content = cmt.content,
                    postId = cmt.postId,
                    updateat = cmt.updateat
                };
                _context.Comments.AddAsync(cmt);
                _context.SaveChanges();
                return cmt;
            } catch (Exception ex) {
                return null;
            }
        }
        public async Task<bool> deleteComment(int CommentId)
        {
            try { 
                var rs = _context.Comments.FirstOrDefault(p => p.id == CommentId);
                rs.deleted = (int) 1;
                rs.deleteat = DateTime.Now;
                _context.SaveChangesAsync();
                return true;
            } catch (Exception ex) {
                return false;
            }
        }

        public async Task<Comments> updateComment(string commentup,int CommentId)
        {
            try {
                var rs = _context.Comments.Where(p => p.id == CommentId).FirstOrDefault();
                rs.content = commentup;
                rs.updateat = DateTime.Now;
                _context.SaveChangesAsync();
                return rs;
            }catch (Exception ex) {
                return null;
            }
        }

     
    }
}