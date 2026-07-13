namespace PlaywrightBull.CLI.Components.Table
{
    internal interface ITable<T1> where T1: IRow
    {
        IReadOnlyList<T1> Rows { get; }
        public int MarginLeft { get; }
        public int MarginTop { get; }
        ITable<T1> Add(Action<T1> rows);
        ITable<T1> AddRange(IEnumerable<Action<T1>> rowDelegates);
        ITable<T1> Remove(int index);
    }
}
