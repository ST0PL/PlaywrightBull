using Microsoft.Playwright;
using PlaywrightBull.Bullmc;

namespace PlaywrightBull
{
    internal class Program
    {
        static async Task Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            BullmcClient bullmc = await BullmcClient.CreateAsync(new BrowserTypeLaunchOptions { Headless = true });
            await bullmc.InitAsync();
            await CLI.Main.Show(bullmc);
        }
    }
}
