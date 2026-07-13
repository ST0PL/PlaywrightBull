using PlaywrightBull.Bullmc;

namespace PlaywrightBull.CLI
{
    internal static class Main
    {
        public static async Task Show(BullmcClient client)
        {
            bool isRunning = true;

            while (isRunning)
            {
                int selection = Common.ShowNavMenu(
                    Constants.MainTitle,
                    Resources.Main.Main.Products,
                    Resources.Main.Main.RecentPurchaces,
                    Resources.Main.Main.About,
                    Resources.Main.Main.Exit);

                switch (selection)
                {
                    case 1:
                        await Categories.Show(client);
                        break;
                    case 2:
                        await History.Show(client);
                        break;
                    case 3:
                        About.Show();
                        break;
                    case 4:
                        isRunning = false;
                        break;
                    default:
                        continue;
                }
            }
        }
    }
}
