using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("What is your grade percentage? ");
        string userGrade = Console.ReadLine();
        float grade = int.Parse(userGrade);
        string letter = "";
        string sign = "";
        if (grade >= 90)
        {
            letter = "A";
        }
        else if (grade >= 80)
        {
            letter = "B";
        }
        else if (grade >= 70)
        {
           
            letter = "C";
        }
        else if (grade >= 60)
        {
            letter = "D";
        }
        else
        {
            letter = "F";
        }
        if ((grade % 10) >= 7)
        {
            sign = "+";
        }
        else if ((grade % 10) < 3)
        {
            sign = "-";
        }
        if ((grade > 93) || (grade < 60))
        {
            sign = "";
        }
        if (grade > 70)
        {
             Console.WriteLine("Congratulation, you pass the course.");    
        }
        else
        {
             Console.WriteLine("Sorry, you fail this time.  Try again!");
        }
        Console.WriteLine($"You got a {letter}{sign}");
    }
}