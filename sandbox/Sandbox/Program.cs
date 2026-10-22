using System;

namespace game
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! This is the Sandbox Project.");
            Console.WriteLine("This is in C#.");

            Console.Write("What is your favorite color? ");
            string color = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(color))
            {
                Console.WriteLine("You didn't enter a color.");
            }
            else
            {
                Console.WriteLine($"Your color is {color}");
            }
        }
    }
}