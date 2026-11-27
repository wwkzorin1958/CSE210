using System;
using System.Media;

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
