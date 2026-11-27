public class BreathingActivity : Activity
{
    public BreathingActivity(string name, string description, int duration) : base(name, description, duration)
    {

    }

    public void Run()
    {
        DisplayStartingMessage();
        DateTimeOffset startedAt = DateTimeOffset.Now;
        System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
        DateTime endTime = DateTime.Now.AddSeconds(GetDuration());

        while (DateTime.Now < endTime)
        {
            Console.Write("\nBreathe in: ");
            PlayBowlSound(false);
            ShowCountDown(5);

            Console.Write("\nBreathe out: ");
            PlayBowlSound(true);
            ShowCountDown(5);
        }

        timer.Stop();
        new ActivityHistory().Save(new ActivityRecord(
            GetName(), startedAt, GetDuration(), timer.Elapsed.TotalSeconds, new List<ActivityResponse>()));
        DisplayEndingMessage();

    }
}