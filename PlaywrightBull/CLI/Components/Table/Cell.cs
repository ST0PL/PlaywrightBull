namespace PlaywrightBull.CLI.Components.Table
{
    internal class Cell(string text, int paddingLeft = 0, int paddingRight = 0) : ICell
    {
        public static readonly Cell Empty = new(string.Empty, 0);
        public string Text => text;
        public int PaddingLeft => paddingLeft;
        public int PaddingRight => paddingRight;
    }
}
