namespace PlaywrightBull.Bullmc.Products
{
    internal class Product(Uri imageURL, string name, string? description, Price? price, IReadOnlyList<Product> innerProducts)
    {
        public Uri Image => imageURL;
        public string Name => name;
        public string? Description => description;
        public Price? Price => price;
        public IReadOnlyList<Product> InnerProducts => innerProducts;
    }
}
