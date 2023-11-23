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
using Notification = It_Supporter.Models.Notification;
using It_Supporter.DataRes;

namespace It_Supporter.Repository
{
    public class Post : IPost
    {
        private readonly ThanhVienContext _context;
        private readonly Microsoft.Extensions.Hosting.IHostingEnvironment _env;

        private readonly IConfiguration _builder;
        private readonly INotiFication _noti;
        private string apiKey = "AIzaSyAjJChu_wQMel-dk0m_KXzZ7rx4SXaXwwg";
        private string authDomain= "it-supporter-34e94.firebaseapp.com";
        private string projectId = "it-supporter-34e94";
        private string storageBucket ="it-supporter-34e94.appspot.com";

        private readonly string AuthEmail = "ndt13102003@gmail.com";
        private readonly string AuthPass = "leluuly1306";
        public Post(ThanhVienContext context,
            Microsoft.Extensions.Hosting.IHostingEnvironment env,
            INotiFication noti,
            IConfiguration builder
            
            )
        {
            _context = context;
            _env = env;
            _noti = noti;
            _builder = builder;
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
                int row = _context.SaveChanges();
                if(row > 0)
                {
                    //add notification
                    Notification noti = new Notification();
                    noti.isRead = 0;
                    noti.CreateDate = DateTime.Now;
                    noti.NotiHeader = "Post notification!";
                    noti.NotiBody = "Admin has add new post!";
                    noti.FromUserId = post.authorId;
                    noti.ToUserId = "1";
                    bool res = await _noti.pushNotiWhenPost(noti);
                    return res;
                }
                return false;
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




        public async Task<ProducerResManagerPosts> managerPosts(Year year, IConfiguration builder)
        {
            try
            {
                ProducerResManagerPosts res = new ProducerResManagerPosts();
                res.listPosts = new List<inforPosts>();


                SqlConnection con = new SqlConnection();

                
                string conenction = builder.GetConnectionString("DefaultConnection");
                con.ConnectionString = conenction;
                string producername = "dbo.Sp_managerPost";

                using(SqlCommand cmd = new SqlCommand(producername, con)) {
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataTable dt = new DataTable();
                    using(SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {

                        cmd.Parameters.AddWithValue("@year", year.year);


                        SqlParameter countofyear = new SqlParameter();
                        countofyear.ParameterName = "@countofyear";
                        countofyear.SqlDbType = SqlDbType.Int;
                        countofyear.Direction= ParameterDirection.Output;
                        cmd.Parameters.Add(countofyear);


                        con.Open();

                        cmd.ExecuteNonQuery();


                        adapter.Fill(dt);

                        res.countPosts = int.Parse(cmd.Parameters["@countofyear"].Value.ToString());
                        foreach (DataRow row in dt.Rows)
                        {
                            inforPosts infor = new inforPosts();
                            infor.numPosts = int.Parse(row["numPosts"].ToString());
                            infor.month_create = int.Parse(row["month_create"].ToString());

                            res.listPosts.Add(infor);
                        }
                        res.statuscode = 200;
                        res.message = "Get manager post succesfully!";
                        con.Close();
                        return res;
                    }
                }
            } catch(Exception ex)
            {
                return null;
            }
        }
    }
}
