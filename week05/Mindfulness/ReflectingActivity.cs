public class ReflectingActivity : Activity
{
    private List<string> _prompt;
    private List<string> _questions;
    private List<string> _remainingQuestions = new List<string>();

    public ReflectingActivity(string name, string description, int duration) : base(name, description, duration)
    {
        _prompt = new List<string>
        {
            "Think of a time when you stood up for someone else.",
            "Think of a time when you did something really difficult.",
            "Think of a time when you helped someone in need.",
            "Think of a time when you did something truly selfless."
        };

        _questions = new List<string>
        {
            "Why was this experience meaningful to you?",
            "Have you ever done anything like this before?",
            "How did you get started?",
            "How did you feel when it was complete?",
            "What made this time different than other times when you were not as successful?",
            "What is your favorite thing about this experience?",
            "What could you learn from this experience that applies to other situations?",
            "What did you learn about yourself through this experience?",
            "How can you keep this experience in mind in the future?",
            "Where did your mind go during the practice? Did it stay in the present, or did it wander to the past or future?",
            "When you noticed your mind wandering, were you able to bring it back gently, or was it difficult?",
            "What was the most noticeable physical sensation you felt during the practice (e.g., the breath, the weight of your body, a sound)?",
            "Did you notice any moments of stillness or quiet in your mind? What did that feel like?",
            "What thoughts or emotions arose while you were practicing?",
            "Were you able to observe your thoughts without judging them (e.g., \"I am having a thought about work\" instead of \"I can't stop thinking about work\")?",
            "Did you find yourself judging the experience (e.g., \"I\'m bad at this,\" \"This is boring\")? How did you respond to that judgment?",
            "If you felt a strong emotion, where did you feel it in your body?",
            "How does your body feel right now, compared to before you started the practice? (e.g., more relaxed, tense, heavy, light?)",
            "Did you notice any areas of tension in your body that you weren't aware of before?",
            "Did you find it easy or challenging to connect with your physical sensations?",
            "Was there any part of the practice that felt particularly challenging or uncomfortable? How did you handle that?",
            "Did you find yourself wanting the experience to be different than it was?",
            "What was the easiest part of the practice for you today?",
            "How did you speak to yourself when you got distracted?",
            "How might you bring this same quality of awareness into your next activity (e.g., walking, eating, or talking to someone)?",
            "What is one small thing you noticed today that you usually take for granted?",
            "How do you feel your nervous system responded to this pause?"
        };
    }

    public List<string> GetPrompt()
    {
        return _prompt;
    }
    public string GetRandomPrompt()
    {
        int index = Random.Shared.Next(_prompt.Count);
        return _prompt[index];
    }
    public void SetPrompt(List<string> prompt)
    {
        _prompt = prompt;
    }
    public List<string> GetQuestions()
    {
        return _questions;
    }

    public string GetRandomQuestion()
    {
        if (_remainingQuestions.Count == 0)
        {
            if (_questions.Count == 0)
            {
                throw new InvalidOperationException("There are no reflection questions.");
            }

            _remainingQuestions.AddRange(_questions);
        }

        int index = Random.Shared.Next(_remainingQuestions.Count);
        string question = _remainingQuestions[index];
        _remainingQuestions.RemoveAt(index);
        return question;
    }
    public void SetQuestions(List<string> questions)
    {
        _questions = questions;
        _remainingQuestions.Clear();
    }
    public void Run()
    {
        _remainingQuestions.Clear();
        DisplayStartingMessage();

        string prompt = GetRandomPrompt();
        Console.Write($"\n --- {prompt}");
        PlayBowlSound(false);

        Console.Write("\nType your thoughts and press Enter, or press Enter to continue without writing: ");
        string initialThoughts = Console.ReadLine();
        if (initialThoughts == null)
        {
            throw new EndOfStreamException("Input ended before the reflecting activity began.");
        }

        List<ActivityResponse> responses = new List<ActivityResponse>
        {
            new ActivityResponse(prompt, new List<string> { initialThoughts })
        };
        DateTimeOffset startedAt = DateTimeOffset.Now;
        System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
        DateTime endTime = DateTime.Now.AddSeconds(GetDuration());
        bool highTone = true;

        while (DateTime.Now < endTime)
        {
            string question = GetRandomQuestion();
            Console.WriteLine($"\n{question}");
            PlayBowlSound(highTone);
            highTone = !highTone;
            Console.Write("Type your response and press Enter, or press Enter to skip: ");
            string answer = Console.ReadLine();
            if (answer == null)
            {
                throw new EndOfStreamException("Input ended before the reflecting activity was complete.");
            }

            responses.Add(new ActivityResponse(question, new List<string> { answer }));
        }

        timer.Stop();
        new ActivityHistory().Save(new ActivityRecord(
            GetName(), startedAt, GetDuration(), timer.Elapsed.TotalSeconds, responses));
        DisplayEndingMessage();
    }

}