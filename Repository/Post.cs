using System;
using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Data;

namespace It_Supporter.Repository
{
    public class Post : IPost
    {
        private readonly ThanhVienContext _context;
        public Post(ThanhVienContext context)
        {
            _context = context;
        }

        public ProducerResAddPost addPost(IConfiguration builder, string context)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = builder.GetConnectionString("DefaultConnection");
            connection.Open();

            string procedurename = "dbo.SP_addPost";

            var content = new SqlParameter("@content", SqlDbType.NVarChar)
            {
                Direction = ParameterDirection.Input,
                Value = context
            };
            using (SqlCommand command = new SqlCommand(procedurename, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(content);
                command.ExecuteReader();

                ProducerResAddPost result = new ProducerResAddPost()
                {
                    returncode = 200,
                    returnmessage = "Add a post successfully!"
                };
                connection.Close();
                return result;
            }
        }
        // an di 1 post
        public ProducerResAddPost hidenPost(int PostId, IConfiguration builder)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = builder.GetConnectionString("DefaultConnection");
            connection.Open();
            if(!_context.Posts.Any(p => p.Id == PostId))
            {
                return new ProducerResAddPost();
            }
            string procedurename = "dbo.SP_hidenPost";
            var postId = new SqlParameter("@PostId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Input,
                Value = PostId
            };
            using (SqlCommand command = new SqlCommand(procedurename,connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(postId);
                command.ExecuteNonQuery();
                ProducerResAddPost reshidenPost = new ProducerResAddPost()
                {
                    returncode = 200,
                    returnmessage = $"Post {PostId} was hiden successfully!"
                };
                connection.Close();
                return reshidenPost;
            }
        }

        // hien lai 1 bai viet
        public ProducerResAddPost showPost(int PostId, IConfiguration builder)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = builder.GetConnectionString("DefaultConnection");
            connection.Open();
            string procedurename = "dbo.SP_showPost";
            var postId = new SqlParameter("@PostId", SqlDbType.Int) { 
                Direction = ParameterDirection.Input,
                Value = PostId
            };

            using (SqlCommand command = new SqlCommand(procedurename, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(postId);


                int rowaffected = command.ExecuteNonQuery();
                ProducerResAddPost resultshowpost = new ProducerResAddPost();
                if (rowaffected > 0)
                {
                    resultshowpost.returncode = 200;
                    resultshowpost.returnmessage = $"PostId {PostId} show successfully!";
                    
                } else
                {
                    resultshowpost.returncode = 401 ;
                    resultshowpost.returnmessage = $"PostId {PostId} cann't be found!";
                }
                connection.Close();
                return resultshowpost;
            }
        }

        // loc bai viet theo ngay thang
        public ICollection<Posts> sortPost(DateTime fromdate, DateTime todate, IConfiguration builder) {
            ICollection<Posts> result = new List<Posts>();
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = builder.GetConnectionString("DefaultConnection");
            connection.Open();
            string procedurename = "dbo.SP_sortPostByDate";
           
            var fromdateime = new SqlParameter("@fromdate", SqlDbType.DateTime) {
                Direction = ParameterDirection.Input,
                Value = fromdate
            };
            var todateime = new SqlParameter("@todate", SqlDbType.DateTime) {
                Direction = ParameterDirection.Input,
                Value = todate
            };
            using (SqlCommand command = new SqlCommand(procedurename,connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(fromdateime);
                command.Parameters.Add(todateime);
                //return table
                using (SqlDataAdapter adapter = new SqlDataAdapter(command)) {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    foreach (DataRow item in table.Rows)
                    {
                       Posts post = new Posts {
                            Id = Int32.Parse(item["Id"].ToString()),
                            CreatePost = DateTime.Parse(item["CreatePost"].ToString()),
                            Content = item["Content"].ToString()
                        };
                        result.Add(post);
                    }
                    return result;
                }        
            }


        }
    }
}
