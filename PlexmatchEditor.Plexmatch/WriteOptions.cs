namespace PlexmatchEditor.Plexmatch;

public record WriteOptions
{
    public PlexmatchTitleHeaderStyle? PreferTitleHeaderStyle {  get; set; }

    public PlexmatchEpisodeHeaderStyle? PreferEpisodeHeaderStyle { get; set; }

    public PlexmatchEpisodeNumberFormat? PreferEpisodeNumberFormat { get; set; }
}
