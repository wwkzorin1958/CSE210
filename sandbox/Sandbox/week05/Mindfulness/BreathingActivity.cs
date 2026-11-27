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

        while (IsWithinDuration(timer))
        {
            Console.Write("\nBreathe in: ");
            PlayBowlSound(false);
            ShowCountDown(5, timer);

            if (!IsWithinDuration(timer))
            {
                break;
            }

            Console.Write("\nBreathe out: ");
            PlayBowlSound(true);
            ShowCountDown(5, timer);
        }

        timer.Stop();
        new ActivityHistory().Save(new ActivityRecord(
            GetName(), startedAt, GetDuration(), timer.Elapsed.TotalSeconds, new List<ActivityResponse>()));
        DisplayEndingMessage();

    }
}