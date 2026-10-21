using Babel.Core.Utilities;

namespace Babel.Core.Tests;

public class MessageLinkParserTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Parse_WhenNullOrEmpty_ReturnsEmptyList(string? input)
    {
        var segments = MessageLinkParser.Parse(input);
        Assert.Empty(segments);
    }

    [Fact]
    public void Parse_WhenNoLinks_ReturnsSingleTextSegment()
    {
        const string text = "Affirmative, moving to extraction point!";
        var segments = MessageLinkParser.Parse(text);

        Assert.Single(segments);
        Assert.Equal(MessageSegmentType.Text, segments[0].Type);
        Assert.Equal(text, segments[0].Text);
        Assert.Null(segments[0].Url);
    }

    [Theory]
    [InlineData("http://example.com")]
    [InlineData("https://discord.gg/helldivers")]
    [InlineData("https://github.com/difinim/helldivers1-babel")]
    public void Parse_WhenOnlyUrl_ReturnsSingleLinkSegment(string url)
    {
        var segments = MessageLinkParser.Parse(url);

        Assert.Single(segments);
        Assert.Equal(MessageSegmentType.Link, segments[0].Type);
        Assert.Equal(url, segments[0].Text);
        Assert.Equal(url, segments[0].Url);
    }

    [Fact]
    public void Parse_WhenUrlEmbeddedInText_ReturnsOrderedSegments()
    {
        const string text = "Check our Discord https://discord.gg/helldivers for squad tactics";
        var segments = MessageLinkParser.Parse(text);

        Assert.Equal(3, segments.Count);

        Assert.Equal(MessageSegmentType.Text, segments[0].Type);
        Assert.Equal("Check our Discord ", segments[0].Text);

        Assert.Equal(MessageSegmentType.Link, segments[1].Type);
        Assert.Equal("https://discord.gg/helldivers", segments[1].Text);
        Assert.Equal("https://discord.gg/helldivers", segments[1].Url);

        Assert.Equal(MessageSegmentType.Text, segments[2].Type);
        Assert.Equal(" for squad tactics", segments[2].Text);
    }

    [Theory]
    [InlineData("Visit https://example.com.", "https://example.com", ".")]
    [InlineData("See https://example.com/guide!", "https://example.com/guide", "!")]
    [InlineData("Did you read https://example.com/faq?", "https://example.com/faq", "?")]
    [InlineData("Tactics (https://example.com/tac)", "https://example.com/tac", ")")]
    [InlineData("Check https://example.com/test...", "https://example.com/test", "...")]
    public void Parse_WhenUrlHasTrailingPunctuation_SeparatesPunctuationFromUrl(string input, string expectedUrl, string expectedTrailing)
    {
        var segments = MessageLinkParser.Parse(input);

        var link = segments.FirstOrDefault(s => s.Type == MessageSegmentType.Link);
        Assert.NotNull(link);
        Assert.Equal(expectedUrl, link.Url);
        Assert.Equal(expectedUrl, link.Text);

        var trailing = segments.Last();
        Assert.Equal(MessageSegmentType.Text, trailing.Type);
        Assert.EndsWith(expectedTrailing, trailing.Text);
    }

    [Fact]
    public void Parse_WhenMultipleUrlsPresent_ExtractsAllLinks()
    {
        const string text = "Check https://site-a.org and http://site-b.net for details";
        var segments = MessageLinkParser.Parse(text);

        Assert.Equal(5, segments.Count);

        Assert.Equal(MessageSegmentType.Text, segments[0].Type);
        Assert.Equal("Check ", segments[0].Text);

        Assert.Equal(MessageSegmentType.Link, segments[1].Type);
        Assert.Equal("https://site-a.org", segments[1].Url);

        Assert.Equal(MessageSegmentType.Text, segments[2].Type);
        Assert.Equal(" and ", segments[2].Text);

        Assert.Equal(MessageSegmentType.Link, segments[3].Type);
        Assert.Equal("http://site-b.net", segments[3].Url);

        Assert.Equal(MessageSegmentType.Text, segments[4].Type);
        Assert.Equal(" for details", segments[4].Text);
    }

    [Theory]
    [InlineData("file:///C:/secret/passwords.txt")]
    [InlineData("ftp://ftp.example.com/file.zip")]
    [InlineData("javascript:alert(1)")]
    public void Parse_WhenNonHttpOrHttps_TreatsAsPlainText(string text)
    {
        var segments = MessageLinkParser.Parse(text);

        Assert.All(segments, s => Assert.Equal(MessageSegmentType.Text, s.Type));
        Assert.DoesNotContain(segments, s => s.Type == MessageSegmentType.Link);
    }

    [Fact]
    public void CleanTrailingPunctuation_EdgeCases()
    {
        var url1 = MessageLinkParser.CleanTrailingPunctuation("https://example.com", out var t1);
        Assert.Equal("https://example.com", url1);
        Assert.Equal(string.Empty, t1);

        var url2 = MessageLinkParser.CleanTrailingPunctuation("https://example.com/path!?)]", out var t2);
        Assert.Equal("https://example.com/path", url2);
        Assert.Equal("!?)]", t2);

        var url3 = MessageLinkParser.CleanTrailingPunctuation("", out var t3);
        Assert.Equal(string.Empty, url3);
        Assert.Equal(string.Empty, t3);
    }

    [Theory]
    [InlineData("https://example.com\0evil.com")]
    [InlineData("https://example.com\x01test")]
    [InlineData("https://example.com\x1Ftest")]
    [InlineData("https://example.com\x7Ftest")]
    public void Parse_WhenContainsNullByteOrControlChars_DoesNotParseAsLink(string maliciousUrl)
    {
        var segments = MessageLinkParser.Parse(maliciousUrl);
        Assert.DoesNotContain(segments, s => s.Type == MessageSegmentType.Link);
    }

    [Theory]
    [InlineData("https://example.com\u202Emoc.evil//:sptth")] // Right-to-Left Override (RLO)
    [InlineData("https://example.com\u200Btest")]              // Zero-width space
    [InlineData("https://example.com\u202Atest")]              // LRE
    [InlineData("https://example.com\u2066test")]              // LRI
    [InlineData("https://example.com\uFEFFtest")]              // BOM
    public void Parse_WhenContainsBiDiOrZeroWidthChars_DoesNotParseAsLink(string spoofedUrl)
    {
        var segments = MessageLinkParser.Parse(spoofedUrl);
        Assert.DoesNotContain(segments, s => s.Type == MessageSegmentType.Link);
    }

    [Theory]
    [InlineData("https://user:password@evil.com/login")]
    [InlineData("http://google.com@evil.com/test")]
    public void Parse_WhenContainsUserCredentials_DoesNotParseAsLink(string credentialUrl)
    {
        var segments = MessageLinkParser.Parse(credentialUrl);
        Assert.DoesNotContain(segments, s => s.Type == MessageSegmentType.Link);
    }

    [Theory]
    [InlineData("https://...")]
    [InlineData("https://???")]
    [InlineData("https://-")]
    [InlineData("https://nodots")]
    public void Parse_WhenInvalidHost_DoesNotParseAsLink(string invalidHostUrl)
    {
        var segments = MessageLinkParser.Parse(invalidHostUrl);
        Assert.DoesNotContain(segments, s => s.Type == MessageSegmentType.Link);
    }

    [Fact]
    public void Parse_WhenHugeInput_DoesNotThrowAndCapsGracefully()
    {
        string hugeString = new string('A', 100000);
        var segments = MessageLinkParser.Parse(hugeString);

        Assert.Single(segments);
        Assert.Equal(MessageSegmentType.Text, segments[0].Type);
        Assert.True(segments[0].Text.Length <= MessageLinkParser.MaxMessageLength);
    }

    [Fact]
    public void SanitizeText_StripsDangerousCharsAndPreservesSafeFormatting()
    {
        string input = "Hello\0 World!\u202E\tTest\nLine";
        string sanitized = MessageLinkParser.SanitizeText(input);

        Assert.DoesNotContain('\0', sanitized);
        Assert.DoesNotContain('\u202E', sanitized);
        Assert.Contains("\t", sanitized);
        Assert.Contains("\n", sanitized);
        Assert.Equal("Hello World!\tTest\nLine", sanitized);
    }

    [Theory]
    [InlineData("https://example.com", true)]
    [InlineData("http://localhost:8080/test", true)]
    [InlineData("http://192.168.1.1/admin", true)]
    [InlineData("ftp://example.com", false)]
    [InlineData("javascript:alert(1)", false)]
    [InlineData("https://example.com\" --cmd", false)]
    [InlineData("https://user:pass@example.com", false)]
    public void IsSafeBrowserUrl_ValidatesCorrectly(string url, bool expectedSafe)
    {
        bool isSafe = MessageLinkParser.IsSafeBrowserUrl(url, out var uri, out _);
        Assert.Equal(expectedSafe, isSafe);
        if (expectedSafe)
        {
            Assert.NotNull(uri);
        }
    }
}
