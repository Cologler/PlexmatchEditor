namespace PlexmatchEditor.Plexmatch;

public enum PlexmatchEpisodeNumberFormat
{
    /// <summary>
    /// e.g. "SP01"
    /// </summary>
    SP,

    /// <summary>
    /// e.g. "12"
    /// </summary>
    NumberOnly,

    /// <summary>
    /// e.g. "E12"
    /// </summary>
    EpisodeOnly,

    /// <summary>
    /// e.g. "S03E12-E13"
    /// </summary>
    SeasonAndEpisode,

    /// <summary>
    /// Include ending season even at the same season. 
    /// e.g. "S03E12-S03E13"
    /// </summary>
    ExplicitSeasonAndEpisode,
}
