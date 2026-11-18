using System;

class Program
{
    static void Main(string[] args)
    {
        var myResume = new Resume();
        myResume._name = ReadText("Person's name: ");

        int jobCount = ReadInt("Number of previous jobs: ");
        for (int i = 0; i < jobCount; i++)
        {
            Console.WriteLine($"Job {i + 1}");
            var job = new Job
            {
                _jobTitle = ReadText("  Job title: "),
                _company = ReadText("  Company: "),
                _startYear = ReadInt("  Start year: "),
                _endYear = ReadInt("  End year: ")
            };
            myResume._jobs.Add(job);
        }

        myResume.Display();
        myResume.AppendToCsv("resume.csv");
        myResume.AppendToJson("resume.json");
    }

    private static string ReadText(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value >= 0)
            {
                return value;
            }

            Console.WriteLine("Enter a non-negative whole number.");
        }
    }
}