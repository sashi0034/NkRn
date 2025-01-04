#nullable enable

using System.Text;

namespace NkRn;

public static class Utils
{
    /// <summary>
    /// Dictionary からクエリ文字列を生成するヘルパー
    /// </summary>
    public static string BuildQueryString(Dictionary<string, string> parameters)
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

    public static HttpClient GetHttpClient()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add(
            "User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36"
        );
        return httpClient;
    }
}