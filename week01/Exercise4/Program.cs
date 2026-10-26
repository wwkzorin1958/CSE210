using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        int inputNumber;
        List<int> numberSeries = new List<int>();
        int sum = 0;
        int countNumber = 0;
        int largestNumber = int.MinValue;
        int smallestPositiveNumber = int.MaxValue;

        Console.WriteLine("Enter a list of numbers, type 0 when finished.");

        do
        {
            Console.Write("Enter number: ");
            while (!int.TryParse(Console.ReadLine(), out inputNumber))
            {
                Console.Write("Please enter a whole number: ");
            }

            if (inputNumber != 0)
            {
                numberSeries.Add(inputNumber);
                sum += inputNumber;
                countNumber++;

                if (inputNumber > largestNumber)
                    largestNumber = inputNumber;

                if (inputNumber > 0 && inputNumber < smallestPositiveNumber)
                    smallestPositiveNumber = inputNumber;
            }
        } while (inputNumber != 0);

        Console.WriteLine($"The sum is: {sum}");

        if (countNumber > 0)
        {
            float averageNumber = (float)sum / countNumber;
            Console.WriteLine($"The average is: {averageNumber:F2}");
            Console.WriteLine($"The largest number is: {largestNumber}");

            if (smallestPositiveNumber != int.MaxValue)
                Console.WriteLine($"The smallest positive number is: {smallestPositiveNumber}");
            else
                Console.WriteLine("No positive numbers were entered.");
        }
        else
        {
            Console.WriteLine("No numbers were entered; average and largest are undefined.");
        }

        numberSeries.Sort();
        foreach (int numberElement in numberSeries)
        {
            Console.WriteLine(numberElement);
        }
    }
}