﻿using System;
using Bogus;
using Microsoft.EntityFrameworkCore.Migrations;
using razorweb.models;

#nullable disable

namespace EntityFrame.Migrations
{
    /// <inheritdoc />
    public partial class initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "ntext", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_articles", x => x.Id);
                });
            //Seed data
            Randomizer.Seed = new Random(8675309);
            var fakerArticle = new Faker<Article>();
            fakerArticle.RuleFor(a => a.Title, f => f.Lorem.Sentence(5, 5));
            fakerArticle.RuleFor(a => a.Content, f => f.Lorem.Paragraphs(1, 3));
            fakerArticle.RuleFor(a => a.Created, f => f.Date.Between(new DateTime(2023, 10, 2), new DateTime(2024, 3, 2)));

            for (var i = 0; i < 30; i++)
            {
                Article article = fakerArticle.Generate();
                //Insert data
                migrationBuilder.InsertData(
                    table: "articles",
                    columns: new[] { "Title", "Content", "Created" },
                    values: new object[]{
                    article.Title,
                    article.Content,
                    article.Created
                    }
                );
            }


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "articles");
        }
    }
}
