namespace PlaywrightBull.CLI.Components.Table
{
    internal class Row : IRow<Cell>
    {
        private readonly List<Cell> _cells = [];

        public IReadOnlyList<Cell> Cells => _cells.AsReadOnly();

        IReadOnlyList<ICell> IRow.Cells => _cells.AsReadOnly();

        public IRow<Cell> Add(Cell cell)
        {
            _cells.Add(cell);
            return this;
        }

        public IRow<Cell> AddRange(IEnumerable<Cell> cells)
        {
            _cells.AddRange(cells);
            return this;
        }

        public IRow<Cell> Remove(int index)
        {
            _cells.RemoveAt(index);
            return this;
        }
    }
}
