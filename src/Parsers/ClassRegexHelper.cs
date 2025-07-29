using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ClassHide.Parsers;
internal class ClassRegexHelper
{
    // To get the match value, get capture group 'content'
    // https://regex101.com/r/Odcyjx/3
    private static readonly Regex _classRegex = new(@"[cC]lass\s*=\s*(['""])\s*(?<content>(?:\n.|(?!\1).)*)?\s*(\1|$)", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly Regex _javaScriptClassRegex = new(@"[cC]lassName\s*=\s*(['""])\s*(?<content>(?:\n.|(?!\1).)*)?\s*(\1|$)", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly Regex _razorClassRegex = new(@"[cC]lass(?:es)?\s*=\s*([""'])(?<content>(?:[^""'\\@]|\\.|@(?:[a-zA-Z0-9.]+)?\((?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!))\)|(?:(?!\1)[^\\]|\\$|\\.)|\([^)]*\))*)(\1|$)", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    // For use on the content capture group of class regexes. No match if there are no single quote pairs.
    // For example, in open ? 'hi @('h')' : '@(Model.Name)', the matches would be 'hi @('h')' and '@(Model.Name)'
    private static readonly Regex _razorQuotePairRegex = new(@"(?<!@\((?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!)))'(?<content>(?:@\((?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!))\)|(?:(?!')[^\\]|\\.))*)'", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly Regex _normalQuotePairRegex = new(@"'(?<content>(?:[^'\\]|\\.)*)'", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    /// <summary>
    /// Gets all class matches in a razor context.
    /// Includes: class="...". This specific method uses a yield return method.
    /// </summary>
    /// <remarks>
    /// This method does not guarantee that matches are ordered sequentially.
    /// </remarks>
    public static IEnumerable<Group> GetClassesRazorEnumerator(string text)
    {
        var lastMatchIndex = 0;

        while (_razorClassRegex.Match(text, lastMatchIndex) is Match match && match.Success)
        {
            Group content = GetClassTextGroup(match);
            lastMatchIndex = content.Index + content.Length;

            var classText = content.Value;

            if (_razorQuotePairRegex.IsMatch(classText))
            {
                var lastQuoteMatchIndex = match.Index;

                while (_razorQuotePairRegex.Match(text, lastQuoteMatchIndex, Math.Min(text.Length - lastQuoteMatchIndex, match.Length)) is Match quoteMatch && quoteMatch.Success)
                {
                    Group quoteContent = GetClassTextGroup(quoteMatch);

                    lastQuoteMatchIndex = quoteContent.Index + quoteContent.Length;
                    yield return quoteContent;
                }
                continue;
            }

            yield return content;
        }
    }

    /// <summary>
    /// Gets all class matches in a normal context.
    /// Includes: class="...". This specific method uses a yield return method.
    /// </summary>
    /// <remarks>
    /// This method does not guarantee that matches are ordered sequentially.
    /// </remarks>
    public static IEnumerable<Group> GetClassesNormalEnumerator(string text)
    {
        var lastMatchIndex = 0;

        while (_classRegex.Match(text, lastMatchIndex) is Match match && match.Success)
        {
            Group content = GetClassTextGroup(match);
            lastMatchIndex = content.Index + content.Length;

            var classText = content.Value;

            if (_normalQuotePairRegex.IsMatch(classText))
            {
                var lastQuoteMatchIndex = match.Index;

                while (_normalQuotePairRegex.Match(text, lastQuoteMatchIndex, Math.Min(text.Length - lastQuoteMatchIndex, match.Length)) is Match quoteMatch && quoteMatch.Success)
                {
                    Group quoteContent = GetClassTextGroup(quoteMatch);

                    lastQuoteMatchIndex = quoteContent.Index + quoteContent.Length;
                    yield return quoteContent;
                }
                continue;
            }

            yield return content;
        }
    }

    /// <summary>
    /// Gets all class matches in a normal context.
    /// Includes: className="...". This specific method uses a yield return method.
    /// </summary>
    /// <remarks>
    /// This method does not guarantee that matches are ordered sequentially.
    /// </remarks>
    public static IEnumerable<Group> GetClassesJavaScriptEnumerator(string text)
    {
        var lastMatchIndex = 0;

        while (_javaScriptClassRegex.Match(text, lastMatchIndex) is Match match && match.Success)
        {
            Group content = GetClassTextGroup(match);
            lastMatchIndex = content.Index + content.Length;

            var classText = content.Value;

            if (_normalQuotePairRegex.IsMatch(classText))
            {
                var lastQuoteMatchIndex = match.Index;

                while (_normalQuotePairRegex.Match(text, lastQuoteMatchIndex, Math.Min(text.Length - lastQuoteMatchIndex, match.Length)) is Match quoteMatch && quoteMatch.Success)
                {
                    Group quoteContent = GetClassTextGroup(quoteMatch);

                    lastQuoteMatchIndex = quoteContent.Index + quoteContent.Length;
                    yield return quoteContent;
                }
                continue;
            }

            yield return content;
        }
    }

    public static Group GetClassTextGroup(Match match)
    {
        // 'content' capture group matches the class value
        return match.Groups["content"];
    }
}
