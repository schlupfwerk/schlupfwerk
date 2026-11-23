// Puts the SHA-256 hashes of the inline <script> blocks of the published index.html (the import map that
// .NET writes at publish time) into the Content-Security-Policy of staticwebapp.config.json, replacing the
// __INLINE_SCRIPT_HASHES__ placeholder. Run after `dotnet publish`:
//
//   dotnet run tools/csp-hashes.cs -- <publish folder>/wwwroot

using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

const string Placeholder = "__INLINE_SCRIPT_HASHES__";

if (args.Length != 1)
{
    Console.Error.WriteLine("usage: dotnet run tools/csp-hashes.cs -- <published wwwroot>");
    return 2;
}

var indexPath = Path.Combine(args[0], "index.html");
var configPath = Path.Combine(args[0], "staticwebapp.config.json");

var html = File.ReadAllText(indexPath);
var hashes = Regex.Matches(html, @"<script(?![^>]*\bsrc=)[^>]*>(.*?)</script>", RegexOptions.Singleline | RegexOptions.IgnoreCase)
    .Select(m => m.Groups[1].Value)
    .Where(body => !string.IsNullOrWhiteSpace(body))
    // Browsers hash the script text after HTML parsing, which turns CRLF and CR into LF.
    .Select(body => body.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n'))
    .Select(body => $"'sha256-{Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(body)))}'")
    .Distinct()
    .ToList();

var config = File.ReadAllText(configPath);
if (!config.Contains(Placeholder, StringComparison.Ordinal))
{
    Console.Error.WriteLine($"{configPath} has no {Placeholder} placeholder (already processed?).");
    return 1;
}

config = config.Replace(" " + Placeholder, hashes.Count == 0 ? "" : " " + string.Join(' ', hashes), StringComparison.Ordinal);
File.WriteAllText(configPath, config);

Console.WriteLine($"CSP: {hashes.Count} inline script hash(es) written to {configPath}");
foreach (var hash in hashes)
{
    Console.WriteLine("  " + hash);
}

return 0;
