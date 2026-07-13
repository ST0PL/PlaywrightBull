namespace PlaywrightBull
{
    internal static class Extensions
    {
        extension<T>(IReadOnlyList<T> values)
        {
            public int IndexOf(T value)
            {
                for (int i = 0; i < values.Count; i++)
                    if (values[i]?.Equals(value) ?? false)
                        return i;
                return -1;
            }

            public void ForEach(Action<T> action)
            {
                foreach (var value in values)
                    action.Invoke(value);
            }
        }

        extension(Console)
        {
            public static void WriteLine(string? value, ConsoleColor foregroundColor)
                => ColoredAction(value, Console.WriteLine, foregroundColor);

            public static void Write(string? value, ConsoleColor foregroundColor)
                => ColoredAction(value, Console.Write, foregroundColor);

            private static void ColoredAction(string? value, Action<string?> action, ConsoleColor foregroundColor)
            {
                ConsoleColor defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = foregroundColor;
                action.Invoke(value);
                Console.ForegroundColor = defaultColor;
            }
        }
    }
}
