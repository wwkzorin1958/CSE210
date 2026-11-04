using System;

class Menu
{
    // Only class Menu can access the method in class Journal
    private Journal _journal;

// Constructor to create an object of class Menu with parameter of a Journal object
    public Menu(Journal journal)
    {
        _journal = journal;
    }

// Main Menu
    public void Run()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("1. Write");
            Console.WriteLine("2. Display");
            Console.WriteLine("3. Load");
            Console.WriteLine("4. Save");
            Console.WriteLine("5. Quit");
            Console.Write("What would you like to do? ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": _journal.Write();   break;
                case "2": _journal.Display(); break;
                case "3": _journal.Load();    break;
                case "4": _journal.Save();    break;
                case "5": running = false;    break;
                default:
                    Console.WriteLine("Invalid entry. Please enter 1–5 only!");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    break;
            }
        }
    }
}
