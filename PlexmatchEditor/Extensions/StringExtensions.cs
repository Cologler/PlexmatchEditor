namespace PlexmatchEditor.Extensions;

internal static class StringExtensions
{
    public static string ToUnixPath(this string str) => str.Replace("\\", "/");
}
