// Add a prompt for user to enter the number of words to be hidden
// Load the scriptures from the text file master_scriptures_reference.txt
// located under ..\bin\Debug\net10.0

using System;

class Program
{
    static void ClearConsole()
    {
        Console.Write("\u001b[3J\u001b[2J\u001b[H");
    }

    static void Main(string[] args)
    {
        ScriptureLibrary library = new ScriptureLibrary("master_scriptures_reference.txt");

        if (library.IsEmpty())
        {
            Console.WriteLine("No scriptures loaded.");
            return;
        }

        int wordsToHide;
        while (true)
        {
            Console.Write("How many words should be hidden each turn? ");
            string countInput = Console.ReadLine();

            if (countInput == null)
            {
                Console.WriteLine("Goodbye!");
                return;
            }

            if (int.TryParse(countInput, out wordsToHide) && wordsToHide > 0)
            {
                break;
            }

            Console.WriteLine("Enter a positive whole number.");
        }

        bool keepGoing = true;

        while (keepGoing)
        {
            Scripture scripture = library.GetRandom();
            bool quitThisRound = false;

            // Memorize this scripture
            while (!quitThisRound && !scripture.AllWordsHidden())
            {
                ClearConsole();
                Console.WriteLine(scripture.GetDisplayText());
                Console.WriteLine();

                Console.Write($"Press Enter to hide {wordsToHide} words, or type 'quit': ");
                string input = Console.ReadLine();

                if (input == null)
                {
                    ClearConsole();
                    Console.WriteLine("No input available. Goodbye!");
                    return;
                }

                if (input.Trim().ToLower() == "quit")
                {
                    ClearConsole();
                    quitThisRound = true;
                    keepGoing = false;
                }
                else
                {
                    scripture.HideRandomWords(wordsToHide);
                }
            }

            if (!quitThisRound)
            {
                ClearConsole();
                Console.WriteLine(scripture.GetDisplayText());
                Console.WriteLine();
            }

            if (keepGoing)
            {
                Console.WriteLine("All words hidden!");
                Console.Write("Practice another scripture? (y/n): ");
                string answer = Console.ReadLine();
                keepGoing = answer != null && answer.Trim().ToLower() == "y";
            }
        }

        Console.WriteLine("Goodbye!");
    }
}