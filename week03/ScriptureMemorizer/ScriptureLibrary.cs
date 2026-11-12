using System;
using System.Collections.Generic;
using System.IO;

public class ScriptureLibrary
{
    private List<Scripture> _scriptures = new List<Scripture>();
    private List<Scripture> _unused;
    private Random _random = new Random();

    public ScriptureLibrary(string fileName)
    {
        string[] lines = File.ReadAllLines(fileName);

        foreach (string line in lines)
        {
            if (line.Trim().Length == 0) continue;   // skip blank lines

            // The reference uses a chapter:verse form, and the scripture text starts
            // after the verse separator. Split at the second colon in the line.
            int firstColon = line.IndexOf(':');
            if (firstColon < 0)
            {
                continue; // skip malformed lines
            }

            int secondColon = line.IndexOf(':', firstColon + 1);
            if (secondColon < 0)
            {
                continue; // skip malformed lines
            }

            string referenceText = line.Substring(0, secondColon);
            string scriptureText = line.Substring(secondColon + 1).Trim(' ', '“', '”', '"');

            Reference reference = Reference.Parse(referenceText);
            _scriptures.Add(new Scripture(reference, scriptureText));
        }

        _unused = new List<Scripture>(_scriptures);
        Console.WriteLine($"Loaded {_scriptures.Count} scriptures.");
    }

    public bool IsEmpty()
    {
        return _scriptures.Count == 0;
    }

    // Pick a random scripture that hasn't been used yet.
    // When all are used, start over.
    public Scripture GetRandom()
    {
        if (_unused.Count == 0)
        {
            _unused = new List<Scripture>(_scriptures);   // refill
        }

        int index = _random.Next(_unused.Count);
        Scripture chosen = _unused[index];
        _unused.RemoveAt(index);
        return chosen;
    }
}