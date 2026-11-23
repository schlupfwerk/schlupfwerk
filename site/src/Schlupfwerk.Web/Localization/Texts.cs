using System.Text.Json;
using System.Text.Json.Serialization;

namespace Schlupfwerk.Web.Localization;

/// <summary>
/// The site's texts, read once from the embedded texts.pt.json and texts.en.json files.
/// A missing key renders as [[key]] so it is easy to spot (and a test fails on it).
/// </summary>
public sealed class Texts
{
    private readonly Dictionary<Lang, Dictionary<string, string>> _byLang = new()
    {
        [Lang.Pt] = Load(Lang.Pt),
        [Lang.En] = Load(Lang.En),
    };

    public string Get(Lang lang, string key) =>
        _byLang[lang].TryGetValue(key, out var value) ? value : $"[[{key}]]";

    public IReadOnlyDictionary<string, string> All(Lang lang) => _byLang[lang];

    private static Dictionary<string, string> Load(Lang lang)
    {
        var name = $"texts.{lang.Code()}.json";
        using var stream = typeof(Texts).Assembly.GetManifestResourceStream(name)
            ?? throw new InvalidOperationException($"Embedded resource '{name}' not found.");
        return JsonSerializer.Deserialize(stream, TextsJsonContext.Default.DictionaryStringString)
            ?? throw new InvalidOperationException($"'{name}' is empty.");
    }
}

[JsonSourceGenerationOptions(ReadCommentHandling = JsonCommentHandling.Skip)]
[JsonSerializable(typeof(Dictionary<string, string>))]
internal sealed partial class TextsJsonContext : JsonSerializerContext;
