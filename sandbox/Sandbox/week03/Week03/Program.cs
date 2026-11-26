using Microsoft.VisualBasic;

namespace Week03;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        // Show 5 counts on the same spot
        // for (int i = 5; i > 0; i--)
        // {
        //     Console.Write(i);
        //     Thread.Sleep(1000);
        //     Console.Write("\b \b");
        // }

        // showing dots while waiting
        // for (int i = 5; i > 0; i--)
        // {
        //     Console.Write(".");
        //     Thread.Sleep(1000);
        // }

        // |/-\|/-\|
        List<string> animationStrings = new List<string>();
        animationStrings.Add("|");
        animationStrings.Add("/");
        animationStrings.Add("-");
        animationStrings.Add("\\");
        animationStrings.Add("|");
        animationStrings.Add("/");
        animationStrings.Add("-");
        animationStrings.Add("\\");

        // foreach (string s in animationStrings)
        // {
        //     Console.Write("s");
        //     Thread.Sleep(1000);
        //     Console.Write("\b \b");
        // }

        DateTime startTime = DateTime.Now;
        DateTime endTime = startTime.AddSeconds(10);

        // display a period for a certain seconds
        // while (DateTime.Now < endTime)
        // {
        //     Console.Write(".");
        //     Thread.Sleep(1000);
        // }


        // the spinner will spin for a certain time
        int i = 0;

        while (DateTime.Now < endTime)
        {
            string s = animationStrings[i];
            Console.Write(s);
            Thread.Sleep(1000);
            Console.Write("\b \b");

            i++;

            if (i >= animationStrings.Count)
            {
                i = 0;
            }
        }

        Console.WriteLine("Done");
    }
}
