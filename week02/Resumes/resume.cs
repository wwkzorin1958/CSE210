using System;
using System.Collections.Generic;
using System.Text.Json;

public class Resume
{
    public string _name = "";
    public List<Job> _jobs = new List<Job>();   // initialized — never null

    public void Display()
    {
        Console.WriteLine($"Name: {_name}");           // print name once
        Console.WriteLine("Jobs:");

        if (_jobs.Count == 0)
        {
            Console.WriteLine("  (no jobs listed)");
            return;
        }

        foreach (Job job in _jobs)
        {
            job.Display();                       // Job.Display handles its own output
        }
    }

    public void AppendToCsv(string filePath)
    {
        bool fileHasContent = File.Exists(filePath) && new FileInfo(filePath).Length > 0;
        var lines = new List<string>();

        if (!fileHasContent)
        {
            lines.Add("Name,Job Title,Company,Start Year,End Year");
        }

        foreach (Job job in _jobs)
        {
            lines.Add(string.Join(",",
                EscapeCsv(_name),
                EscapeCsv(job._jobTitle),
                EscapeCsv(job._company),
                job._startYear,
                job._endYear));
        }

        if (_jobs.Count == 0)
        {
            lines.Add(EscapeCsv(_name) + ",,,,");
        }

        File.AppendAllLines(filePath, lines);
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') || value.Contains('"') ||
            value.Contains('\n') || value.Contains('\r'))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        return value;
    }
    public void AppendToJson(string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        };

        var resumes = new List<Resume>();

        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);

            if (!string.IsNullOrWhiteSpace(existingJson))
            {
                using JsonDocument document = JsonDocument.Parse(existingJson);

                if (document.RootElement.ValueKind == JsonValueKind.Array)
                {
                    List<Resume> existingResumes =
                        JsonSerializer.Deserialize<List<Resume>>(existingJson, options);

                    if (existingResumes == null)
                    {
                        throw new InvalidDataException("The existing JSON file does not contain a resume list.");
                    }

                    resumes.AddRange(existingResumes);
                }
                else if (document.RootElement.ValueKind == JsonValueKind.Object)
                {
                    Resume existingResume = JsonSerializer.Deserialize<Resume>(existingJson, options);

                    if (existingResume == null)
                    {
                        throw new InvalidDataException("The existing JSON file does not contain a resume.");
                    }

                    resumes.Add(existingResume);
                }
                else
                {
                    throw new InvalidDataException("The existing JSON must contain a resume object or an array of resumes.");
                }
            }
        }

        resumes.Add(this);
        string json = JsonSerializer.Serialize(resumes, options);
        File.WriteAllText(filePath, json);
    }
}