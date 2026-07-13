using PlaywrightBull.Bullmc;
using PlaywrightBull.Bullmc.Products;
using PlaywrightBull.CLI.Components.Table;

namespace PlaywrightBull.CLI
{
    internal class ProductTable
    {
        private static readonly Dictionary<Category, List<Product>> _cache = [];

        public static async Task Show(BullmcClient client, Category category)
        {
            while (true)
            {
                Console.Clear();

                Task loadingScreenTask;

                List<Product>? products;

                using (CancellationTokenSource cts = new())
                {
                    loadingScreenTask = Common.ShowLoadingText(Resources.ProductTable.ProductTable.Loading, ".", 3, 500, cts.Token);

                    if (!_cache.TryGetValue(category, out products))
                    {
                        products = await client.GetProductsAsync(category);
                        _cache.Add(category, products);
                    }

                    cts.Cancel();

                    try { await loadingScreenTask.WaitAsync(TimeSpan.FromSeconds(10)); } catch(TaskCanceledException) { }
                }

                if (products.Count < 1)
                {
                    Console.Clear();
                    Console.WriteLine(Resources.Common.Common.NothingFound, foregroundColor: ConsoleColor.DarkYellow);
                    Console.Write(Resources.Common.Common.AnyKey);
                    Console.ReadKey();
                    return;
                }

                int padLeft = 2, padRight = 2;
                int marginLeft = 10, marginTop = 1;
                var table = new ConsoleTable(marginLeft, marginTop);


                table.Add(row => row.Add(new Cell("№", padLeft, padRight))
                                    .Add(new Cell(Resources.Common.Common.Name, padLeft, padRight))
                                    .Add(new Cell(Resources.ProductTable.ProductTable.Price, padLeft, padRight)));

                for (int i = 0; i < products.Count; i++)
                    table.Add(row => row.Add(new Cell((i+1).ToString(), padLeft, padRight))
                                        .Add(new Cell(products[i].Name ?? string.Empty, padLeft, padRight))
                                        .Add(new Cell($"{products[i].Price?.Value} {products[i].Price?.Currency}", padLeft, padRight)));

                ConsoleKeyInfo keyInfo;

                string leftMargin = new(' ', marginLeft);
                string topMargin = new('\n', marginTop);

                do
                {
                    Console.Clear();
                    Console.WriteLine(topMargin + leftMargin + Resources.ProductTable.ProductTable.ResourceManager.GetString(category.ToString())!.Replace("\n", $"\n{leftMargin}"), foregroundColor: ConsoleColor.DarkYellow);
                    table.Draw(selectionColor: ConsoleColor.DarkYellow);
                    Console.WriteLine("\n" + Resources.Common.Common.Refresh);
                    Console.WriteLine(Resources.ProductTable.ProductTable.SelectOrBack);

                    keyInfo = Console.ReadKey();
                } while (table.ProcessInput(keyInfo));

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        await ProductDetails.Show(client, products[table.SelectedRow - 1]);
                        break;
                    case ConsoleKey.R:
                        _cache.Remove(category);
                        break;
                    default: return;
                }
            }
        }
    }
}
