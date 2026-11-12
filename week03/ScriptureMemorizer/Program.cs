using System;

class Program
{
    static void Main(string[] args)
    {
        // Build the scripture (single verse example)
        Reference reference = new Reference("John", 3, 16);
        string text = "For God so loved the world that he gave his only begotten Son";
        Scripture scripture = new Scripture(reference, text);

        // Main loop
        while (true)
        {
            Console.Clear();
            Console.WriteLine(scripture.GetDisplayText());
            Console.WriteLine();

            if (scripture.AllWordsHidden())
            {
                Console.WriteLine("All words hidden. Press Enter to exit.");
                Console.ReadLine();
                break;
            }

            Console.Write("Press Enter to continue, or type 'quit' to exit: ");
            string input = Console.ReadLine();

            if (input != null && input.Trim().ToLower() == "quit")
            {
                break;
            }

            scripture.HideRandomWords(3);
        }
    }
}