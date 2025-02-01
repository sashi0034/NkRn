#nullable enable

using System.Text.RegularExpressions;

namespace NkRn;

public static class KakuyomuRandomizer
{
    public static async Task Execute()
    {
        Console.WriteLine("Kakuyomu Randomizer");

        var novels = await RandomizerHelper.FetchNovels("kakuyomu", FetchKakuyomu);

        RandomizerHelper.RandomizeLoop("https://kakuyomu.jp/works", novels);
    }

    public static async Task<List<string>> FetchKakuyomu(int minTextLength)
    {
        int nextPage = 1;

        const int maxErrorCount = 3;
        int errorCount = 0;

        var random = new Random();

        List<string> novels = new();
        while (true)
        {
            try
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"Fetching... page={nextPage}        ");

                var pageNovels = await fetchPage(minTextLength, nextPage);
                if (pageNovels.Count == 0) break;

                novels.AddRange(pageNovels);

                nextPage++;
            }
            catch (Exception e)
            {
                errorCount++;
                if (errorCount >= maxErrorCount)
                {
                    Console.WriteLine($"\n[Error] Failed to fetch novels: {e.Message}");
                    break;
                }
            }

            // 適当な時間を待機
            await Task.Delay(random.NextInt64(0, 10) switch
            {
                <= 4 => 50,
                <= 8 => 50 * random.Next(2, 4),
                _ => 50 * random.Next(2, 16)
            });
        }

        novels = novels.Distinct().ToList();

        Console.SetCursorPosition(0, Console.CursorTop);
        Console.WriteLine($"Finished fetching novels: {novels.Count}");

        return novels;
    }

    private static async Task<List<string>> fetchPage(int minTextLength, int nextPage)
    {
        var uri = "https://kakuyomu.jp/search?order=published_at&ex_q=%E5%A5%B3%E6%80%A7%E5%90%91%E3%81%91+BL"
                  + $"&total_character_count_range={minTextLength}-"
                  + $"&page={nextPage}";
        using var httpClient = Utils.GetHttpClient();

        string responseString = await httpClient.GetStringAsync(uri);

        // ページ末尾であるか判定
        if (responseString.Contains("小説は見つかりませんでした"))
        {
            return new List<string>();
        }

        // 正規表現で作品リストを抽出
        string pattern = @"href=""/works/(\d+)""";
        var matches = Regex.Matches(responseString, pattern);
        List<string> temp = new();
        foreach (Match match in matches)
        {
            if (temp.Contains(match.Groups[1].Value)) continue;
            temp.Add(match.Groups[1].Value);
        }

        return temp;
    }
}