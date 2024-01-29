using System.Windows.Markup;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

#nullable disable

namespace EntityFrame.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            for (int i = 0; i < 50; i++)
            {
                migrationBuilder.InsertData("Users",
                    columns: new[]{
                        "Id",
                        "UserName",
                        "Email",
                        "SecurityStamp",
                        "EmailConfirmed",
                        "PhoneNumberConfirmed",
                        "HomeAddress",
                        "TwoFactorEnabled",
                        "LockoutEnabled",
                        "AccessFailedCount"
                    },
                    values: new object[]{
                        Guid.NewGuid().ToString(),
                        "User-"+i.ToString("D3"),
                        "email-"+i.ToString()+"@gmail.com",
                        Guid.NewGuid().ToString(),
                        true,
                        true,
                        "...@#...",
                        false,
                        false,
                        0
                    }

                );
            }
        }



        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
