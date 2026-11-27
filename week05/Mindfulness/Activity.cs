using System;
using System.Media;
using System.Text;

public class Activity
{
    private string _name;
    private string _description;
    private int _duration;

    public Activity(string name, string description, int duration)
    {
        _name = name;
        _description = description;
        _duration = duration;
    }

    public string GetName()
    {
        return _name;
    }
    public void SetName(string name)
    {
        _name = name;
    }

    public string GetDescription()
    {
        return _description;
    }
    public void SetDescription(string description)
    {
        _description = description;
    }

    public int GetDuration()
    {
        return _duration;
    }

    public void SetDuration(int duration)
    {
        _duration = duration;
    }


    public void DisplayStartingMessage()
    {
        Console.WriteLine($"Welcome to the {_name} Activity.");
        Console.WriteLine(_description);
        int seconds;
        while (true)
        {
            Console.Write("How long, in seconds, would you like for your session? ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out seconds) && seconds > 0)
            {
                break;
            }

            Console.WriteLine("Please enter a positive number of seconds.");
        }

        _duration = seconds;
        WaitAMoment();
    }
    public void DisplayEndingMessage()
    {
        Console.WriteLine("Well done!!\n");
        Console.WriteLine($"You have completed another {_duration} seconds of the {_name} Activity.");
        ShowSpinner(3);
    }
    public void ShowSpinner(int seconds)
    {
        List<string> animationStrings = new List<string>();
        animationStrings.Add("|");
        animationStrings.Add("/");
        animationStrings.Add("-");
        animationStrings.Add("\\");
        animationStrings.Add("|");
        animationStrings.Add("/");
        animationStrings.Add("-");
        animationStrings.Add("\\");

        DateTime startTime = DateTime.Now;
        DateTime endTime = startTime.AddSeconds(seconds);
        int i = 0;

        while (DateTime.Now < endTime)
        {
            string s = animationStrings[i];
            Console.Write(s);
            Thread.Sleep(1000);
            Console.Write("\b \b");

            i++;

            if (i >= animationStrings.Count)
            {
                i = 0;
            }
        }
    }
    public void ShowCountDown(int seconds)
    {
        // Show seconds counts on the same spot

        for (int i = seconds; i > 0; i--)
        {
            Console.Write(i);
            Thread.Sleep(1000);
            Console.Write("\b \b");
        }
    }

    protected bool IsWithinDuration(System.Diagnostics.Stopwatch timer)
    {
        return timer.Elapsed < TimeSpan.FromSeconds(GetDuration());
    }

    protected void ShowCountDown(int seconds, System.Diagnostics.Stopwatch timer)
    {
        int count = seconds;
        while (count > 0 && IsWithinDuration(timer))
        {
            Console.Write(count);
            TimeSpan remaining = TimeSpan.FromSeconds(GetDuration()) - timer.Elapsed;
            if (remaining <= TimeSpan.Zero)
            {
                break;
            }

            Thread.Sleep(remaining < TimeSpan.FromSeconds(1)
                ? remaining
                : TimeSpan.FromSeconds(1));
            Console.Write("\b \b");
            count--;
        }
    }

    protected string ReadLineWithinDuration(System.Diagnostics.Stopwatch timer)
    {
        if (Console.IsInputRedirected)
        {
            throw new InvalidOperationException("Timed activity input requires an interactive console.");
        }

        StringBuilder input = new StringBuilder();
        while (IsWithinDuration(timer))
        {
            if (!Console.KeyAvailable)
            {
                Thread.Sleep(10);
                continue;
            }

            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return input.ToString();
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (input.Length > 0)
                {
                    input.Length--;
                    Console.Write("\b \b");
                }

                continue;
            }

            if (!char.IsControl(key.KeyChar))
            {
                input.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }

        Console.WriteLine();
        return null;
    }

    public void WaitAMoment()
    {
        // showing dots while waiting
        Console.Clear();
        Console.Write("Get ready");
        for (int i = 5; i > 0; i--)
        {
            Console.Write(".");
            Thread.Sleep(1000);
        }
    }
    private static readonly SoundPlayer _lowBowl =
    new SoundPlayer(Path.Combine(AppContext.BaseDirectory, "Sounds", "Breathin.wav"));
    private static readonly SoundPlayer _highBowl =
        new SoundPlayer(Path.Combine(AppContext.BaseDirectory, "Sounds", "Breathout.wav"));

    public void PlayBowlSound(bool highTone)
    {
        SoundPlayer player = highTone ? _highBowl : _lowBowl;
        player.Load();
        player.Play();
    }
}
