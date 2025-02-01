#nullable enable

using System.Text.Json;

namespace NkRn;

public class FetchedElement
{
    /// <summary>
    /// 要素のキー
    /// </summary>
    public string Key { get; set; } = "";

    /// <summary>
    /// フェッチした日時
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// フェッチした小説リスト
    /// </summary>
    public List<string> Novels { get; set; } = new();

    public void Update(List<string> novels)
    {
        DateTime = DateTime.Now;
        Novels = novels;
    }
}

public class FetchedDatabase
{
    public List<FetchedElement> Elements { get; set; } = new();
}

/// <summary>
/// フェッチ後にローカルに保存されたデータベース
/// </summary>
public class LocalDatabase
{
    private readonly FetchedDatabase _fetchedDatabase = new();

    private static LocalDatabase? _instance;
    public static LocalDatabase Instance => _instance ??= new LocalDatabase();

    private const string DatabasePath = "database.json";

    public LocalDatabase()
    {
        if (File.Exists(DatabasePath))
        {
            var json = File.ReadAllText(DatabasePath);
            _fetchedDatabase = JsonSerializer.Deserialize<FetchedDatabase>(json) ?? new FetchedDatabase();
        }
    }

    public void Store()
    {
        var json = JsonSerializer.Serialize(_fetchedDatabase);
        File.WriteAllText(DatabasePath, json);
    }

    public FetchedElement Fetch(string key)
    {
        var element = _fetchedDatabase.Elements.FirstOrDefault(e => e.Key == key);
        if (element == null)
        {
            element = new FetchedElement { Key = key };
            _fetchedDatabase.Elements.Add(element);
        }

        return element;
    }
}