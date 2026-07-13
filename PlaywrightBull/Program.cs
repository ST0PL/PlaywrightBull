using Microsoft.Playwright;
using PlaywrightBull.Bullmc;
using System.Globalization;

namespace PlaywrightBull
{
    internal class Program
    {
        static async Task Main()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US", false);
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            BullmcClient bullmc = await BullmcClient.CreateAsync(new BrowserTypeLaunchOptions { Headless = false });
            await bullmc.InitAsync();
            await CLI.Main.Show(bullmc);
        }
    }
}
