using System;
using System.Net.Http.Headers;

namespace game
{
    class Program
    {
        static void Main(string[] args)
        {
            Book book1 = new Book();
            book1.SetAuthor("Smith");
            book1.SetTitle("A Great Book");

            Console.WriteLine(book1.GetBookInfo());

            PictureBook book2 = new PictureBook();
            book2.SetAuthor("Jones");
            book2.SetTitle("A Wonderful Picture Book");
            book2.SetIllustrator("Burton");
            string illustrator = book2.GetIllustrator();

            Console.WriteLine($"{book2.GetBookInfo()} with Illustrator {illustrator}");
            Console.WriteLine(book2.GetPictureBookInfo());

            Book book3 = new Book();
            Console.WriteLine(book3.GetBookInfo());

            PictureBook book4 = new PictureBook();
            Console.WriteLine(book4.GetPictureBookInfo());

            Book book5 = new Book("Edwards", "Another book");
            Console.WriteLine(book4.GetBookInfo());

            PictureBook book6 = new PictureBook();
            Console.WriteLine(book6.GetPictureBookInfo());
        }
    }
}