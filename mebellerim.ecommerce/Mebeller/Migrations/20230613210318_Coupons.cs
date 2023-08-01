using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mebeller.Migrations
{
    public partial class Coupons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("0008db09-3f06-4e26-acdc-d297c5d53eb7"));

            migrationBuilder.DeleteData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("2240c956-6b8e-4754-aa44-52377e0588d7"));

            migrationBuilder.DeleteData(
                table: "BlogPosts",
                keyColumn: "Id",
                keyValue: new Guid("bd63b33b-f005-4c6e-916a-c649d9e69216"));

            migrationBuilder.DropColumn(
                name: "DiscountPrice",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "DiscountValue",
                table: "Discounts");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPrice",
                table: "Discounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountValue",
                table: "Discounts",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.InsertData(
                table: "BlogPosts",
                columns: new[] { "Id", "Author", "BlogHits", "CategoryId", "Content", "FeaturedImageUrl", "Heading", "PageTitle", "PublishedDate", "ShortDescription", "UrlHandle", "Visible" },
                values: new object[,]
                {
                    { new Guid("0008db09-3f06-4e26-acdc-d297c5d53eb7"), "David Johnson", 0, null, "This is the content of the third blog post.", "https://images.sofology.co.uk/q_70,dpr_1.0,w_1600,c_scale,fl_lossy,f_auto/productmedia/lifestyle/sku000973164.jpg", "Third Blog Post", "Third Blog Post", new DateTimeOffset(new DateTime(2023, 6, 13, 13, 57, 14, 590, DateTimeKind.Unspecified).AddTicks(8320), new TimeSpan(0, 0, 0, 0, 0)), "Short description of the third blog post.", "third-blog-post", true },
                    { new Guid("2240c956-6b8e-4754-aa44-52377e0588d7"), "John Doe", 0, null, "This is the content of the first blog post.", "https://images.sofology.co.uk/q_70,dpr_1.0,w_1600,c_scale,fl_lossy,f_auto/productmedia/lifestyle/sku000973164.jpg", "First Blog Post", "First Blog Post", new DateTimeOffset(new DateTime(2023, 6, 13, 13, 57, 14, 590, DateTimeKind.Unspecified).AddTicks(8302), new TimeSpan(0, 0, 0, 0, 0)), "Short description of the first blog post.", "first-blog-post", true },
                    { new Guid("bd63b33b-f005-4c6e-916a-c649d9e69216"), "Jane Smith", 0, null, "This is the content of the second blog post.", "https://images.sofology.co.uk/q_70,dpr_1.0,w_1600,c_scale,fl_lossy,f_auto/productmedia/lifestyle/sku000973164.jpg", "Second Blog Post", "Second Blog Post", new DateTimeOffset(new DateTime(2023, 6, 13, 13, 57, 14, 590, DateTimeKind.Unspecified).AddTicks(8318), new TimeSpan(0, 0, 0, 0, 0)), "Short description of the second blog post.", "second-blog-post", true }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 13, 13, 57, 14, 590, DateTimeKind.Utc).AddTicks(7872));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 13, 13, 57, 14, 590, DateTimeKind.Utc).AddTicks(7881));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 13, 13, 57, 14, 590, DateTimeKind.Utc).AddTicks(7883));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 13, 13, 57, 14, 590, DateTimeKind.Utc).AddTicks(7885));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 13, 13, 57, 14, 590, DateTimeKind.Utc).AddTicks(7887));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 13, 13, 57, 14, 590, DateTimeKind.Utc).AddTicks(7889));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7,
                column: "RegistrationTime",
                value: new DateTime(2023, 6, 13, 13, 57, 14, 590, DateTimeKind.Utc).AddTicks(7890));
        }
    }
}
