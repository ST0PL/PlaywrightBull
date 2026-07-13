namespace PlaywrightBull.CLI
{
    internal static class Common
    {
        public static int ShowNavMenu(string title, params string[] points)
        {
            Console.Clear();
            Console.WriteLine(title, foregroundColor: ConsoleColor.DarkYellow);
            Console.WriteLine(string.Join("\n", points.Select((p, i) => $"    {i + 1}. {p}")));

            Console.Write($"\n{Resources.Common.Common.Select}");

            int selection;

            while (!int.TryParse(Console.ReadLine(), out selection))
                Console.Write(Resources.Common.Common.TryAgain);

            return selection;
        }

        public static Task ShowLoadingText(string text, string indicator, int indicatorRepeats, int millisecondsUpdateDelay, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                Console.Write(text);
                int i = 0;
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(millisecondsUpdateDelay);
                    if (i == indicatorRepeats)
                    {
                        Console.CursorLeft -= indicator.Length * indicatorRepeats;
                        Console.Write(new string(' ', indicator.Length * indicatorRepeats));
                        Console.CursorLeft -= indicator.Length * indicatorRepeats;
                        i = 0;
                        continue;
                    }

                    Console.Write(indicator);
                    i++;
                }
            }, cancellationToken);
        }
    }
}
