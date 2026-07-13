using System.Text;

namespace PlaywrightBull.CLI.Components.Table
{
    internal class ConsoleTable(int marginLeft = 0, int marginTop = 0) : ITable<Row>
    {
        private readonly List<Row> _rows = [];
        private int _selectedRow = 1;

        public int SelectedRow => _selectedRow;
        public IReadOnlyList<Row> Rows => _rows.AsReadOnly();
        public int MarginLeft => marginLeft;
        public int MarginTop => marginTop;

        public ITable<Row> Add(Action<Row> rowDelegate)
        {
            Row row = new();
            rowDelegate(row);
            _rows.Add(row);
            return this;
        }

        public ITable<Row> AddRange(IEnumerable<Action<Row>> rowDelegates)
        {
            foreach (var rowDelegate in rowDelegates)
                Add(rowDelegate);
            return this;
        }

        public ITable<Row> Remove(int index)
        {
            _rows.RemoveAt(index);
            return this;
        }

        public override string? ToString()
        {
            StringBuilder tableBuilder = new();

            List<int> maxColumnWidths = [];

            int columnCount = AlignCellCount();
            int columnIndex = 0;

            while (columnIndex < columnCount)
            {
                maxColumnWidths.
                    Add(_rows.Max(row =>
                        row.Cells[columnIndex].Text.Length + row.Cells[columnIndex].PaddingLeft + row.Cells[columnIndex].PaddingRight));
                columnIndex++;
            }

            tableBuilder.Append(new string('\n', MarginTop));

            tableBuilder.Append(GetHeader(maxColumnWidths));

            foreach (var row in _rows)
            {
                int rowIndex = _rows.IndexOf(row);
                StringBuilder rowBuilder = new();
                rowBuilder.Append(new string(' ', MarginLeft));

                foreach (var cell in row.Cells)
                {
                    int cellIndex = row.Cells.IndexOf(cell);
                    var text = $"{new string(' ', cell.PaddingLeft)}{cell.Text}{new string(' ', cell.PaddingRight)}";
                    
                    rowBuilder.Append('║');
                    rowBuilder.Append(AlignContentWidth(text, maxColumnWidths[cellIndex]));
                }

                rowBuilder.Append('║');
                tableBuilder.AppendLine(rowBuilder.ToString());

                if (rowIndex == _rows.Count - 1)
                    continue;

                tableBuilder.Append(GetRowSeparator(maxColumnWidths));
            }

            tableBuilder.Append(GetFooter(maxColumnWidths));

            return tableBuilder.ToString();
        }

        public void Draw(ConsoleColor selectionColor = ConsoleColor.DarkMagenta)
        {
            var defaultColor = Console.BackgroundColor;

            List<int> maxColumnWidths = [];

            int columnCount = AlignCellCount();
            int columnIndex = 0;

            while (columnIndex < columnCount)
            {
                maxColumnWidths.
                    Add(_rows.Max(row =>
                        row.Cells[columnIndex].Text.Length + row.Cells[columnIndex].PaddingLeft + row.Cells[columnIndex].PaddingRight));
                columnIndex++;
            }

            Console.Write(new string('\n', MarginTop));
            Console.Write(GetHeader(maxColumnWidths));

            foreach (var row in _rows)
            {
                int rowIndex = _rows.IndexOf(row);

                Console.Write(new string(' ', MarginLeft));

                if (rowIndex == _selectedRow)
                    Console.BackgroundColor = selectionColor;

                foreach (var cell in row.Cells)
                {
                    int cellIndex = row.Cells.IndexOf(cell);
                    var text = $"{new string(' ', cell.PaddingLeft)}{cell.Text}{new string(' ', cell.PaddingRight)}";
                    
                    Console.Write('║');
                    Console.Write(AlignContentWidth(text, maxColumnWidths[cellIndex]));
                }

                Console.WriteLine('║');

                Console.BackgroundColor = defaultColor;

                if (rowIndex == _rows.Count - 1)
                    continue;

                Console.Write(GetRowSeparator(maxColumnWidths));
            }

            Console.Write(GetFooter(maxColumnWidths));
        }

        public bool ProcessInput(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.DownArrow:
                    _selectedRow++;
                    break;
                case ConsoleKey.UpArrow:
                    _selectedRow--;
                    break;
                default: return false;
            }

            _selectedRow = _selectedRow == 0 ? _rows.Count-1 : _selectedRow == _rows.Count ? 1 : _selectedRow;
            return true;
        }

        private string GetHeader(List<int> columnWidths)
        {
            StringBuilder sb = new();
            int columnCount = columnWidths.Count;
            sb.Append($"{new string(' ', MarginLeft)}╔");

            for (int i = 0; i < columnCount; i++)
            {
                sb.Append(new string('═', columnWidths[i]));
                if (i == columnCount - 1)
                    continue;
                sb.Append('╦');
            }

            sb.Append('╗');
            sb.AppendLine();
            return sb.ToString();
        }

        private string GetRowSeparator(List<int> columnWidths)
        {
            StringBuilder sb = new();

            int columnCount = columnWidths.Count;
            sb.Append($"{new string(' ', MarginLeft)}╠");

            for (int i = 0; i < columnWidths.Count; i++)
            {
                sb.Append(new string('═', columnWidths[i]));
                if (i == columnCount - 1)
                    continue;
                sb.Append('╬');
            }

            sb.Append('╣');
            sb.AppendLine();

            return sb.ToString();
        }

        private string GetFooter(List<int> columnWidths)
        {
            StringBuilder sb = new();
            int columnCount = columnWidths.Count;
            sb.Append($"{new string(' ', MarginLeft)}╚");

            for (int i = 0; i < columnCount; i++)
            {
                sb.Append(new string('═', columnWidths[i]));
                if (i == columnCount - 1)
                    continue;
                sb.Append('╩');
            }

            sb.Append('╝');
            sb.AppendLine();

            return sb.ToString();
        }

        private int GetMaxCellCount()
            => _rows.Max(r => r.Cells.Count);

        private int AlignCellCount()
        {
            int columnCount = GetMaxCellCount();

            _rows.ForEach(row =>
            {
                int diff = columnCount - row.Cells.Count;
                for (int i = 0; i < diff; i++)
                    row.Add(Cell.Empty);
            });

            return columnCount;
        }

        private static string AlignContentWidth(string content, int columnWidth)
            => $"{content}{new string(' ', columnWidth- content.Length)}";
    }
}
