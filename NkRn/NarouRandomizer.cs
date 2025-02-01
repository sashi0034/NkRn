#nullable enable

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace NkRn;

public static class NarouRandomizer
{
    public static async Task Execute()
    {
        Console.WriteLine("Narou Randomizer");

        var novels = await RandomizerHelper.FetchNovels("narou", FetchNarou);

        RandomizerHelper.RandomizeLoop("https://ncode.syosetu.com/novelview/infotop/ncode", novels);
    }

    public static async Task<List<string>> FetchNarou(int minTextLength)
    {
        int nextMinTextLength = minTextLength;

        const int maxErrorCount = 3;
        int errorCount = 0;

        List<string> novels = new();
        while (true)
        {
            try
            {
                var batchResult = await fetchBatch(nextMinTextLength);
                novels.AddRange(batchResult.Novels);

                if (batchResult.RemainCount <= 0) break;

                nextMinTextLength = batchResult.LastTextLength + 1;

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"Fetching... {batchResult.RemainCount}        ");
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
        }

        Console.SetCursorPosition(0, Console.CursorTop);
        Console.WriteLine($"Finished fetching novels: {novels.Count}");

        return novels;
    }

    private record BatchResult(
        List<string> Novels,
        int RemainCount,
        int LastTextLength);

    private static async Task<BatchResult> fetchBatch(int minTextLength)
    {
        string baseUrl = "https://api.syosetu.com/novelapi/api/";

        var parameters = new Dictionary<string, string>
        {
            { "of", "n-l" }, // ncode, length を抽出 (https://dev.syosetu.com/man/api/#output)
            { "notword", "女主人公" }, // 除外キーワード設定
            { "keyword", "1" }, // notword を keyword へ適応
            { "notbl", "1" },
            { "minlen", $"{minTextLength}" }, // 作品本文の最小文字数を設定
            { "lim", "500" }, // 最大出力数
            { "order", "lengthasc" }, // 作品本文の文字数が少ない順
            { "out", "json" },
        };

        // QueryString を生成
        // 例: "?of=t-w-n-k&word=%E8%91%97%E4%BD%9C%E6%A8%A9%E3%83%95%E3%83%AA%E3%83%BC&keyword=1&out=json"
        string queryString = Utils.BuildQueryString(parameters);
        string requestUrl = $"{baseUrl}{queryString}";

        using var httpClient = Utils.GetHttpClient();

        string responseString;
        try
        {
            responseString = await httpClient.GetStringAsync(requestUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Failed to get request: {ex.Message}");
            throw;
        }

        // Handling JSON
        try
        {
            using var document = JsonDocument.Parse(responseString);

            int allocant = 0;
            var novels = new List<string>();
            int lastTextLength = 0;
            foreach (var element in document.RootElement.EnumerateArray())
            {
                if (element.TryGetProperty("allcount", out var allCountElem))
                {
                    allocant = allCountElem.GetInt32();
                    continue;
                }

                if (element.TryGetProperty("length", out var lengthElem))
                {
                    lastTextLength = Math.Max(lastTextLength, lengthElem.GetInt32());
                }

                if (element.TryGetProperty("ncode", out var e))
                {
                    var s = e.GetString();
                    if (s != null) novels.Add(s);
                }
            }

            return new BatchResult(novels, Math.Max(0, allocant - novels.Count), lastTextLength);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Failed to parse json: {ex.Message}");
            throw;
        }
    }

    private static void debugPrintJson(JsonDocument document)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        string prettyJson = JsonSerializer.Serialize(document.RootElement, jsonOptions);
        Console.WriteLine(prettyJson);
    }
}