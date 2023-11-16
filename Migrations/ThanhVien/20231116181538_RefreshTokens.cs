using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace It_Supporter.Migrations.ThanhVien
{
    /// <inheritdoc />
    public partial class RefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    authorId = table.Column<string>(type: "char(10)", nullable: false),
                    postId = table.Column<int>(type: "int", nullable: false),
                    createat = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "Current_Timestamp"),
                    deleteat = table.Column<DateTime>(type: "datetime", nullable: true),
                    content = table.Column<string>(type: "ntext", nullable: false),
                    deleted = table.Column<int>(type: "int", nullable: true),
                    parentId = table.Column<int>(type: "int", nullable: true),
                    updateat = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    postId = table.Column<int>(type: "int", nullable: false),
                    url = table.Column<string>(type: "varchar(max)", nullable: false),
                    description = table.Column<string>(type: "ntext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customername = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    phonenumber = table.Column<string>(type: "char(10)", nullable: false),
                    machine_status = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    techId = table.Column<int>(type: "int", nullable: false),
                    services = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    machinesgetat = table.Column<DateTime>(type: "datetime", nullable: false),
                    TesterId = table.Column<string>(type: "char(10)", nullable: true),
                    Technical = table.Column<string>(type: "char(10)", nullable: true),
                    deleted = table.Column<byte>(type: "tinyint", nullable: false, defaultValueSql: "0"),
                    serviceCharger = table.Column<decimal>(type: "money", nullable: false, defaultValueSql: "0.00"),
                    note = table.Column<string>(type: "ntext", nullable: true),
                    finished = table.Column<byte>(type: "tinyint", nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotiId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ToUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotiHeader = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotiBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isRead = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotiId);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(450)", nullable: false),
                    refreshToken = table.Column<string>(type: "varchar(40)", nullable: false),
                    ExpriteTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "TechEvents",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    address = table.Column<string>(type: "nvarchar(300)", nullable: false),
                    techday = table.Column<DateTime>(type: "date", nullable: false),
                    timestart = table.Column<DateTime>(type: "datetime", nullable: false),
                    timeend = table.Column<DateTime>(type: "datetime", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechEvents", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "THANHVIEN",
                columns: table => new
                {
                    MaTV = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenTv = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Khoahoc = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nganhhoc = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoDT = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "date", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Chucvu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    namvaohoc = table.Column<int>(type: "int", nullable: false),
                    Ban = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    deleted = table.Column<byte>(type: "tinyint", nullable: true),
                    urlImage = table.Column<string>(type: "varchar(300)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THANHVIEN", x => x.MaTV);
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    Username = table.Column<string>(type: "varchar(100)", nullable: false),
                    Password = table.Column<string>(type: "varchar(20)", nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    authorId = table.Column<string>(type: "char(10)", nullable: false),
                    content = table.Column<string>(type: "ntext", nullable: true),
                    createat = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "Current_Timestamp"),
                    updateat = table.Column<DateTime>(type: "datetime", nullable: true),
                    deleteat = table.Column<DateTime>(type: "datetime", nullable: true),
                    deleted = table.Column<byte>(type: "tinyint", nullable: true),
                    urlImage = table.Column<string>(type: "varchar(300)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.id);
                    table.ForeignKey(
                        name: "FK_Posts_THANHVIEN_authorId",
                        column: x => x.authorId,
                        principalTable: "THANHVIEN",
                        principalColumn: "MaTV",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_authorId",
                table: "Posts",
                column: "authorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "TechEvents");

            migrationBuilder.DropTable(
                name: "UserAccounts");

            migrationBuilder.DropTable(
                name: "THANHVIEN");
        }
    }
}
