// https://dev.syosetu.com/man/api/

namespace NkRn;

internal static class Program
{
    static void Main(string[] args)
    {
        string modeIndex = args.Length > 0 ? args[0] : "";
        if (modeIndex == "")
        {
            Console.WriteLine("Please specify the mode:"
                              + "\n\tn - narou"
                              + "\n\tnr - narou 18"
                              + "\n\tnm - narou midnight"
                              + "\n\tk - kakuyomu");
            modeIndex = Console.ReadLine() ?? "";
        }

        switch (modeIndex)
        {
        case "n":
            NarouRandomizer.Execute().Wait();
            break;
        case "nr":
            Narou18Randomizer.Execute(false).Wait();
            break;
        case "nm":
            Narou18Randomizer.Execute(true).Wait();
            break;
        case "k":
            KakuyomuRandomizer.Execute().Wait();
            break;
        default:
            NarouRandomizer.Execute().Wait();
            break;
        }
    }
}