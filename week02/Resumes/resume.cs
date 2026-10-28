using System;
using System.Collections.Generic;

public class Resume
{
    public string _name = "";
    public List<Job> _jobs = new List<Job>();   // initialized — never null

    public void Display()
    {
        Console.WriteLine($"Name: {_name}");           // print name once
        Console.WriteLine("Jobs:");

        if (_jobs.Count == 0)
        {
            Console.WriteLine("  (no jobs listed)");
            return;
        }

        foreach (Job job in _jobs)
        {
            job.Display();                       // Job.Display handles its own output
        }
    }
}