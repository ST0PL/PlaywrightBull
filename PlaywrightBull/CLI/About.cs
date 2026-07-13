using System.Diagnostics;

namespace PlaywrightBull.CLI
{
    internal static class About
    {
        public static void Show()
        {
            bool isRunning = true;
            while (isRunning)
            {
                int selection = Common.ShowNavMenu(
                    Constants.CreditsTitle,
                    Resources.About.About.OpenPage,
                    Resources.Common.Common.Back);

                switch (selection)
                {
                    case 1:
                        Process.Start(new ProcessStartInfo(Constants.GithubPage) { UseShellExecute = true });
                        break;
                    case 2:
                        isRunning = false;
                        break;
                    default:
                        continue;
                }
            }
        }
    }
}
