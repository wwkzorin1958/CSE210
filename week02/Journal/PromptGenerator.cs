using System;
using System.Collections.Generic;

class PromptGenerator
{
    private Random _random = new Random();

    private List<string> _prompts = new List<string>
    {
        "Who was the most interesting person I interacted with today?",
        "What was the best part of your day?",
        "How did I see the hand of the Lord in my life today?",
        "What was the strongest emotion I felt today?",
        "If I had one thing I could do over today, what would it be?",
        "What was the most challenging thing today?",
        "Who did you help today, and how?",
        "What are you grateful for right now?",
        "What did you learn today?",
        "What made you smile today?",
        "What would you do differently today?",
        "Describe a moment that stood out to you."
    };

    public string GetRandomPrompt()
    {
        int index = _random.Next(_prompts.Count);
        return _prompts[index];
    }
}