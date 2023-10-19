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
        public async Task<bool> createComment(Comments cmt)
        {
            try {
                Comments conmment = new Comments {
                    authoId = cmt.authoId,
                    createat = DateTime.Now,
                    deleteat = null,
                    parentId = cmt.parentId,
                    content = cmt.content
                };
                _context.Comments.Add(cmt);
                _context.SaveChangesAsync();
                return true;
            } catch (Exception ex) {
                return false;
            }
        }
        public async Task<bool> deleteComment(int CommentId)
        {
            try { 
                var rs = _context.Comments.FirstOrDefault(p => p.id == CommentId);
                rs.deleted = 1;
                rs.deleteat = DateTime.Now;
                _context.SaveChangesAsync();
                return true;
            } catch (Exception ex) {
                return false;
            }
        }

        public async Task<bool> updateComment(string content,int CommentId)
        {
            try {
                var rs = _context.Comments.FirstOrDefault(p => p.id == CommentId);
                rs.content = content;
                rs.createat = DateTime.Now;
                _context.SaveChangesAsync();
                return true;
            }catch (Exception ex) {
                return false;
            }
        }
    }
}