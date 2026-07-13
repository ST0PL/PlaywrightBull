using PlaywrightBull.Bullmc;

namespace PlaywrightBull.CLI
{
    internal class Categories
    {
        public static async Task Show(BullmcClient client)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"{Resources.Categories.Categories.Title}\n", foregroundColor: ConsoleColor.DarkYellow);

                string[] categoryNames = [..Enum.GetNames<Category>().Select(Resources.Categories.Categories.ResourceManager.GetString)!];


                for (int i = 0; i < categoryNames.Length; i++)
                    Console.WriteLine($"    {i + 1}. {categoryNames[i]}");

                Console.WriteLine($"    {categoryNames.Length + 1}. {Resources.Common.Common.Back}");

                Console.Write($"\n{Resources.Common.Common.Select}");

                int categoryIndex;
                bool parsed;

                do
                {
                    parsed = int.TryParse(Console.ReadLine(), out var selectedCategory);
                    categoryIndex = selectedCategory - 1;

                    if (parsed && (Enum.IsDefined(typeof(Category), categoryIndex) || categoryIndex == categoryNames.Length))
                        break;

                    Console.Write(Resources.Common.Common.TryAgain);
                } while (true);

                if (categoryIndex == categoryNames.Length)
                    break;

                await ProductTable.Show(client, (Category)categoryIndex);
            }
        }
    }
}
