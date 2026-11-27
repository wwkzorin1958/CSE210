using System.Text.Json;

public record ActivityResponse(string Prompt, List<string> Answers);

public record ActivityRecord(
    string ActivityName,
    DateTimeOffset StartedAt,
    int RequestedSeconds,
    double ElapsedSeconds,
    List<ActivityResponse> Responses);

public class ActivityHistory
{
    private readonly string _path;

    public ActivityHistory() : this(GetDefaultPath())
    {
    }

    private static string GetDefaultPath()
    {
        DirectoryInfo directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Mindfulness.csproj")))
            {
                return Path.Combine(directory.FullName, "activity-history.jsonl");
            }

            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, "activity-history.jsonl");
    }

    public ActivityHistory(string path)
    {
        _path = Path.GetFullPath(path);
    }

    public string PathName => _path;

    public void Save(ActivityRecord record)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path));
        File.AppendAllText(_path, JsonSerializer.Serialize(record) + Environment.NewLine);
    }

    public List<ActivityRecord> Load()
    {
        List<ActivityRecord> records = new List<ActivityRecord>();
        if (!File.Exists(_path))
        {
            return records;
        }

        int lineNumber = 0;
        foreach (string line in File.ReadLines(_path))
        {
            lineNumber++;
            try
            {
                ActivityRecord record = JsonSerializer.Deserialize<ActivityRecord>(line);
                if (record == null || record.ActivityName == null || record.Responses == null ||
                    record.Responses.Any(response => response == null || response.Prompt == null ||
                        response.Answers == null || response.Answers.Any(answer => answer == null)))
                {
                    throw new InvalidDataException($"Invalid activity history at line {lineNumber}.");
                }

                records.Add(record);
            }
            catch (JsonException exception)
            {
                throw new InvalidDataException($"Invalid activity history at line {lineNumber}.", exception);
            }
        }

        return records;
    }
}
