using System;

class Program
{
    static void Main(string[] args)
    {
        List<Video> videos = new List<Video>();

        Video video = new Video("Learning C#", "Alex", 300);
        video.AddComment("Sam", "This was helpful!");
        video.AddComment("Jordan", "Great explanation.");
        video.AddComment("Maggie", "I need more on class and object!");
        videos.Add(video);

        Video video2 = new Video("Using Visual Studio", "Sam", 500);
        video2.AddComment("Adam", "Very detailed explanation!");
        video2.AddComment("Oscar", "More videos on Visual Studio, please.");
        video2.AddComment("William", "This video is for beginners.");
        video2.AddComment("Kevin", "What about pyCharm?");
        videos.Add(video2);

        foreach (Video currentVideo in videos)
        {
            Console.WriteLine($"{currentVideo._title} has {currentVideo._comments.Count} comments.");
            currentVideo.DisplayComment();
            Console.WriteLine();
        }
    }
}