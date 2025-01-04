

// https://dev.syosetu.com/man/api/

namespace NkRn;

static class Program
{
    static async Task Main(string[] args)
    {
        var novels = await NarouFetch.FetchAllNovels(1000000);
        Console.WriteLine(novels.Novels.Count);
    }
}