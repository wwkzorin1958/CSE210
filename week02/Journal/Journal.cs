using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class Journal
{
    private List<Entry> _entries = new List<Entry>();
    private PromptGenerator _promptGenerator = new PromptGenerator();

    
    public void Write()
    {
        Entry entry = new Entry();
        entry.Date = DateTime.Now.ToShortDateString();
        
        // Pick a random prompt
        entry.Prompt = _promptGenerator.GetRandomPrompt();

        Console.WriteLine(entry.Prompt);
        entry.Response = Console.ReadLine();
        _entries.Add(entry);
    }

    public void Display()
    {
        // foreach (Entry entry in _entries)
        // {
        //     Console.WriteLine($"{entry._date} - {entry._prompt}");
        //     Console.WriteLine(entry._response);
        //     Console.WriteLine();
        // }

        if (_entries.Count == 0)
        {
            Console.WriteLine("No entries to display.");
            return;
        }

        foreach (Entry entry in _entries)
        {
            // each entry prints itself
            entry.Display();   
        }
    }

    public void Load()   
    { 
        /* read file into _entries */ 
        Console.Write("What is the filename? ");
        string fileName = Console.ReadLine();
        if (!File.Exists(fileName))
        {
            Console.WriteLine($"File not found: {fileName}");
            return;
        }
        
        Console.WriteLine("Reading list from file...");

        string jsonString = File.ReadAllText(fileName);
        _entries.Clear();
        
        // Deserialize JSON string back to List<Entry>
        _entries = JsonSerializer.Deserialize<List<Entry>>(jsonString) ?? new List<Entry>();
        
        Console.WriteLine($"Loaded {_entries.Count} entries from JSON.");


        // string[] lines = System.IO.File.ReadAllLines(fileName);

        // foreach (string line in lines)
        // {
        //     string[] parts = line.Split("~~");
        //     if (parts.Length < 3) continue; 
        //     Entry newEntry = new Entry();
        //     newEntry._date = parts[0];
        //     newEntry._prompt = parts[1];
        //     newEntry._response = parts[2];

        //     _entries.Add(newEntry);
        // }
        // Console.WriteLine($"Loaded {_entries.Count} entries.");
    }
    public void Save()   
    { 
        /* write _entries → file */ 
        Console.Write("What is the filename? ");
        string fileName = Console.ReadLine();
        Console.WriteLine("Saving to file...");

        // Serialize List<Entry> to JSON string (formatted for readability)
        string jsonString = JsonSerializer.Serialize(_entries, new JsonSerializerOptions { WriteIndented = true });
        
        File.WriteAllText(fileName, jsonString);
        Console.WriteLine($"Saved {_entries.Count} entries to JSON.");
        
        // using(StreamWriter outputFile = new StreamWriter(fileName))
        // foreach (Entry entry in _entries)
        // {
        //     outputFile.WriteLine($"{entry._date}~~{entry._prompt}~~{entry._response}");
        // }

    }
}