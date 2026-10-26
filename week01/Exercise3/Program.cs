using System;

class Program
{
    static void Main(string[] args)
    {
        Random random = new Random();
        bool play = true;

        while (play)
        {
            int magicNumber = random.Next(1, 101);   // 1–100
            int count = 0;
            int guessNumber;

            do
            {
                Console.Write("What is your guess? ");
                while (!int.TryParse(Console.ReadLine(), out guessNumber))
                {
                    Console.Write("Please enter a whole number: ");
                }

                if (guessNumber < magicNumber)
                    Console.WriteLine("Higher");
                else if (guessNumber > magicNumber)
                    Console.WriteLine("Lower");

                count++;
            }
            while (guessNumber != magicNumber);

            Console.WriteLine($"You guessed it! It took you {count} guesses.");

            Console.Write("Do you want to continue to play (Y/N)? ");
            play = Console.ReadLine()?.Trim().ToUpper() == "Y";
        }
    }
}
