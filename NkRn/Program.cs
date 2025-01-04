using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

// https://dev.syosetu.com/man/api/

namespace NkRn
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            string baseUrl = "https://api.syosetu.com/novelapi/api/";

            var parameters = new Dictionary<string, string>
            {
                { "of", "t-n" }, // title, ncode を抽出 (https://dev.syosetu.com/man/api/#output)
                { "notword", "女主人公" }, // 除外キーワード設定
                { "keyword", "1" }, // notword を keyword へ適応
                { "notbl", "1" },
                { "minlen", "500000" }, // 作品本文の最小文字数を 50 万字以上に設定
                { "out", "json" },
            };

            // QueryString を生成
            // 例: "?of=t-w-n-k&word=%E8%91%97%E4%BD%9C%E6%A8%A9%E3%83%95%E3%83%AA%E3%83%BC&keyword=1&out=json"
            string queryString = buildQueryString(parameters);
            string requestUrl = $"{baseUrl}{queryString}";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36"
            );

            string responseString;
            try
            {
                responseString = await httpClient.GetStringAsync(requestUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] GETリクエストに失敗しました: {ex.Message}");
                return;
            }

            // JSONをパースして整形
            try
            {
                // JSONをパース
                using var document = JsonDocument.Parse(responseString);

                // 整形出力用のオプション
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    // 日本語などマルチバイト文字をそのまま出力するための設定
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                };

                // 整形後のJSON文字列
                string prettyJson = JsonSerializer.Serialize(document.RootElement, jsonOptions);

                // 結果をコンソールに出力
                Console.WriteLine(prettyJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] JSONパースに失敗しました: {ex.Message}");
            }
        }

        /// <summary>
        /// Dictionary からクエリ文字列を生成するヘルパー
        /// </summary>
        private static string buildQueryString(Dictionary<string, string> parameters)
        {
            // 例: new Dictionary {{"a","b"}, {"c","d"} } => "?a=b&c=d"
            if (parameters.Count == 0) return string.Empty;

            var sb = new StringBuilder("?");
            bool first = true;
            foreach (var kvp in parameters)
            {
                if (!first) sb.Append("&");
                sb.Append(kvp.Key);
                sb.Append("=");
                sb.Append(Uri.EscapeDataString(kvp.Value));
                first = false;
            }

            return sb.ToString();
        }
    }
}