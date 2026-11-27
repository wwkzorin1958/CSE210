using System;
using System.Threading.Tasks.Dataflow;
using System.Xml.Serialization;

class Program
{
    static void Main(string[] args)
    {
        string selection = "";
        do
        {
            Console.Clear();
            Console.WriteLine("Menu Options:");
            Console.WriteLine("1. Start breathing activity");
            Console.WriteLine("2. Start reflecting activity");
            Console.WriteLine("3. Start listing activity");
            Console.WriteLine("4. Review activity history");
            Console.WriteLine("5. Quit");
            Console.Write("Select a choice from the menu: ");
            selection = Console.ReadLine();
            switch (selection)
            {
                case "1":
                    BreathingActivity breathing = new BreathingActivity(
                    "Breathing",
                    "This activity will help you relax by breathing slowly.",
                    30
                    );
                    breathing.Run();
                    break;
                case "2":
                    ReflectingActivity reflecting = new ReflectingActivity(
                        "Reflecting",
                        "This activity will help you reflect on times...",
                        50
                        );
                    reflecting.Run();
                    break;
                case "3":
                    ListingActivity listing = new ListingActivity(
                        "Listing",
                        "This activity will help you reflect on the good things in your life by having you list as many things as you can in a certain area.",
                        50,
                        0
                        );
                    listing.Run();
                    break;
                case "4":
                    DisplayHistory();
                    break;
                case "5":
                    Console.WriteLine("See you next time, Have a nice sleep!!");
                    break;
                default:
                    Console.WriteLine("Invalid Choice.");
                    break;
            }

        } while (selection != "5");


    }

    static void DisplayHistory()
    {
        ActivityHistory history = new ActivityHistory();
        Console.WriteLine($"\nActivity history: {history.PathName}");
        List<ActivityRecord> records = history.Load();
        if (records.Count == 0)
        {
            Console.WriteLine("No completed activities have been saved yet.");
        }

        foreach (ActivityRecord record in records)
        {
            Console.WriteLine($"\n{record.StartedAt.ToLocalTime():yyyy-MM-dd HH:mm:ss zzz} - {record.ActivityName}");
            Console.WriteLine($"Requested: {record.RequestedSeconds} seconds; elapsed: {record.ElapsedSeconds:F1} seconds");
            foreach (ActivityResponse response in record.Responses)
            {
                Console.WriteLine($"  {response.Prompt}");
                if (response.Answers.Count == 0)
                {
                    Console.WriteLine("    (No response recorded)");
                }

                foreach (string answer in response.Answers)
                {
                    Console.WriteLine($"    - {(string.IsNullOrWhiteSpace(answer) ? "(No response recorded)" : answer)}");
                }
            }
        }

        Console.Write("\nPress Enter to return to the menu. ");
        Console.ReadLine();
    }
}