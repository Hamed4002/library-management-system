using Library_Management_System.Data;
using Library_Management_System.Exceptions;
using Library_Management_System.Models;

namespace Library_Management_System.Services
{
    public static class BookService
    {
        public static List<Book> GetAll()
        {
            return ExcelContext.GetBooks();
        }

        public static Book? FindByCode(int code)
        {
            return GetAll().FirstOrDefault(b => b.Code == code);
        }

        public static void Add(Book book)
        {
            var books = GetAll();

            if (books.Any(b => b.Code == book.Code))
                throw new DuplicateException($"A book with code {book.Code} already exists.");

            books.Add(book);
            ExcelContext.SaveBooks(books);
        }

        public static void Update(int oldCode, Book updated)
        {
            if (oldCode != updated.Code && LendingService.IsLent(oldCode))
                throw new LendingException($"Cannot change code of book {oldCode}: it is currently lent out.");

            var books = GetAll();
            var index = books.FindIndex(b => b.Code == oldCode);

            if (index == -1)
                throw new NotFoundException($"Book with code {oldCode} was not found.");

            if (oldCode != updated.Code && books.Any(b => b.Code == updated.Code))
                throw new DuplicateException($"Code {updated.Code} is already in use.");

            books[index] = updated;
            ExcelContext.SaveBooks(books);
        }

        public static void Delete(int code)
        {
            if (LendingService.IsLent(code))
                throw new LendingException($"Cannot delete book {code}: it is currently lent out.");

            var books = GetAll();
            var book = books.FirstOrDefault(b => b.Code == code);

            if (book == null)
                throw new NotFoundException($"Book with code {code} was not found.");

            books.Remove(book);
            ExcelContext.SaveBooks(books);
        }

        public static List<Book> Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return GetAll();

            text = text.Trim().ToLower();
            return GetAll().Where(b =>
                b.Code.ToString().Contains(text) ||
                b.Title.ToLower().Contains(text) ||
                b.Author.ToLower().Contains(text) ||
                b.Genre.ToLower().Contains(text)
            ).ToList();
        }
    }
}