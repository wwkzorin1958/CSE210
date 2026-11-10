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

        Console.Write("Enter the top number: ");
        int topInput = int.Parse(Console.ReadLine());

        Console.Write("Enter the bottom number: ");
        int bottomInput = int.Parse(Console.ReadLine());

        fraction.SetTop(topInput);
        fraction.SetBottom(bottomInput);

        int top = fraction.GetTop();
        int bottom = fraction.GetBottom();

        Console.WriteLine($"Top: {top}");
        Console.WriteLine($"Bottom: {bottom}");
        Console.WriteLine($"Fraction: {fraction.GetFractionString()}");
        Console.WriteLine($"Decimal: {fraction.GetDecimalValue()}");

        Console.Write("Would you like to enter another fraction? (y/n): ");
        string response = Console.ReadLine();

        if (string.Equals(response, "y", StringComparison.OrdinalIgnoreCase))
        {
            RunFractionTest();
        }
   }
}