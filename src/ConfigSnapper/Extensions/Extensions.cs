namespace ConfigSnapper.Extensions;

internal static class Extensions
{
    internal static string GetAbsolutePath(this string path)
    {
        if (path.IsAbsolutePath())
            return path;

        if (path.StartsWith("./") || path.StartsWith(".\\"))
            path = path.Substring(2);
        return Path.Combine(AppContext.BaseDirectory, path);
    }

    private static bool IsAbsolutePath(this string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        // Check if the path is rooted (absolute)
        return Path.IsPathRooted(path) &&
            !string.IsNullOrEmpty(Path.GetPathRoot(path)?
            .Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }

    internal static bool IsEmpty(this string value) => value == "";

    internal static bool IsNotEmpty(this string value) => value != "";
}