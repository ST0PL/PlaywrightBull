using Microsoft.Playwright;
using PlaywrightBull.Bullmc;
using System.Globalization;

namespace PlaywrightBull
{
    internal class Program
    {
        static async Task Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            BullmcClient bullmc;

            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            try
            {
                bullmc = await BullmcClient.CreateAsync(new BrowserTypeLaunchOptions { Headless = false });
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine(Resources.Main.Main.BullmcClientCreationFailed);
                Console.Write(Resources.Common.Common.AnyKeyContinue);
                Console.ReadKey();
                return;
            }
            await bullmc.InitAsync();
            await CLI.Main.Show(bullmc);
        }
    }
}
