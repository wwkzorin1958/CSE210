using System;
using System.IO;

class Program
{
    private static readonly string ProjectDirectory = FindProjectDirectory();

    static void Main(string[] args)
    {
        Console.WriteLine("Console app is ready.");
        Console.WriteLine($"Project directory: {ProjectDirectory}");
        Console.WriteLine($"Example data path: {GetDataFilePath("data.txt")}");
    }

    private static string GetDataFilePath(string fileName)
    {
        if (Path.GetFileName(fileName) != fileName)
        {
            throw new ArgumentException("Provide a file name without a directory path.", nameof(fileName));
        }

        return Path.Combine(ProjectDirectory, fileName);
    }

    private static string FindProjectDirectory()
    {
        DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "week04.csproj")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return AppContext.BaseDirectory;
    }
}
