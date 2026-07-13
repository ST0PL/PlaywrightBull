namespace PlaywrightBull.CLI.Components.Table
{
    internal interface ICell
    {
        string Text { get; }
        public int PaddingLeft { get; }
        public int PaddingRight { get; }
    }
}
