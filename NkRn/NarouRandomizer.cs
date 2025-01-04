#nullable enable

namespace NkRn;

public static class NarouRandomizer
{
    public static async Task Execute()
    {
        int minTextLength = inputMinTextLength();
        var novels = (await NarouFetch.FetchAllNovels(minTextLength)).Novels;

        Console.WriteLine("Fetched novels:");

        var random = new Random();
        while (true)
        {
            Console.WriteLine("Press enter to randomize a novel"); // TODO: or type 'q' to quit.
            Console.ReadLine();

            Console.WriteLine("-----------------------------------------------");

            const int takeCount = 10;
            for (int i = 0; i < takeCount; ++i)
            {
                var novel = novels[random.Next(novels.Count)];
                Console.Write(novel + ", ");
                Utils.OpenUrlInBrowser($"https://ncode.syosetu.com/novelview/infotop/ncode/{novel.ToLower()}/");
            }

            Console.Write("\n");

            Console.WriteLine("-----------------------------------------------");
        }
    }

    private static int inputMinTextLength()
    {
        const int defaultMinTextLength = 500000; // 50 万文字
        const int minimumMinTextLength = 200000; // 20 千文字

        while (true)
        {
            Console.Write(
                $"Enter the minimum word count for the body of the novel (Default: {defaultMinTextLength}): ");
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) return defaultMinTextLength;

            if (int.TryParse(input, out var minTextLength) && minTextLength >= minimumMinTextLength)
            {
                return minTextLength;
            }

            Console.WriteLine($"Please enter a positive integer greater than or equal to {minimumMinTextLength}.");
        }
    }
}