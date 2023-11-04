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
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;
using Microsoft.AspNetCore.Identity;

namespace It_Supporter.Repository
{
    public class Post : IPost
    {
        private readonly ThanhVienContext _context;
        private readonly Microsoft.Extensions.Hosting.IHostingEnvironment _env;
        private string apiKey = "AIzaSyAjJChu_wQMel-dk0m_KXzZ7rx4SXaXwwg";
        private string authDomain= "it-supporter-34e94.firebaseapp.com";
        private string projectId = "it-supporter-34e94";
        private string storageBucket ="it-supporter-34e94.appspot.com";

        private readonly string AuthEmail = "ndt13102003@gmail.com";
        private readonly string AuthPass = "leluuly1306";
        public Post(ThanhVienContext context, Microsoft.Extensions.Hosting.IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<bool> addNewPost(PostsUpload post)
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

                if(post.fileUpload.files.Length > 0) {
                    var _urlImage = await UploadPost(post.fileUpload);
                    newpost.urlImage = _urlImage; 
                }
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

    private async Task<string> UploadToFireBase(string FileName) {
        string foldername = "post";
        string path = Path.Combine(_env.ContentRootPath, $"Upload\\{foldername}");
        string filePath = Path.Combine(path,FileName);
        var objectname = $"post_img/{FileName}";
        var credentialPath = Path.Combine(_env.ContentRootPath,"configFirebase.json");
        var credential = GoogleCredential.FromFile(credentialPath).CreateScoped("https://www.googleapis.com/auth/cloud-platform");

        var storeClient = await StorageClient.CreateAsync(credential);

        var uploadOptions = new UploadObjectOptions {
            PredefinedAcl = PredefinedObjectAcl.PublicRead
        };

        using (FileStream fs = new FileStream(filePath,FileMode.Open))
        {
            var task = await storeClient.UploadObjectAsync(storageBucket,objectname,null,fs,uploadOptions);
            try {
                return task.MediaLink;
            } catch {
                return null;
            }
        }

    }
    public async Task<string> UploadPost(FileUpload file)
    {
        try {
            string FileName = $"{Path.GetFileNameWithoutExtension(file.fileName)}_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-tt")}{Path.GetExtension(file.files.FileName)}";
            string rootpath = $"Upload\\post";
            string pathfile = Path.Combine(rootpath,FileName);
            using (FileStream fs = new FileStream(Path.Combine(_env.ContentRootPath, pathfile), FileMode.Create))
            {
                file.files.CopyToAsync(fs);
                fs.FlushAsync();
            }
            string downloadUrl = await UploadToFireBase(FileName);
            if(downloadUrl != null) {
                File.Delete(Path.Combine(_env.ContentRootPath, pathfile));
            } 
            return downloadUrl;
        } catch(Exception ex) {
            return null;
        } 
    }

        public Task<Posts> GetPosts()
        {
            throw new NotImplementedException();
        }
    }
}
