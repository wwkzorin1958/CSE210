using System.Net.Mail;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
class Video
{
    public String _title;
    public String _author;
    public double _length;
    public List<Comment> _comments;

    public Video(string title, string author, double length)
    {
        _title = title;
        _author = author;
        _length = length;
        _comments = new List<Comment>();
    }

    public void AddComment(string name, string text)
    {
        _comments.Add(new Comment(name, text));
    }

    public void DisplayComment()
    {
        Console.WriteLine($"Video Title: {_title} ");
        Console.WriteLine($"Author: {_author}");
        Console.WriteLine($"Length: {_length} seconds");
        foreach (var item in _comments)
        {
            Console.Write($"User: {item._name}'s comment: ");
            Console.WriteLine(item._text);
        }

    }
}