using PlaywrightBull.Bullmc;
using PlaywrightBull.Bullmc.Products;
using PlaywrightBull.CLI.Components.Table;

namespace PlaywrightBull.CLI
{
    internal static class ProductDetails
    {
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
                var relatedProductsTable = new ConsoleTable(marginLeft, marginTop);


                productTable.Add(row => row.Add(new Cell(Resources.Common.Common.Name, padLeft, padRight))
                                           .Add(new Cell(Resources.ProductTable.ProductTable.Price, padLeft, padRight))
                                           .Add(new Cell(Resources.ProductDetails.ProductDetails.Image, padLeft, padRight)))
                            .Add(row => row.Add(new Cell(product?.Name ?? string.Empty, padLeft, padRight))
                                           .Add(new Cell(product?.Price?.Text ?? "Unknown", padLeft, padRight))
                                           .Add(new Cell(product?.Image?.ToString() ?? string.Empty, padLeft, padRight)));


                if (product.InnerProducts.Count > 0)
                {

                    relatedProductsTable.Add(row => row.Add(new Cell(Resources.Common.Common.Name, padLeft, padRight))
                                        .Add(new Cell(Resources.ProductTable.ProductTable.Price, padLeft, padRight))
                                        .Add(new Cell(Resources.ProductDetails.ProductDetails.Image, padLeft, padRight)));

                    foreach (var innerProd in product.InnerProducts)
                    {

                        relatedProductsTable.Add(row => row.Add(new Cell(innerProd?.Name ?? string.Empty, padLeft, padRight))
                                                           .Add(new Cell(innerProd?.Price?.Text ?? "Unknown", padLeft, padRight))
                                                           .Add(new Cell(innerProd?.Image?.ToString() ?? string.Empty, padLeft, padRight)));
                    }

                    ConsoleKeyInfo consoleKeyInfo;

                    do
                    {
                        Console.Clear();
                        Console.Write(productTable);

                        Console.Write("\n" + new string(' ', marginLeft) + Resources.ProductDetails.ProductDetails.Related, foregroundColor: ConsoleColor.DarkYellow);

                        relatedProductsTable.Draw(selectionColor: ConsoleColor.DarkYellow);


                        if (!string.IsNullOrWhiteSpace(product.Description))
                        {
                            string leftMargin = new(' ', marginLeft);

                            Console.WriteLine($"\n{leftMargin}" + Resources.ProductDetails.ProductDetails.Description, foregroundColor: ConsoleColor.DarkYellow);
                            Console.WriteLine(($"\n" + product.Description).Replace("\n", $"\n{leftMargin}"));
                        }

                        Console.WriteLine("\n" + Resources.ProductTable.ProductTable.SelectOrBack);

                        consoleKeyInfo = Console.ReadKey();
                    } while (relatedProductsTable.ProcessInput(consoleKeyInfo));

                    switch (consoleKeyInfo.Key)
                    {
                        case ConsoleKey.Enter:
                            await Show(client, product.InnerProducts[relatedProductsTable.SelectedRow - 1]);
                            break;
                        default: return;
                    }

                    Console.WriteLine("\n" + Resources.ProductDetails.ProductDetails.Related);
                    Console.WriteLine("\n" + string.Join("\n", product.InnerProducts));
                }
                else
                {
                    Console.Write(productTable);

                    if (!string.IsNullOrWhiteSpace(product.Description))
                    {
                        string leftMargin = new(' ', marginLeft);

                        Console.WriteLine($"\n{leftMargin}" + Resources.ProductDetails.ProductDetails.Description, foregroundColor: ConsoleColor.DarkYellow);
                        Console.WriteLine(($"\n" + product.Description).Replace("\n", $"\n{leftMargin}"));
                    }

                    Console.Write("\n" + Resources.Common.Common.AnyKey);
                    Console.ReadKey();
                    return;
                }
            }
        }
    }
}
