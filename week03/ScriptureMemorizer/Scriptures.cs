using System;
using System.Collections.Generic;

public class Scripture
{
    private Reference _reference;
    private List<Word> _words;
    private Random _random = new Random();

    public Scripture(Reference reference, string text)
    {
        _reference = reference;
        _words = new List<Word>();

        // Split the text into words and wrap each in a Word object
        string[] parts = text.Split(' ');
        foreach (string part in parts)
        {
            _words.Add(new Word(part));
        }
    }

    public void HideRandomWords(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Build a list of only the words that are still visible
            List<Word> visible = new List<Word>();
            foreach (Word word in _words)
            {
                if (!word.IsHidden())
                {
                    visible.Add(word);
                }
            }

            // If nothing is visible, stop (everything is hidden)
            if (visible.Count == 0)
            {
                return;
            }

            // Pick one visible word at random and hide it
            int index = _random.Next(visible.Count);
            visible[index].Hide();
        }
    }

    public bool AllWordsHidden()
    {
        foreach (Word word in _words)
        {
            if (!word.IsHidden())
            {
                return false;   // found a visible word → not all hidden
            }
        }
        return true;            // no visible words → all hidden
    }

    public string GetDisplayText()
    {
        // Build the words portion by joining each word's display text
        List<string> displayParts = new List<string>();
        foreach (Word word in _words)
        {
            displayParts.Add(word.GetDisplayText());
        }
        string wordsText = string.Join(" ", displayParts);

        return $"{_reference.GetDisplayText()}\n{wordsText}";
    }
}