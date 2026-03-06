using System.Reflection;
using System.Text.Json;
using Contentstack.Management.Core.Models;
using Contentstack.Management.Core.Models.Token;

namespace ApiSurface;

static class Program
{
    static int Main(string[] args)
    {
        string? outputPath = null;
        for (var i = 0; i < args.Length; i++)
        {
            if ((args[i] == "--output" || args[i] == "-o") && i + 1 < args.Length)
            {
                outputPath = args[++i];
                break;
            }
        }
        outputPath ??= "api-surface.json";

        var assembly = typeof(Stack).Assembly;
        var apiSurface = CollectApiSurface(assembly);
        var json = JsonSerializer.Serialize(apiSurface, new JsonSerializerOptions { WriteIndented = true });
        var dir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(outputPath, json);
        Console.WriteLine($"api-surface.json written: {Path.GetFullPath(outputPath)} ({apiSurface.Count} methods)");
        return 0;
    }

    static List<ApiSurfaceEntry> CollectApiSurface(Assembly assembly)
    {
        var list = new List<ApiSurfaceEntry>();
        var responseType = typeof(Contentstack.Management.Core.ContentstackResponse);
        var taskType = typeof(Task);

        var typesToScan = new[]
        {
            typeof(Stack),
            typeof(ContentType),
            typeof(Asset),
            typeof(BulkOperation),
            typeof(Locale),
            typeof(GlobalField),
            typeof(Release),
            typeof(ReleaseItem),
            typeof(Organization),
            typeof(Contentstack.Management.Core.Models.Environment),
            typeof(Folder),
            typeof(Entry),
            typeof(Webhook),
            typeof(Workflow),
            typeof(Role),
            typeof(Label),
            typeof(Contentstack.Management.Core.Models.Version),
            typeof(User),
            typeof(AuditLog),
            typeof(VariantGroup),
            typeof(Extension),
            typeof(PublishRule),
            typeof(PublishQueue),
            typeof(DeliveryToken),
            typeof(ManagementToken),
        };

        foreach (var type in typesToScan)
        {
            var component = type.Name;
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => !m.IsSpecialName) // skip get_*, set_*
                .Where(m =>
                {
                    var ret = m.ReturnType;
                    if (ret == responseType) return true;
                    if (ret.IsGenericType && ret.GetGenericTypeDefinition() == taskType)
                    {
                        var arg = ret.GetGenericArguments()[0];
                        return arg == responseType;
                    }
                    return false;
                });

            foreach (var m in methods.OrderBy(x => x.Name))
            {
                var methodName = m.Name;
                var key = $"{component}.{methodName}";
                list.Add(new ApiSurfaceEntry
                {
                    Component = component,
                    Method = methodName,
                    Key = key
                });
            }
        }

        return list;
    }

    private sealed class ApiSurfaceEntry
    {
        public string Component { get; set; } = "";
        public string Method { get; set; } = "";
        public string Key { get; set; } = "";
    }
}
