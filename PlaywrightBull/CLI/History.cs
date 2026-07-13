using PlaywrightBull.Bullmc;
using PlaywrightBull.Bullmc.Products;
using PlaywrightBull.CLI.Components.Table;

namespace PlaywrightBull.CLI
{
    internal static class History
    {
        public static async Task Show(BullmcClient client)
        {
            Task loadingScreenTask;
            List<HistoryItem> history;

            while (true)
            {
                Console.Clear();

                using (CancellationTokenSource cts = new())
                {
                    loadingScreenTask = Common.ShowLoadingText(Resources.ProductTable.ProductTable.Loading, ".", 3, 500, cts.Token);

                    history = await client.GetHistoryAsync();

                    cts.Cancel();
                    try { await loadingScreenTask.WaitAsync(TimeSpan.FromSeconds(10)); } catch (TaskCanceledException) { }
                }

                if(history.Count < 1)
                {
                    Console.Clear();
                    Console.WriteLine(Resources.Common.Common.NothingFound, foregroundColor: ConsoleColor.DarkYellow);
                    Console.Write(Resources.Common.Common.AnyKey);
                    Console.ReadKey();
                    return;
                }

                Console.Clear();

                int padLeft = 2, padRight = 2;
                int marginLeft = 10, marginTop = 2;

                string leftMargin = new(' ', marginLeft);
                string topMargin = new('\n', marginTop);

                Console.Write(topMargin + leftMargin + Resources.History.History.Title.Replace("\n", $"\n{leftMargin}"), foregroundColor: ConsoleColor.DarkYellow);

                var historyTable = new ConsoleTable(marginLeft, marginTop);

                historyTable.Add(row => row.Add(new Cell(Resources.Common.Common.Name, padLeft, padRight))
                                           .Add(new Cell(Resources.History.History.Nickname, padLeft, padRight))
                                           .Add(new Cell(Resources.Common.Common.Image, padLeft, padRight)));

                foreach (var historyItem in history)
                {
                    historyTable.Add(row => row.Add(new Cell(historyItem.ProductName, padLeft, padRight))
                                               .Add(new Cell(historyItem.Nickname, padLeft, padRight))
                                               .Add(new Cell(historyItem.Image.ToString(), padLeft, padRight)));
                }

                Console.WriteLine(historyTable);

                Console.WriteLine(Resources.Common.Common.Refresh);
                Console.WriteLine(Resources.Common.Common.AnyKey);

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.R:
                        break;
                    default: return;
                }
            }
        }
    }
}
