using System;
using System.Collections.Generic;
using System.IO;

class Journal
{
    private List<Entry> _entries = new List<Entry>();
    private Random _random = new Random();

    private List<string> _prompts = new List<string>
    {
        "Who was the most interesting person I interacted with today?",
        "What was the best part of your day?",
        "How did I see the hand of the Lord in my life today?",
        "What was the strongest emotion I felt today?",
        "If I had one thing I could do over today, what would it be?",
        "What was the most challenging thing today?",
        "Who did you help today, and how?",
        "What are you grateful for right now?",
        "What did you learn today?",
        "What made you smile today?",
        "What would you do differently today?",
        "Describe a moment that stood out to you."
    };

    public void Write()
    {
        Entry entry = new Entry();
        entry._date = DateTime.Now.ToShortDateString();
        
        // Pick a random prompt
        int index = _random.Next(_prompts.Count);
        entry._prompt = _prompts[index];

        Console.WriteLine(entry._prompt);
        entry._response = Console.ReadLine();
        _entries.Add(entry);
    }

    public void Display()
    {
        foreach (Entry entry in _entries)
        {
            Console.WriteLine($"{entry._date} - {entry._prompt}");
            Console.WriteLine(entry._response);
            Console.WriteLine();
        }
    }

    public void Load()   
    { /* read file → _entries */ 
        Console.Write("What is the filename? ");
        string fileName = Console.ReadLine();
        if (!File.Exists(fileName))
        {
            Console.WriteLine($"File not found: {fileName}");
            return;
        }
        
        Console.WriteLine("Reading list from file...");
     
        _entries.Clear();

        string[] lines = System.IO.File.ReadAllLines(fileName);

        foreach (string line in lines)
        {
            string[] parts = line.Split("~~");
            if (parts.Length < 3) continue; 
            Entry newEntry = new Entry();
            newEntry._date = parts[0];
            newEntry._prompt = parts[1];
            newEntry._response = parts[2];

            _entries.Add(newEntry);
        }
        Console.WriteLine($"Loaded {_entries.Count} entries.");
    }
    public void Save()   
    { 
        /* write _entries → file */ 
        Console.Write("What is the filename? ");
        string fileName = Console.ReadLine();
        Console.WriteLine("Saving to file...");
        
        using(StreamWriter outputFile = new StreamWriter(fileName))
        foreach (Entry entry in _entries)
        {
            outputFile.WriteLine($"{entry._date}~~{entry._prompt}~~{entry._response}");
        }

    }
}