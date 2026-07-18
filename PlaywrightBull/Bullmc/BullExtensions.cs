using Microsoft.Playwright;
using PlaywrightBull.Bullmc.Products;

namespace PlaywrightBull.Bullmc
{
    internal static class BullExtensions
    {
        extension(BullmcClient bullmc)
        {
            public async Task<List<Product>> GetProductsAsync(Category category)
            {
                await bullmc.GotoMainAsync();

                int navIndex = (int)category;
                await bullmc.ClickNavButton(navIndex);

                await Task.Delay(500);

                var productsLocator = bullmc.Locator(".row.no-gutters > div:not([style*=\"display: none\"])");
                await productsLocator.First.WaitForAsync();

                var tasks = (await productsLocator.AllAsync()).Select(l=>l.Locator(".product-card > a").GetAttributeAsync("href"));
                var links = await Task.WhenAll(tasks);
                
                List<Product> result = [];

                foreach(var link in links)
                {
                    var product = await bullmc.ParseProductAsync(link);
                    if(product is not null)
                        result.Add(product);

                    await Task.Delay(300);
                }

                return result;
            }

            public async Task<Product?> ParseProductAsync(string? link)
            {
                if (link is null)
                    return null;

                await bullmc.GotoIfUrlNotEqualAsync(link);
                var columnLocator = bullmc.Locator(".row > div");
                var rowItemLocator = columnLocator.Locator(".row > div");
                await rowItemLocator.First.WaitForAsync();
                var rowItemLocators = await rowItemLocator.AllAsync();

                string imageURL = await rowItemLocators[0].Locator("img").GetAttributeAsync("src") ?? "";
                string? name = await rowItemLocators[1].Locator("h1").InnerTextAsync();
                string? description = null;

                try { description = await columnLocator.Locator("> p").First.InnerTextAsync(new() { Timeout = 500 }); }
                catch (TimeoutException) { }

                Price? price = await bullmc.GetPriceAsync(link);

                return new(new Uri(link), new Uri(imageURL), name, description, price, await bullmc.GetOtherProductsAsync(link));
            }

            public async Task<Price?> GetPriceAsync(string? link)
            {
                if (string.IsNullOrWhiteSpace(link))
                    return null;

                await bullmc.GotoIfUrlNotEqualAsync(link);

                string priceText = (await bullmc.Locator(".card-body > h3").AllInnerTextsAsync())[1].Split(" /")[0];
                var priceParts = priceText.Split();

                Price? price = null;

                if (!string.IsNullOrWhiteSpace(priceText) && float.TryParse(priceParts.Length > 2 ? $"{priceParts[0]}{priceParts[1]}" : priceParts[0], out float val))
                    price = new(val, priceParts[^1]);

                return price;

            }

            public async Task<IReadOnlyList<string>> GetOtherProductsAsync(string? link)
            {
                if (link is null)
                    return [];

                await bullmc.GotoIfUrlNotEqualAsync(link);

                var sameProductAnchorsLocator = bullmc.Locator(".same-product-sm");

                try { await sameProductAnchorsLocator.First.WaitForAsync(new() { Timeout = 1000 }); }
                catch (TimeoutException) { return []; }

                var anchorLocators = await sameProductAnchorsLocator.AllAsync();

                HashSet<string> links = [];

                foreach (var anchorLocator in anchorLocators)
                {
                    string? href = await anchorLocator.GetAttributeAsync("href");
                    if (href is not null)
                        links.Add(href);
                }

                return [.. links];
            }

            public async Task<List<HistoryItem>> GetHistoryAsync()
            {
                await bullmc.GotoMainAsync();
                var historyItemsLocator = bullmc.Locator(".swiper-slide .card-body");
                try { await historyItemsLocator.First.ScrollIntoViewIfNeededAsync(new() { Timeout = 5000 }); } // move carousel into viewport}
                catch (TimeoutException) { return []; }

                var historyItemLocators = await historyItemsLocator.AllAsync();

                List<HistoryItem> historyItems = [];

                await Task.Delay(300);

                foreach (var historyItemLocator in historyItemLocators)
                {
                    historyItems.Add(
                        new HistoryItem(new Uri(await historyItemLocator.Locator("img").GetAttributeAsync("src")),
                                        await historyItemLocator.Locator("h5").InnerTextAsync(),
                                        await historyItemLocator.Locator("p").InnerTextAsync()));
                }

                return historyItems;
            }
        }
    }
}