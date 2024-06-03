using PlexmatchEditor.Plexmatch;

namespace PlexmatchEditor.Tests.Plexmatch;

[TestClass]
public class PlexmatchEpisodeRangeTests
{
    [TestMethod]
    public void TestTryParse()
    {
        // test cases from https://support.plex.tv/articles/plexmatch/

        Assert.AreEqual(
            new PlexmatchEpisodeRange(new PlexmatchEpisodeIndex(4, 0), default) 
            { 
                EpisodeNumberFormat = PlexmatchEpisodeNumberFormat.SP
            }, 
            PlexmatchEpisodeRange.TryParse("SP04".AsMemory()));

        Assert.AreEqual(
            new PlexmatchEpisodeRange(new PlexmatchEpisodeIndex(2, default), default)
            {
                EpisodeNumberFormat = PlexmatchEpisodeNumberFormat.EpisodeOnly
            },
            PlexmatchEpisodeRange.TryParse("E02".AsMemory()));

        Assert.AreEqual(
            new PlexmatchEpisodeRange(new PlexmatchEpisodeIndex(2, default), default)
            {
                EpisodeNumberFormat = PlexmatchEpisodeNumberFormat.NumberOnly
            },
            PlexmatchEpisodeRange.TryParse("02".AsMemory()));

        Assert.AreEqual(
            new PlexmatchEpisodeRange(new PlexmatchEpisodeIndex(4, 3), default)
            {
                EpisodeNumberFormat = PlexmatchEpisodeNumberFormat.SeasonAndEpisode
            },
            PlexmatchEpisodeRange.TryParse("S03E04".AsMemory()));

        Assert.AreEqual(
            new PlexmatchEpisodeRange(new PlexmatchEpisodeIndex(1, 1), new PlexmatchEpisodeIndex(2, default))
            {
                EpisodeNumberFormat = PlexmatchEpisodeNumberFormat.SeasonAndEpisode
            },
            PlexmatchEpisodeRange.TryParse("S01E01-E02".AsMemory()));

        Assert.AreEqual(
            new PlexmatchEpisodeRange(new PlexmatchEpisodeIndex(1, default), new PlexmatchEpisodeIndex(2, default))
            {
                EpisodeNumberFormat = PlexmatchEpisodeNumberFormat.NumberOnly
            },
            PlexmatchEpisodeRange.TryParse("01-02".AsMemory()));

        Assert.AreEqual(
            new PlexmatchEpisodeRange(new PlexmatchEpisodeIndex(12, 3), new PlexmatchEpisodeIndex(13, 3))
            {
                EpisodeNumberFormat = PlexmatchEpisodeNumberFormat.ExplicitSeasonAndEpisode
            },
            PlexmatchEpisodeRange.TryParse("S03E12-S03E13".AsMemory()));
    }

    [TestMethod]
    public void TestToString()
    {
        // test cases from https://support.plex.tv/articles/plexmatch/

        Assert.AreEqual(
            "SP04",
            PlexmatchEpisodeRange.TryParse("SP04".AsMemory())!.Value.ToString(default, default));

        Assert.AreEqual(
            "E02",
            PlexmatchEpisodeRange.TryParse("E02".AsMemory())!.Value.ToString(default, default));

        Assert.AreEqual(
            "02",
            PlexmatchEpisodeRange.TryParse("02".AsMemory())!.Value.ToString(default, default));

        Assert.AreEqual(
            "S03E04",
            PlexmatchEpisodeRange.TryParse("S03E04".AsMemory())!.Value.ToString(default, default));

        Assert.AreEqual(
            "S01E01-E02",
            PlexmatchEpisodeRange.TryParse("S01E01-E02".AsMemory())!.Value.ToString(default, default));

        Assert.AreEqual(
            "01-02",
            PlexmatchEpisodeRange.TryParse("01-02".AsMemory())!.Value.ToString(default, default));

        Assert.AreEqual(
            "S03E12-S03E13",
            PlexmatchEpisodeRange.TryParse("S03E12-S03E13".AsMemory())!.Value.ToString(default, default));
    }
}