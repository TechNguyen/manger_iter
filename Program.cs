using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using It_Supporter.realtime;
using It_Supporter.Repository;
using It_Supporter.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Filters;
using Hangfire;
using It_Supporter.BackGroundJob;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedSqlServerCache(options => {
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.SchemaName = "dbo";
    options.TableName = "Session";
});


builder.Services.AddControllers();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(60);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
           builder =>
           {
               builder.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//Hangfire

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});


builder.Services.AddScoped<IUserAccount, UserAccountRepo>();
builder.Services.AddScoped<IMember, Member>();
builder.Services.AddScoped<IPost,Post>();
builder.Services.AddScoped<IEmaiLService, SendEmail>();
builder.Services.AddScoped<ITechnical, Technical>();
builder.Services.AddScoped<ISendingMesage, messageProducer>();
builder.Services.AddScoped<INotiFication, NotificationRep>();
builder.Services.AddScoped<IComment, CommentRepo>(); 
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<ITokenService, UserAccountRepo>();
builder.Services.AddScoped<IExcel, Member>();
builder.Services.AddTransient<IbirthDay, BirthdayJob>();

builder.Services.Configure<SMTP>(builder.Configuration.GetSection("SMTPConfig"));
builder.Services.AddDbContext<ThanhVienContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDbContext<UserAccountContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
); ;

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
}
);



builder.Services.AddDefaultIdentity<IdentityUser>()
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<UserAccountContext>();



builder.Services.AddAuthorization(
        options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        }
    
    );
builder.Services.AddSignalR();

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));


//dang ky cac job
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseSession();

app.UseHttpLogging();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowAllOrigins");



app.UseHangfireDashboard();

app.MapHangfireDashboard();

var server = new BackgroundJobServer();


// Schedule a recurring background job
RecurringJob.AddOrUpdate<IbirthDay>(e => e.sendMailHappyBirday(), Cron.Daily);

app.Run();
