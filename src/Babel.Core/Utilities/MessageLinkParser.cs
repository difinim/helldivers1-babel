using System.Text.RegularExpressions;

namespace Babel.Core.Utilities;

public enum MessageSegmentType
{
    Text,
    Link
}

public record MessageSegment(MessageSegmentType Type, string Text, string? Url = null);

public static class MessageLinkParser
{
    /// <summary>
    /// Maximum length of message text allowed for URL parsing to prevent CPU/memory abuse.
    /// </summary>
    public const int MaxMessageLength = 4096;

    /// <summary>
    /// Maximum permitted length for an individual URL.
    /// </summary>
    public const int MaxUrlLength = 2048;

    // Strict URL regex: requires http:// or https://, stops at whitespace, quotes, and angle brackets.
    // Dangerous control characters and BiDi overrides are captured into candidate URL and rejected by IsSafeBrowserUrl.
    private static readonly Regex UrlRegex = new(
        @"(https?://[^\s<>""']+)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromSeconds(1));

    private static readonly char[] TrailingPunctuation = { '.', ',', ';', ':', '!', '?', ')', ']', '}', '>' };

    /// <summary>
    /// Checks whether a character is a dangerous control character, zero-width space, or BiDi directionality override.
    /// </summary>
    public static bool IsDangerousCharacter(char c)
    {
        // ASCII & Latin-1 control characters (including null \0)
        if (c < 32 || (c >= 127 && c <= 159)) return true;

        // Zero-width characters & BiDi controls used in Trojan Source / URL spoofing attacks
        if (c is >= '\u200B' and <= '\u200F') return true; // Zero-width spaces, LRM, RLM
        if (c is >= '\u202A' and <= '\u202E') return true; // BiDi embedding / override (LRE, RLE, PDF, LRO, RLO)
        if (c is >= '\u2066' and <= '\u2069') return true; // BiDi isolates (LRI, RLI, FSI, PDI)
        if (c == '\uFEFF') return true;                    // Zero-width no-break space / BOM

        return false;
    }

    /// <summary>
    /// Checks if a string contains any dangerous control characters or BiDi overrides.
    /// </summary>
    public static bool ContainsDangerousCharacters(string? s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        foreach (char c in s)
        {
            if (IsDangerousCharacter(c)) return true;
        }
        return false;
    }

    /// <summary>
    /// Strips null bytes, dangerous control characters, invalid surrogates, and BiDi overrides from text.
    /// Safe for rendering in WPF text blocks without risk of DirectWrite/WPF formatting engine crashes.
    /// </summary>
    public static string SanitizeText(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        if (text.Length > MaxMessageLength)
            text = text.Substring(0, MaxMessageLength);

        var sb = new System.Text.StringBuilder(text.Length);
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            // Allow standard formatting whitespace: newline, carriage return, tab
            if (c is '\r' or '\n' or '\t')
            {
                sb.Append(c);
                continue;
            }

            // Skip dangerous control and BiDi characters
            if (IsDangerousCharacter(c))
                continue;

            // Check for valid UTF-16 surrogate pairs
            if (char.IsSurrogate(c))
            {
                if (char.IsHighSurrogate(c) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]))
                {
                    sb.Append(c);
                    sb.Append(text[++i]);
                }
                // Skip isolated/unpaired surrogates to prevent encoding crashes
                continue;
            }

