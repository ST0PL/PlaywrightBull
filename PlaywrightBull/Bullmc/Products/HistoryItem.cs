namespace PlaywrightBull.Bullmc.Products
{
    internal class HistoryItem(Uri imageURL, string productName, string nickname)
    {
        public Uri Image => imageURL;
        public string ProductName => productName;
        public string Nickname => nickname;
    }
}
