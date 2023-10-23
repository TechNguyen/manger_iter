using System;
using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Data;
using Microsoft.Build.Framework;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace It_Supporter.Repository
{
    public class Post : IPost
    {
        private readonly ThanhVienContext _context;
        public Post(ThanhVienContext context)
        {
            _context = context;
        }

        public async Task<bool> addNewPost(Posts post)
        {
            try {  
                if(post.deleted == null) {
                    post.deleted = 0;
                }
                if(post.createat == null) {
                    post.createat = DateTime.Now;
                }
                Posts newpost = new Posts {
                    authorId = post.authorId,
                    content = post.content,
                    deleteat = post.deleteat,
                    updateat = post.updateat,
                    deleted = post.deleted,
                    createat = post.createat
                };
                _context.Posts.Add(newpost);
                _context.SaveChanges();
                return true;
            } catch (Exception ex) {
                return false;
            }
        }
        // an di 1 post
        public async Task<bool> hidenPost(int PostId)
        {
            try {
                var rs = _context.Posts.Where(p => p.id.Equals(PostId)).FirstOrDefault();
                rs.deleted = 1;
                _context.SaveChangesAsync();
                return true; 
            } catch (Exception ex) {
                return false; 
            }
        }

        // hien lai 1 bai viet
        public async Task<bool> showPost(int PostId)
        {
            try {
                var rs = _context.Posts.FirstOrDefault(p => p.id.Equals(PostId));
                rs.deleted = 0;
                _context.SaveChangesAsync();
                return true;
            } catch(Exception ex) {
                return false;
            }

        }

        public async Task<ICollection<Posts>> sortPost(DateSortPost dateSortPost)
        {
            try {
                var respondata = await _context.Posts.Where(p => DateTime.Compare(p.createat.Value,dateSortPost.fromdate.Value) >= 0)
                                                    .Where(p => DateTime.Compare(p.createat.Value,dateSortPost.todate.Value) <= 0)
                                                    .Where(p => p.deleted == 0)
                                                    .ToListAsync();
                return respondata;
            } catch(Exception ex) {
                return null;
            }
        }

        //Managed Post;
        public async Task<ICollection<Posts>> managed()  {
            try {   
                var result = _context.Posts.Where(p => p.deleted == 0).ToList();
                return result; 
            } catch(Exception ex) {
                return null;
            }
        }

        private async Task<bool> UploadPost(FileUpload file)
        {
            try {
                string FileName = $"{Path.GetFileNameWithoutExtension(file.fileName)}_{System.DateTime.Now.ToString("dd--mm--yyyy-h-mm-tt")}{Path.GetExtension(file.files.FileName)}"
                return true;
            } catch(Exception ex) {
                return false;
            }
        }
    }
}