            sb.Append(c);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Validates whether a candidate URL string is 100% safe to parse as a clickable link and open in an external browser.
    /// </summary>
    public static bool IsSafeBrowserUrl(string? url, out Uri? uri, out string reason)
    {
        uri = null;

        if (string.IsNullOrWhiteSpace(url))
        {
            reason = "URL is empty or whitespace.";
            return false;
        }

        if (url.Length > MaxUrlLength)
        {
            reason = $"URL exceeds maximum allowed length of {MaxUrlLength} characters.";
            return false;
        }

        if (ContainsDangerousCharacters(url))
        {
            reason = "URL contains dangerous control or bidirectional characters.";
            return false;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
        {
            reason = "URL is not a valid absolute URI.";
            return false;
        }

        // Scheme MUST be strictly HTTP or HTTPS
        if (!uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) &&
            !uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            reason = $"URI scheme '{uri.Scheme}' is prohibited. Only HTTP and HTTPS are permitted.";
            return false;
        }

        // Reject embedded credentials (phishing attack: http://user:pass@evil.com or http://trusted.com@evil.com)
        if (!string.IsNullOrEmpty(uri.UserInfo))
        {
            reason = "URLs with embedded user credentials are not permitted.";
            return false;
        }

        // Host validation
        if (string.IsNullOrWhiteSpace(uri.Host) || !IsValidHost(uri.Host))
        {
            reason = "URL does not contain a valid DNS hostname or IP address.";
            return false;
        }

        // Ensure well-formed RFC 3986 format
        if (!Uri.IsWellFormedUriString(uri.AbsoluteUri, UriKind.Absolute))
        {
            reason = "URL is not a well-formed RFC 3986 URI.";
            return false;
        }

        // Reject shell metacharacters in AbsoluteUri for defense-in-depth against command injection
        string abs = uri.AbsoluteUri;
        if (abs.IndexOfAny(new[] { '"', '\'', '`', '^', '|', '\0', '\r', '\n' }) >= 0)
        {
            reason = "URL contains illegal shell or control characters.";
            return false;
        }

        reason = "OK";
        return true;
    }

    /// <summary>
    /// Validates that a host name is a legitimate DNS domain, localhost, or valid IP address.
    /// </summary>
    public static bool IsValidHost(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
            return false;

        var hostType = Uri.CheckHostName(host);
        if (hostType is UriHostNameType.IPv4 or UriHostNameType.IPv6)
            return true;

        if (hostType == UriHostNameType.Dns)
        {
            if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                return true;

            // DNS name must contain at least one dot, not at start or end (e.g. "example.com")
            int dotIndex = host.IndexOf('.');
            return dotIndex > 0 && dotIndex < host.Length - 1;
        }

        return false;
    }

    /// <summary>
    /// Parses a message string into a sequence of text and clickable link segments.
    /// Only safe http:// and https:// URLs are classified as links.
    /// </summary>
    public static IReadOnlyList<MessageSegment> Parse(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return Array.Empty<MessageSegment>();

        if (text.Length > MaxMessageLength)
            text = text.Substring(0, MaxMessageLength);

        var segments = new List<MessageSegment>();
        int lastIndex = 0;
        MatchCollection matches;
        try
        {
            matches = UrlRegex.Matches(text);
        }
        catch (RegexMatchTimeoutException)
        {
            return new[] { new MessageSegment(MessageSegmentType.Text, text) };
        }

        foreach (Match match in matches)
        {
            if (match.Index > lastIndex)
            {
                segments.Add(new MessageSegment(MessageSegmentType.Text, text.Substring(lastIndex, match.Index - lastIndex)));
            }

            string rawUrl = match.Value;
            string url = CleanTrailingPunctuation(rawUrl, out string trailing);

            if (IsSafeBrowserUrl(url, out _, out _))
            {
                segments.Add(new MessageSegment(MessageSegmentType.Link, url, url));
            }
            else
            {
                segments.Add(new MessageSegment(MessageSegmentType.Text, url));
            }

            if (!string.IsNullOrEmpty(trailing))
            {
                segments.Add(new MessageSegment(MessageSegmentType.Text, trailing));
            }

            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < text.Length)
        {
            segments.Add(new MessageSegment(MessageSegmentType.Text, text.Substring(lastIndex)));
        }

        return segments;
    }

    /// <summary>
    /// Strips trailing punctuation characters (e.g. '.', '!', ')') from a matched URL.
    /// </summary>
    public static string CleanTrailingPunctuation(string url, out string trailing)
    {
        trailing = string.Empty;
        if (string.IsNullOrEmpty(url))
            return string.Empty;

        int trimCount = 0;
        for (int i = url.Length - 1; i >= 0; i--)
        {
            if (Array.IndexOf(TrailingPunctuation, url[i]) >= 0)
            {
                trimCount++;
            }
            else
            {
                break;
            }
        }

        if (trimCount > 0)
        {
            trailing = url.Substring(url.Length - trimCount);
            return url.Substring(0, url.Length - trimCount);
        }

        return url;
    }
}
