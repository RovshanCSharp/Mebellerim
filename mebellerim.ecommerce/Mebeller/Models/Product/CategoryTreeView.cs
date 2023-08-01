namespace Mebeller.Models.Product
{
    public class CategoryTreeView
    {
        public int CategoryId { get; init; }
        public string CategoryName { get; init; }
        public int CategoryLevel { get; set; }
        public int CategoryProductsCount { get; init; }
        public int BlogsCount { get; set; }
     }
}