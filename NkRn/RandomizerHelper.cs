#nullable enable

namespace NkRn;

public static class RandomizerHelper
{
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

    public static async Task<List<string>> FetchNovels(string baseKey, Func<int, Task<List<string>>> fetchFunc)
    {
        int minTextLength = inputMinTextLength();

        var fetchedElement = LocalDatabase.Instance.Fetch($"{baseKey}:{minTextLength.ToString()}");
        if (fetchedElement.Novels.Count > 0)
        {
            Console.WriteLine($"Last fetched: {fetchedElement.DateTime} ({fetchedElement.Novels.Count} novels)");
            Console.WriteLine($"Do you want to refresh the list? [y/N]");
            var input = Console.ReadLine();
            if (input?.ToLower() != "y") return fetchedElement.Novels;
        }

        var novels = await fetchFunc(minTextLength);

        fetchedElement.Update(novels);
        LocalDatabase.Instance.Store();

        return novels;
    }

    public static void RandomizeLoop(string baseUri, List<string> novels)
    {
        var random = new Random();
        while (true)
        {
            Console.WriteLine("Press enter to randomize a novel"); // TODO: or type 'q' to quit.
            Console.ReadLine();

            Console.WriteLine("-----------------------------------------------");

            const int takeCount = 10;
            for (int i = 0; i < takeCount; ++i)
            {
                var novel = novels[random.Next(novels.Count)].ToLower();
                Console.Write(novel + ", ");
                Utils.OpenUrlInBrowser($"{baseUri}/{novel}");
            }

            Console.Write("\n");

            Console.WriteLine("-----------------------------------------------");
        }
    }
}