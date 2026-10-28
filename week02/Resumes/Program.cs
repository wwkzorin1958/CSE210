using System;

class Program
{
    static void Main(string[] args)
    {
        var job1 = new Job();
        job1._company = "Microsoft";
        job1._jobTitle = "Software Engineer";
        job1._startYear = 2019;
        job1._endYear = 2022;
        // job1.Display();

        var job2 = new Job();
        job2._company = "Apple";
        job2._jobTitle = "Manager";
        job2._startYear = 2022;
        job2._endYear = 2023;
        // job2.Display();

        var myResume = new Resume();
        // myResume._name = "Ricky";
        // myResume._jobs.Add(job1);      // ← add them
        // myResume._jobs.Add(job2);      // ←
        // Console.WriteLine(myResume._jobs[0]._jobTitle);
        // Console.WriteLine(myResume._jobs[1]._jobTitle);
        
        myResume._name = "Allison Rose";
        myResume._jobs.Add(job1);
        myResume._jobs.Add(job2);
        myResume.Display();

   
    }
}