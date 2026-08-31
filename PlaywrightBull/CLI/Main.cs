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
                    Resources.Main.Main.About,
                    Resources.Main.Main.Exit);

                switch (selection)
                {
                    case 1:
                        await Categories.Show(client);
                        break;
                    case 2:
                        About.Show();
                        break;
                    case 3:
                        isRunning = false;
                        break;
                    default:
                        continue;
                }
            }
        }
    }
}
