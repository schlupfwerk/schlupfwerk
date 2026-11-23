namespace Schlupfwerk.Web.Tests;

/// <summary>Finds files in the repository, starting from the test output folder.</summary>
internal static class SiteFiles
{
    /// <summary>The site/ folder (the one with Schlupfwerk.slnx).</summary>
    public static string SiteRoot { get; } = FindSiteRoot();

    /// <summary>The repository root, one level above site/.</summary>
    public static string RepoRoot => Path.GetDirectoryName(SiteRoot)!;

    public static string WebRoot => Path.Combine(SiteRoot, "src", "Schlupfwerk.Web", "wwwroot");

    private static string FindSiteRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "Schlupfwerk.slnx")))
            {
                return dir.FullName;
            }
        }

        throw new InvalidOperationException("Schlupfwerk.slnx not found above " + AppContext.BaseDirectory);
    }
}
