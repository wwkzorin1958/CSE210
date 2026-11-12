using System;

class Program
{
    static void Main(string[] args)
    {
        RunFractionTest();
    }

    static void RunFractionTest()
    {
        Fraction fraction = new Fraction();

        // ---- Get a valid top number ----
        int topInput;
        while (true)
        {
            Console.Write("Enter the top number: ");
            string topText = Console.ReadLine();

            if (int.TryParse(topText, out topInput))
            {
                break;   // valid integer — leave the loop
            }

            Console.WriteLine("Please enter a whole number.");
        }

        fraction.SetTop(topInput);

        // ---- Get a valid bottom number (not 0), using try/catch ----
        while (true)
        {
            Console.Write("Enter the bottom number (not 0): ");
            string bottomText = Console.ReadLine();

            if (!int.TryParse(bottomText, out int bottomInput))
            {
                Console.WriteLine("Please enter a whole number.");
                continue;   // re-prompt
            }

            try
            {
                fraction.SetBottom(bottomInput);   // throws if bottom == 0
                break;                             // success — leave the loop
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);     // "Denominator cannot be zero."
                // loop continues → prompt again
            }
        }

        // ---- Display results ----
        int top = fraction.GetTop();
        int bottom = fraction.GetBottom();

        Console.WriteLine($"Top: {top}");
        Console.WriteLine($"Bottom: {bottom}");
        Console.WriteLine($"Fraction: {fraction.GetFractionString()}");
        Console.WriteLine($"Decimal: {fraction.GetDecimalValue()}");

        // ---- Ask to run again ----
        Console.Write("Would you like to enter another fraction? (y/n): ");
        string response = Console.ReadLine();

        if (string.Equals(response, "y", StringComparison.OrdinalIgnoreCase))
        {
            RunFractionTest();
        }
    }
}