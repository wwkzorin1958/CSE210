public class ListingActivity : Activity
{
    private int _count;
    private List<string> _prompt;

    public ListingActivity(string name, string description, int duration, int count) : base(name, description, duration)
    {
        _count = count;
        _prompt = new List<string>
        {
            "Who are people you appreciate?",
            "What are some things you enjoyed this week?",
            "What are personal strengths of yours?",
            "Who are people that you have helped this week?",
            "When have you felt the Holy Ghost this month?",
            "Who are some of your personal heroes?",
            "List everything you can currently hear in this room and outside of it.",
            "List every physical sensation you can feel right now (e.g., the chair beneath you, the air on your skin, the texture of your clothes).",
            "List all the colors you can see in your immediate field of vision.",
            "List 10 things you are grateful for in this exact moment.",
            "List all the emotions you have felt today, without judging them as good or bad.",
            "List the things that are currently taking up space in your mind (worries, to-do lists, daydreams).",
            "List 10 things that bring you a sense of peace or calm.",
            "List the things you are holding onto that you might be ready to let go of.",
            "List all the things you do on 'autopilot' every single day (e.g., brushing teeth, driving, scrolling).",
            "List the small, simple pleasures you often overlook.",
            "What was it like to write without stopping to edit or judge?",
            "Did you notice your mind wandering while you were writing? Where did it go?",
            "Was there any resistance or discomfort when you couldn't think of something to write?",
            "Look at your list. What is the most surprising thing you wrote down?",
            "Did you notice any physical tension in your hand or shoulders as you were writing? Were you able to release it?",
            "If you had to choose just one item on your list to focus on for the rest of the day, what would it be?",
            "How did it feel to let your thoughts flow freely onto the paper?"

        };
    }

    public int GetCount()
    {
        return _count;
    }
    public void SetCount(int count)
    {
        _count = count;
    }
    public List<string> GetPrompt()
    {
        return _prompt;
    }

    public void SetPrompt(List<string> prompt)
    {
        _prompt = prompt;
    }

    public void Run()
    {
        _count = 0;
        DisplayStartingMessage();
        string currentPrompt = GetRandomPrompt();
        Console.WriteLine($"\n--- {currentPrompt}");
        PlayBowlSound(false);
        Console.WriteLine("Enter one item per line, or press Enter on a blank line for another prompt.");
        Console.WriteLine("If time runs out while you are typing, press Enter to finish.");

        List<ActivityResponse> responses = new List<ActivityResponse>();
        DateTimeOffset startedAt = DateTimeOffset.Now;
        System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
        List<string> items = GetListFromUser(currentPrompt, responses, timer);
        timer.Stop();
        _count = items.Count;

        new ActivityHistory().Save(new ActivityRecord(
            GetName(), startedAt, GetDuration(), timer.Elapsed.TotalSeconds, responses));
        PlayBowlSound(true);
        Console.WriteLine($"\nYou listed {_count} items.");
        DisplayEndingMessage();
    }
    public string GetRandomPrompt()
    {
        if (_prompt == null || _prompt.Count == 0)
        {
            throw new InvalidOperationException("There are no listing prompts.");
        }

        int index = Random.Shared.Next(_prompt.Count);
        return _prompt[index];
    }
    public List<string> GetListFromUser(string currentPrompt)
    {
        return GetListFromUser(currentPrompt, new List<ActivityResponse>(),
            System.Diagnostics.Stopwatch.StartNew());
    }

    private List<string> GetListFromUser(
        string currentPrompt, List<ActivityResponse> responses, System.Diagnostics.Stopwatch timer)
    {
        List<string> items = new List<string>();
        responses.Add(new ActivityResponse(currentPrompt, new List<string>()));

        while (timer.Elapsed < TimeSpan.FromSeconds(GetDuration()))
        {
            Console.Write("> ");
            string item = Console.ReadLine();
            if (item == null)
            {
                throw new EndOfStreamException("Input ended before the listing activity was complete.");
            }

            if (string.IsNullOrWhiteSpace(item))
            {
                if (timer.Elapsed < TimeSpan.FromSeconds(GetDuration()))
                {
                    List<string> otherPrompts = _prompt
                        .Where(prompt => prompt != currentPrompt)
                        .ToList();

                    if (otherPrompts.Count > 0)
                    {
                        currentPrompt = otherPrompts[Random.Shared.Next(otherPrompts.Count)];
                        Console.WriteLine($"\n--- {currentPrompt}");
                        responses.Add(new ActivityResponse(currentPrompt, new List<string>()));
                    }
                    else
                    {
                        Console.WriteLine("There are no other prompts available.");
                    }
                }

                continue;
            }

            items.Add(item);
            responses[responses.Count - 1].Answers.Add(item);
        }

        return items;
    }


}