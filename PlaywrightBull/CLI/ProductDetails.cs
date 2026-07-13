using PlaywrightBull.Bullmc;
using PlaywrightBull.Bullmc.Products;
using PlaywrightBull.CLI.Components.Table;

namespace PlaywrightBull.CLI
{
    internal static class ProductDetails
    {
        private static readonly Dictionary<string, Product?> _otherProducts = [];
        public static async Task Show(BullmcClient client, Product? product)
        {
            while (true)
            {
                Console.Clear();

                if (product is null)
                    return;

                int padLeft = 2, padRight = 2;
                int marginLeft = 10, marginTop = 2;

                var productTable = new ConsoleTable(marginLeft, marginTop);
                var otherProductsTable = new ConsoleTable(marginLeft, marginTop);


                productTable.Add(row => row.Add(new Cell(Resources.Common.Common.Name, padLeft, padRight))
                                           .Add(new Cell(Resources.ProductTable.ProductTable.Price, padLeft, padRight))
                                           .Add(new Cell(Resources.ProductDetails.ProductDetails.Image)))
                            .Add(row => row.Add(new Cell(product?.Name ?? string.Empty, padLeft, padRight))
                                           .Add(new Cell($"{product?.Price?.Value} {product?.Price?.Currency}", padLeft, padRight))
                                           .Add(new Cell(product?.Image?.ToString() ?? string.Empty, padLeft, padRight)));


                if (product.Other.Count > 0)
                {

                    otherProductsTable.Add(row => row.Add(new Cell(Resources.Common.Common.Name, padLeft, padRight))
                                      .Add(new Cell(Resources.ProductTable.ProductTable.Price, padLeft, padRight))
                                      .Add(new Cell(Resources.ProductDetails.ProductDetails.Image)));


                    Task loadingScreenTask;

                    using (CancellationTokenSource cts = new())
                    {
                        loadingScreenTask = Common.ShowLoadingText(Resources.ProductTable.ProductTable.Loading, ".", 3, 500, cts.Token);

                        foreach (var otherLink in product.Other)
                        {
                            if (!_otherProducts.TryGetValue(otherLink, out var otherProduct))
                            {
                                // First search in the cache; if product not found, then parse content by link
                                otherProduct = _otherProducts.Values.Where(v => v?.Url?.ToString()?.Equals(otherLink) ?? false).FirstOrDefault() ?? await client.ParseProductAsync(otherLink);
                                _otherProducts.Add(otherLink, otherProduct);
                            }

                            otherProductsTable.Add(row => row.Add(new Cell(otherProduct?.Name ?? string.Empty, padLeft, padRight))
                                                             .Add(new Cell($"{otherProduct?.Price?.Value} {otherProduct?.Price?.Currency}", padLeft, padRight))
                                                             .Add(new Cell(otherProduct?.Image?.ToString() ?? string.Empty, padLeft, padRight)));
                        }

                        cts.Cancel();

                        try { await loadingScreenTask.WaitAsync(TimeSpan.FromSeconds(10)); } catch (TaskCanceledException) { }
                    }

                    ConsoleKeyInfo consoleKeyInfo;

                    do
                    {
                        Console.Clear();
                        Console.Write(productTable);

                        string leftMargin = new string(' ', marginLeft);

                        if (!string.IsNullOrWhiteSpace(product.Description))
                        {

                            Console.WriteLine($"\n{leftMargin}" + Resources.ProductDetails.ProductDetails.Description, foregroundColor: ConsoleColor.DarkYellow);
                            Console.WriteLine(($"\n" + product.Description).Replace("\n", $"\n{leftMargin}"));
                        }

                        Console.Write($"\n{leftMargin}" + Resources.ProductDetails.ProductDetails.Other, foregroundColor: ConsoleColor.DarkYellow);

                        otherProductsTable.Draw(selectionColor: ConsoleColor.DarkYellow);

                        Console.WriteLine("\n" + Resources.ProductDetails.ProductDetails.Refresh);
                        Console.WriteLine(Resources.ProductTable.ProductTable.SelectOrBack);

                        consoleKeyInfo = Console.ReadKey();
                    } while (otherProductsTable.ProcessInput(consoleKeyInfo));

                    switch (consoleKeyInfo.Key)
                    {
                        case ConsoleKey.Enter:
                            await Show(client, _otherProducts[product.Other[otherProductsTable.SelectedRow - 1]]);
                            break;
                        case ConsoleKey.R:
                            product.Other.ForEach(other => _otherProducts.Remove(other));
                            continue;
                        default: return;
                    }

                    Console.WriteLine("\n" + Resources.ProductDetails.ProductDetails.Other);
                    Console.WriteLine("\n" + string.Join("\n", product.Other));
                }
                else
                {
                    Console.Write(productTable);

                    if (!string.IsNullOrWhiteSpace(product.Description))
                    {
                        Console.WriteLine("\n" + Resources.ProductDetails.ProductDetails.Description);
                        Console.WriteLine("\n" + product.Description);
                    }

                    Console.Write("\n" + Resources.Common.Common.AnyKey);
                    Console.ReadKey();
                }
            }
        }
    }
}
