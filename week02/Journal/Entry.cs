using System;
public class Entry
{
    // public string _date;
    // public string _prompt;
    // public string _response; 

    public string Date     { get; set; } = "";
    public string Prompt   { get; set; } = "";
    public string Response { get; set; } = "";

    // public void Display()
    // {
    //     Console.WriteLine($"{_date} - {_prompt}");
    //     Console.WriteLine(_response);
    //     Console.WriteLine();
    // }

    public void Display()
    {
        Console.WriteLine($"{Date} - {Prompt}");
        Console.WriteLine(Response);
        Console.WriteLine();
    }

}