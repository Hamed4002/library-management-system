namespace Library_Management_System.Models
{
    public class Book
    {
        public int Code { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }

        public Book(int code, string title, string author, string genre)
        {
            Code = code;
            Title = title;
            Author = author;
            Genre = genre;
        }

        public override string ToString()
        {
            return $"{Code,-10}\t{Title,-35}\t{Author,-25}\t{Genre,-15}";
        }
    }
}
