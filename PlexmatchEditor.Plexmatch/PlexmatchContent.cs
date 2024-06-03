using System.Text;

namespace PlexmatchEditor.Plexmatch;

public class PlexmatchContent
{
    static readonly Func<ReadOnlyMemory<char>, IPlexmatchRow?>[] s_ParseFuncs = 
    [
        PlexmatchTitleRow.TryParse,
        PlexmatchYearRow.TryParse,
        PlexmatchSeasonRow.TryParse,
        PlexmatchEpisodeRow.TryParse,
        PlexmatchOtherRow.TryParse,
    ];

    public List<IPlexmatchRow> Rows { get; } = [];

    public async ValueTask LoadFromAsync(Stream stream)
    {
        List<IPlexmatchRow> rows = [];

        using var reader = new StreamReader(stream);
        while (await reader.ReadLineAsync().ConfigureAwait(false) is { } line)
        {
            if (s_ParseFuncs.Select(x => x(line.AsMemory())).FirstOrDefault(x => x is not null) is { } row)
            {
                rows.Add(row);
            }
        }

        this.Rows.AddRange(rows);
    }

    public ValueTask<string> DumpAsync(WriteOptions? options = default)
    {
        var context = new WriteContext
        {
            To = new StringBuilder(),
            Options = options ?? new(),
            ScopeSeason = this.Rows.OfType<PlexmatchSeasonRow>().FirstOrDefault()?.Season
        };

        foreach (var row in this.Rows)
        {
            row.Write(context);
        }

        return new(context.To.ToString());
    }
}
