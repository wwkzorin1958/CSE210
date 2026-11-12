using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Reference
{
    private string _book;
    private int _chapter;
    private int _startVerse;
    private int _endVerse;

    public Reference(string book, int chapter, int verse)
    {
        _book = book;
        _chapter = chapter;
        _startVerse = verse;
        _endVerse = verse;
    }

    public Reference(string book, int chapter, int startVerse, int endVerse)
    {
        _book = book;
        _chapter = chapter;
        _startVerse = startVerse;
        _endVerse = endVerse;
    }

    public string GetDisplayText()
    {
        if (_startVerse == _endVerse)
            return $"{_book} {_chapter}:{_startVerse}";
        return $"{_book} {_chapter}:{_startVerse}-{_endVerse}";
    }

    // "Moses 1:39"  or  "Abraham 2:9-11"  or  "Doctrine and Covenants 121:36, 41-42"
    public static Reference Parse(string text)
    {
        string trimmedText = text.Trim();
        int colon = trimmedText.IndexOf(':');
        if (colon < 0)
        {
            throw new FormatException($"Invalid scripture reference format: '{text}'");
        }

        string beforeColon = trimmedText.Substring(0, colon);      // "Moses 1"
        string afterColon  = trimmedText.Substring(colon + 1);     // "39" or "9-11" or "36, 41-42"

        int space = beforeColon.LastIndexOf(' ');
        if (space < 0)
        {
            throw new FormatException($"Invalid scripture reference format: '{text}'");
        }

        string book = beforeColon.Substring(0, space);
        int chapter = int.Parse(beforeColon.Substring(space + 1));

        string verses = afterColon.Replace('–', '-').Replace('—', '-');
        MatchCollection matches = Regex.Matches(verses, @"\d+");
        if (matches.Count == 0)
        {
            throw new FormatException($"Invalid scripture reference format: '{text}'");
        }

        int startVerse = int.Parse(matches[0].Value);
        int endVerse = int.Parse(matches[matches.Count - 1].Value);
        return new Reference(book, chapter, startVerse, endVerse);
    }
}