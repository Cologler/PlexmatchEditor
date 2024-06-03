namespace PlexmatchEditor.Plexmatch;

public record PlexmatchYearRow : IPlexmatchRow
{
    public const string YearHeaderName = "year";

    public required int Year { get; set; }

    public void Write(WriteContext writeContext) => writeContext.To.Append($"{YearHeaderName}: ").Append(this.Year).AppendLine();

    public static IPlexmatchRow? TryParse(ReadOnlyMemory<char> line)
    {
        var trimedLine = line.Trim();
        foreach (var header in new string[] { YearHeaderName })
        {
            if (trimedLine.Span.StartsWith($"{header}:", StringComparison.OrdinalIgnoreCase))
            {
                var yearText = trimedLine[(header.Length + 1)..].Trim();
                if (int.TryParse(yearText.Span, out var year))
                {
                    return new PlexmatchYearRow 
                    { 
                        Year = year 
                    };
                }
            }
        }
        return default;
    }
}
