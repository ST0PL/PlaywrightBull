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
                await bullmc.ClickNavButton(navIndex, new() { Force = true });

                await Task.Delay(500);

                var productsLocator = bullmc.Locator("div > .flex.flex-col.rounded-3xl.overflow-hidden.w-full");
                
                List<Product> result = [];

                foreach(var locator in await productsLocator.AllAsync())
                {
                    var product = await bullmc.ParseItemAsync(locator);
                    if(product is not null)
                        result.Add(product);

                    await Task.Delay(300);
                }

                return result;
            }

            public async Task<Product?> ParseItemAsync(ILocator itemLocator)
            {
                var imageURL = new Uri(bullmc.HeadUrl, (await itemLocator.Locator("img").GetAttributeAsync("src"))!);
                var name = await itemLocator.Locator("p").Nth(0).TextContentAsync();
                var textPrice = await itemLocator.Locator("p[class*='text-[var(--accent-color)]']").TextContentAsync();
                var priceParts = textPrice.Split();

                int priceStartIndex = priceParts.Length > 1 ? 1 : 0; // check is the price a range

                if(priceStartIndex == 0)
                    return await bullmc.ParseProductAsync(itemLocator);
                
                var valueTextPrice = string.Join(string.Empty, priceParts[priceStartIndex..]);

                return new(
                    imageURL,
                    name!,
                    null,
                    new Price(textPrice, float.Parse(valueTextPrice[0..^1]), valueTextPrice[^1..]),
                    await bullmc.GetItemProductsAsync(itemLocator));
            }

            public async Task<IReadOnlyList<Product>> GetItemProductsAsync(ILocator itemLocator)
            {
                await itemLocator.Locator(".flex.p-3.rounded-2xl.cursor-pointer.f-smooth").ClickAsync(); // open products popup
                var productPopupButtons = await bullmc.Locator("div[role=\"dialog\"] .flex.p-3.rounded-2xl.cursor-pointer.f-smooth").AllAsync();

                var prods = new List<Product>();

                foreach(var btn in productPopupButtons)
                {
                    await btn.ClickAsync();

                    var prodPopupLocator = bullmc.Locator("[data-slot='dialog-content']").Last;

                    var imageURL = new Uri(bullmc.HeadUrl, (await prodPopupLocator.Locator("img").Nth(0).GetAttributeAsync("src"))!);
                    var name = await prodPopupLocator.Locator("p").Nth(0).TextContentAsync();
                    var textPrice = await prodPopupLocator.Locator("p[class*='text-[var(--accent-color)]']").Nth(0).TextContentAsync();

                    var valueTextPrice = textPrice.Replace(" ", "");

                    var descriptionLocator = prodPopupLocator.Locator(".opacity-50.mt-2.whitespace-pre-line");
                    string? description = null;

                    if (await descriptionLocator.CountAsync() > 0)
                        description = await descriptionLocator.TextContentAsync();

                    prods.Add(
                        new(
                            imageURL,
                            name!,
                            description,
                            new Price(textPrice, float.Parse(valueTextPrice[0..^1]), valueTextPrice[^1..]),
                            []));

                    await bullmc.PressAsync("Escape"); // close specific product popup
                    await Task.Delay(300);
                }

                await bullmc.PressAsync("Escape"); // close products popup

                return prods;
            }

            public async Task<Product> ParseProductAsync(ILocator itemLocator)
            {
                await itemLocator.Locator(".flex.p-3.rounded-2xl.cursor-pointer.f-smooth").ClickAsync(); // open product popup

                var prodPopupLocator = bullmc.Locator("[data-slot='dialog-content']").Last;

                var imageURL = new Uri(bullmc.HeadUrl, (await prodPopupLocator.Locator("img").Nth(0).GetAttributeAsync("src"))!);
                var name = await prodPopupLocator.Locator("p").Nth(0).TextContentAsync();
                var textPrice = await prodPopupLocator.Locator("p[class*='text-[var(--accent-color)]']").Nth(0).TextContentAsync();

                var valueTextPrice = textPrice.Replace(" ", "");

                var descriptionLocator = prodPopupLocator.Locator(".opacity-50.mt-2.whitespace-pre-line");
                string? description = null;

                if (await descriptionLocator.CountAsync() > 0)
                    description = await descriptionLocator.TextContentAsync();

                await bullmc.PressAsync("Escape"); // close products popup

                return new(
                        imageURL,
                        name!,
                        description,
                        new Price(textPrice, float.Parse(valueTextPrice[0..^1]), valueTextPrice[^1..]),
                        []);
            }
        }
    }
}