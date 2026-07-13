namespace PlaywrightBull.Bullmc.Products
{
    internal class Price(float value, string? vault)
    {
        public float Value => value;
        public string? Currency => vault;
    }
}
