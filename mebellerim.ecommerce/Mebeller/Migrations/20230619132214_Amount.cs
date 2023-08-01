using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mebeller.Migrations
{
    public partial class Amount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("608cbe5a-dbeb-4b09-89d3-b74617f4b605"));

            migrationBuilder.DeleteData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("dabcb683-6e4f-483b-b53d-a7ffc8ba0d3d"));

            migrationBuilder.DeleteData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("e9fbf564-4cc3-4140-83e0-856a17a6fc2b"));

            migrationBuilder.RenameColumn(
                name: "DiscountPercentage",
                table: "Discounts",
                newName: "Amount");

            migrationBuilder.InsertData(
                table: "BlogPosts",
                columns: new[] { "Id", "Author", "BlogHits", "CategoryId", "Content", "FeaturedImageUrl", "Heading", "PageTitle", "PublishedDate", "ShortDescription", "UrlHandle", "Visible" },
                values: new object[,]
                {
                    { new Guid("3334af6b-2633-40c7-b8e8-e75e713b5c09"), "John Doe", 0, null, "This is the content of the first blog post.", "https://images.sofology.co.uk/q_70,dpr_1.0,w_1600,c_scale,fl_lossy,f_auto/productmedia/lifestyle/sku000973164.jpg", "First Blog Post", "First Blog Post", new DateTimeOffset(new DateTime(2023, 6, 19, 13, 22, 13, 866, DateTimeKind.Unspecified).AddTicks(5730), new TimeSpan(0, 0, 0, 0, 0)), "Short description of the first blog post.", "first-blog-post", true },
                    { new Guid("631b287a-79df-4917-9a70-09a35814f1f2"), "Jane Smith", 0, null, "This is the content of the second blog post.", "https://images.sofology.co.uk/q_70,dpr_1.0,w_1600,c_scale,fl_lossy,f_auto/productmedia/lifestyle/sku000973164.jpg", "Second Blog Post", "Second Blog Post", new DateTimeOffset(new DateTime(2023, 6, 19, 13, 22, 13, 866, DateTimeKind.Unspecified).AddTicks(5743), new TimeSpan(0, 0, 0, 0, 0)), "Short description of the second blog post.", "second-blog-post", true },
                    { new Guid("90cac365-5c6a-4950-98c6-30256bc4ccfd"), "David Johnson", 0, null, "This is the content of the third blog post.", "https://images.sofology.co.uk/q_70,dpr_1.0,w_1600,c_scale,fl_lossy,f_auto/productmedia/lifestyle/sku000973164.jpg", "Third Blog Post", "Third Blog Post", new DateTimeOffset(new DateTime(2023, 6, 19, 13, 22, 13, 866, DateTimeKind.Unspecified).AddTicks(5745), new TimeSpan(0, 0, 0, 0, 0)), "Short description of the third blog post.", "third-blog-post", true }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 19, 13, 22, 13, 866, DateTimeKind.Utc).AddTicks(5311));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 19, 13, 22, 13, 866, DateTimeKind.Utc).AddTicks(5350));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 19, 13, 22, 13, 866, DateTimeKind.Utc).AddTicks(5352));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 19, 13, 22, 13, 866, DateTimeKind.Utc).AddTicks(5354));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 19, 13, 22, 13, 866, DateTimeKind.Utc).AddTicks(5356));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 19, 13, 22, 13, 866, DateTimeKind.Utc).AddTicks(5358));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 19, 13, 22, 13, 866, DateTimeKind.Utc).AddTicks(5359));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("3334af6b-2633-40c7-b8e8-e75e713b5c09"));

            migrationBuilder.DeleteData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("631b287a-79df-4917-9a70-09a35814f1f2"));

            migrationBuilder.DeleteData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("90cac365-5c6a-4950-98c6-30256bc4ccfd"));

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Discounts",
                newName: "DiscountPercentage");

            migrationBuilder.InsertData(
                table: "BlogPosts",
                columns: new[] { "Id", "Author", "BlogHits", "CategoryId", "Content", "FeaturedImageUrl", "Heading", "PageTitle", "PublishedDate", "ShortDescription", "UrlHandle", "Visible" },
                values: new object[,]
                {
                    { new Guid("608cbe5a-dbeb-4b09-89d3-b74617f4b605"), "John Doe", 0, null, "This is the content of the first blog post.", "https://images.sofology.co.uk/q_70,dpr_1.0,w_1600,c_scale,fl_lossy,f_auto/productmedia/lifestyle/sku000973164.jpg", "First Blog Post", "First Blog Post", new DateTimeOffset(new DateTime(2023, 6, 13, 21, 3, 17, 991, DateTimeKind.Unspecified).AddTicks(6049), new TimeSpan(0, 0, 0, 0, 0)), "Short description of the first blog post.", "first-blog-post", true },
                    { new Guid("dabcb683-6e4f-483b-b53d-a7ffc8ba0d3d"), "Jane Smith", 0, null, "This is the content of the second blog post.", "https://images.sofology.co.uk/q_70,dpr_1.0,w_1600,c_scale,fl_lossy,f_auto/productmedia/lifestyle/sku000973164.jpg", "Second Blog Post", "Second Blog Post", new DateTimeOffset(new DateTime(2023, 6, 13, 21, 3, 17, 991, DateTimeKind.Unspecified).AddTicks(6059), new TimeSpan(0, 0, 0, 0, 0)), "Short description of the second blog post.", "second-blog-post", true },
                    { new Guid("e9fbf564-4cc3-4140-83e0-856a17a6fc2b"), "David Johnson", 0, null, "This is the content of the third blog post.", "https://images.sofology.co.uk/q_70,dpr_1.0,w_1600,c_scale,fl_lossy,f_auto/productmedia/lifestyle/sku000973164.jpg", "Third Blog Post", "Third Blog Post", new DateTimeOffset(new DateTime(2023, 6, 13, 21, 3, 17, 991, DateTimeKind.Unspecified).AddTicks(6061), new TimeSpan(0, 0, 0, 0, 0)), "Short description of the third blog post.", "third-blog-post", true }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 13, 21, 3, 17, 991, DateTimeKind.Utc).AddTicks(5764));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 13, 21, 3, 17, 991, DateTimeKind.Utc).AddTicks(5772));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 13, 21, 3, 17, 991, DateTimeKind.Utc).AddTicks(5773));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 13, 21, 3, 17, 991, DateTimeKind.Utc).AddTicks(5774));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 13, 21, 3, 17, 991, DateTimeKind.Utc).AddTicks(5776));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 13, 21, 3, 17, 991, DateTimeKind.Utc).AddTicks(5777));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 13, 21, 3, 17, 991, DateTimeKind.Utc).AddTicks(5779));
        }
    }
}
