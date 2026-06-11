// Refresh regions.json from the Contentstack CDN.
//
// Works for both SDK developers and SDK consumers — no file copying needed.
// NuGet automatically places this file in your project's Scripts/ folder
// when you install the contentstack.management.csharp package.
//
// Usage (run from your project root after dotnet build):
//   dotnet run Scripts/refresh-region.cs
//
// Run whenever Contentstack adds a new region or service.

using System.IO;
using System.Net.Http;
using System.Text.Json;

const string RegionsUrl = "https://artifacts.contentstack.com/regions.json";

string root = Directory.GetCurrentDirectory();

Console.WriteLine($"Fetching {RegionsUrl} ...");

string json;
try
{
    using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
    json = await http.GetStringAsync(RegionsUrl);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"ERROR: Could not download regions.json: {ex.Message}");
    return 1;
}

JsonDocument doc;
try
{
    doc = JsonDocument.Parse(json);
}
catch (JsonException ex)
{
    Console.Error.WriteLine($"ERROR: Downloaded content is not valid JSON: {ex.Message}");
    return 1;
}

if (!doc.RootElement.TryGetProperty("regions", out var regionsEl))
{
    Console.Error.WriteLine("ERROR: Downloaded JSON does not contain a 'regions' key.");
    return 1;
}

int regionCount = regionsEl.GetArrayLength();

// ── All bin output dirs — finds every Contentstack.Management.Core.dll in bin/ ──
// Works for both the SDK repo and consumer projects after dotnet build.
// Writes Assets/regions.json next to each DLL found.
int binCount = 0;
foreach (string dll in Directory.GetFiles(root, "Contentstack.Management.Core.dll", SearchOption.AllDirectories))
{
    if (!dll.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar))
        continue;

    string binDest = Path.Combine(Path.GetDirectoryName(dll)!, "Assets", "regions.json");
    await WriteFile(binDest, json);
    Console.WriteLine($"[bin]    Wrote {regionCount} regions → {binDest}");
    binCount++;
}

if (binCount == 0)
    Console.WriteLine("[bin]    No build output found — run 'dotnet build' first, then re-run this script.");

return 0;

static async Task WriteFile(string path, string content)
{
    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    await File.WriteAllTextAsync(path, content);
}
