namespace PlaywrightBull.Bullmc.Products
{
    internal class Product(Uri productURL, Uri imageURL, string name, string? description, Price? price, IReadOnlyList<string> other)
    {
        public Uri Url => productURL;
        public Uri Image => imageURL;
        public string Name => name;
        public string? Description => description;
        public Price? Price => price;
        public IReadOnlyList<string> Other => other;
    }
}
