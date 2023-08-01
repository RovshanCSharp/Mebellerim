using System;
using System.Collections.Generic;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Models.Blog;
using Mebeller.Models.Media;
using Mebeller.Models.Product;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mebeller.Data.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrdersDetails { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<CommentReply> CommentReplies { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BlogPostComment> BlogPostComment { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureCategories(modelBuilder);
            ConfigureProducts(modelBuilder);
             ConfigureDiscounts(modelBuilder);
            ConfigureBlog(modelBuilder);
            ConfigureTotalPrice(modelBuilder);
        }

        private void ConfigureTotalPrice(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetails>()
                .Property(od => od.OrderDetailsTotalPrice)
                .HasConversion<double>(); 

            modelBuilder.Entity<OrderInvoiceDetails>()
                .Property(oid => oid.InvoiceDetailsTotalPrice)
                .HasConversion<double>(); 
            
            modelBuilder.Entity<OrderDetails>()
                .Property(od => od.OrderDetailsTotalPrice)
                .HasColumnType("decimal(18, 4)"); 

            modelBuilder.Entity<OrderInvoiceDetails>()
                .Property(oid => oid.InvoiceDetailsTotalPrice)
                .HasColumnType("decimal(18, 4)"); 
        }
        
        private void ConfigureBlog(ModelBuilder modelBuilder)
        {
            var blogPosts = new List<BlogPost>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Heading = "First Blog Post",
                    PageTitle = "First Blog Post",
                    Content = "This is the content of the first blog post.",
                    ShortDescription = "Short description of the first blog post.",
                    FeaturedImageUrl =
                        "https://images.sofology.co.uk/q_70,dpr_1.0,w_1600,c_scale,fl_lossy,f_auto/productmedia/lifestyle/sku000973164.jpg",
                    UrlHandle = "first-blog-post",
                    PublishedDate = DateTime.UtcNow,
                    Author = "John Doe",
                    Visible = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Heading = "Second Blog Post",
                    PageTitle = "Second Blog Post",
                    Content = "This is the content of the second blog post.",
                    ShortDescription = "Short description of the second blog post.",
                    FeaturedImageUrl =
                        "https://images.sofology.co.uk/q_70,dpr_1.0,w_1600,c_scale,fl_lossy,f_auto/productmedia/lifestyle/sku000973164.jpg",
                    UrlHandle = "second-blog-post",
                    PublishedDate = DateTime.UtcNow,
                    Author = "Jane Smith",
                    Visible = true
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Heading = "Third Blog Post",
                    PageTitle = "Third Blog Post",
                    Content = "This is the content of the third blog post.",
                    ShortDescription = "Short description of the third blog post.",
                    FeaturedImageUrl =
                        "https://images.sofology.co.uk/q_70,dpr_1.0,w_1600,c_scale,fl_lossy,f_auto/productmedia/lifestyle/sku000973164.jpg",
                    UrlHandle = "third-blog-post",
                    PublishedDate = DateTime.UtcNow,
                    Author = "David Johnson",
                    Visible = true
                }
            };
            modelBuilder.Entity<BlogPost>().HasData(blogPosts);
        }

        private void ConfigureProducts(ModelBuilder modelBuilder)
        {
            var products = new[]
            {
                new Product
                {
                    ProductId = 1,
                    ProductName = "Office Chair",
                    ProductDescription = "Comfortable chair for office use",
                    ProductPrice = 100,
                    ProductQuantityInStock = 50,
                    UserEmail = "mebeller@turn.az",
                    UserName = "Mebeller",
                    UserPhoneNumber = "12345678910"
                },
                new Product
                {
                    ProductId = 2,
                    ProductName = "Office Desk",
                    ProductDescription = "Spacious desk for office use",
                    ProductPrice = 200,
                    ProductQuantityInStock = 30,
                    UserEmail = "mebeller@turn.az",
                    UserName = "Mebeller",
                    UserPhoneNumber = "12345678910"
                },
                new Product
                {
                    ProductId = 3,
                    ProductName = "Office Desk 2",
                    ProductDescription = "Spacious desk for office use",
                    ProductPrice = 100,
                    ProductQuantityInStock = 30,
                    UserEmail = "mebeller@turn.az",
                    UserName = "Mebeller",
                    UserPhoneNumber = "12345678910"
                },
                new Product
                {
                    ProductId = 4,
                    ProductName = "Office Desk 3",
                    ProductDescription = "Spacious desk for office use",
                    ProductPrice = 200,
                    ProductQuantityInStock = 30,
                    UserEmail = "mebeller@turn.az",
                    UserName = "Mebeller",
                    UserPhoneNumber = "12345678910"
                },
                new Product
                {
                    ProductId = 5,
                    ProductName = "Office Desk 4",
                    ProductDescription = "Spacious desk for office use",
                    ProductPrice = 200,
                    ProductQuantityInStock = 30,
                    UserEmail = "mebeller@turn.az",
                    UserName = "Mebeller",
                    UserPhoneNumber = "12345678910"
                },
                new Product
                {
                    ProductId = 6,
                    ProductName = "Office Desk 5",
                    ProductDescription = "Spacious desk for office use",
                    ProductPrice = 200,
                    ProductQuantityInStock = 30,
                    UserEmail = "mebeller@turn.az",
                    UserName = "Mebeller",
                    UserPhoneNumber = "12345678910"
                },
                new Product
                {
                    ProductId = 7,
                    ProductName = "Office Desk 6",
                    ProductDescription = "Spacious desk for office use",
                    ProductPrice = 150,
                    ProductQuantityInStock = 30,
                    UserEmail = "mebeller@turn.az",
                    UserName = "Mebeller",
                    UserPhoneNumber = "12345678910"
                },
            };
            modelBuilder.Entity<Product>().HasData(products);
        }

        private void ConfigureCategories(ModelBuilder modelBuilder)
        {
            var categories = new[]
            {
                new Category { CategoryId = 1, CategoryName = "Ofis mebeli" },
                new Category { CategoryId = 2, CategoryName = "Mətbəx mebelləri" },
                new Category { CategoryId = 3, CategoryName = "Yataq otağı mebelləri" },
                new Category { CategoryId = 4, CategoryName = "Ev mebelləri" },
                new Category { CategoryId = 5, CategoryName = "Dolablar" },
                new Category { CategoryId = 6, CategoryName = "Divanlar" },
                new Category { CategoryId = 7, CategoryName = "Stol stul" },
                new Category { CategoryId = 8, CategoryName = "Jalüz və pərdə" },
                new Category { CategoryId = 9, CategoryName = "Bağ evi üçün mebellər" },
                new Category { CategoryId = 10, CategoryName = "Digərləri" }
            };
            modelBuilder.Entity<Category>().HasData(categories);
        }


        private void ConfigureDiscounts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Discount>().Property(d => d.Amount).HasColumnType("decimal(18, 4)");
            modelBuilder.Entity<Discount>().Property(d => d.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Discount>().Property(d => d.Amount).HasColumnType("decimal(18,2)");
        }
    }
}