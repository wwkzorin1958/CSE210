using System.Net.Mail;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
class Video
{
    public String _title;
    public String _author;
    public double _length;
    public List<Comment> Comments;

    public Video(string title, string author, double length)
    {
        _title = title;
        _author = author;
        _length = length;
        Comments = new List<Comment>();
    }

    public void AddComment(string name, string text)
    {
        Comments.Add(new Comment(name, text));
    }

    public void DisplayComment()
    {
        Console.WriteLine($"Video Title: {_title} ");
        Console.WriteLine($"Author: {_author}");
        Console.WriteLine($"Length: {_length} seconds");
        foreach (var item in Comments)
        {
            Console.Write($"User: {item._name}'s comment: ");
            Console.WriteLine(item._text);
        }

    }
}