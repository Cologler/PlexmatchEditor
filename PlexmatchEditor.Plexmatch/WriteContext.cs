using System.Text;

namespace PlexmatchEditor.Plexmatch;

public readonly record struct WriteContext
{
    public required StringBuilder To { get; init; }

    public required WriteOptions Options { get; init; }

    public int? ScopeSeason { get; init; }
}
