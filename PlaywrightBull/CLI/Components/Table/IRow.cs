namespace PlaywrightBull.CLI.Components.Table
{
    internal interface IRow
    {
        IReadOnlyList<ICell> Cells { get; }
    }

    internal interface IRow<T> : IRow where T : ICell
    {
        IRow<T> Add(T cell);
        IRow<T> AddRange(IEnumerable<T> cell);
        IRow<T> Remove(int index);
    }
}
