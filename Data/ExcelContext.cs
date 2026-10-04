using ClosedXML.Excel;
using Library_Management_System.Models;

namespace Library_Management_System.Data
{
    public static class ExcelContext
    {
        private const string FilePath = "./LMS.xlsx";
        private const string MembersSheet = "Members";
        private const string BooksSheet = "Books";
        private const string LendingsSheet = "Lendings";

        public static void EnsureFileExists()
        {
            if (File.Exists(FilePath)) return;

            using var workbook = new XLWorkbook();
            workbook.Worksheets.Add(MembersSheet);
            workbook.Worksheets.Add(BooksSheet);
            workbook.Worksheets.Add(LendingsSheet);
            workbook.SaveAs(FilePath);
        }

        public static List<Member> GetMembers()
        {
            using var workbook = new XLWorkbook(FilePath);
            var sheet = workbook.Worksheet(MembersSheet);

            return sheet.RowsUsed()
                .Select(r => new Member(
                    r.Cell(1).GetValue<int>(),
                    r.Cell(2).GetValue<string>(),
                    r.Cell(3).GetValue<string>(),
                    r.Cell(4).GetValue<string>()))
                .ToList();
        }

        public static List<Book> GetBooks()
        {
            using var workbook = new XLWorkbook(FilePath);
            var sheet = workbook.Worksheet(BooksSheet);

            return sheet.RowsUsed()
                .Select(r => new Book(
                    r.Cell(1).GetValue<int>(),
                    r.Cell(2).GetValue<string>(),
                    r.Cell(3).GetValue<string>(),
                    r.Cell(4).GetValue<string>()))
                .ToList();
        }

        public static List<Lending> GetLendings()
        {
            using var workbook = new XLWorkbook(FilePath);
            var sheet = workbook.Worksheet(LendingsSheet);

            return sheet.RowsUsed()
                .Select(r => new Lending(
                    r.Cell(1).GetValue<int>(),
                    r.Cell(2).GetValue<int>(),
                    DateTime.Parse(r.Cell(3).GetValue<string>())))
                .ToList();
        }

        public static void SaveMembers(List<Member> members)
        {
            using var workbook = new XLWorkbook(FilePath);
            var sheet = workbook.Worksheet(MembersSheet);
            sheet.Clear();

            for (int i = 0; i < members.Count; i++)
            {
                var m = members[i];
                var row = i + 1;

                sheet.Cell(row, 1).Value = m.Id;
                sheet.Cell(row, 2).Value = m.FirstName;
                sheet.Cell(row, 3).Value = m.LastName;
                sheet.Cell(row, 4).Value = m.MembershipType;
            }
            workbook.Save();
        }

        public static void SaveBooks(List<Book> books)
        {
            using var workbook = new XLWorkbook(FilePath);
            var sheet = workbook.Worksheet(BooksSheet);
            sheet.Clear();

            for (int i = 0; i < books.Count; i++)
            {
                var b = books[i];
                var row = i + 1;

                sheet.Cell(row, 1).Value = b.Code;
                sheet.Cell(row, 2).Value = b.Title;
                sheet.Cell(row, 3).Value = b.Author;
                sheet.Cell(row, 4).Value = b.Genre;
            }
            workbook.Save();
        }

        public static void SaveLendings(List<Lending> lendings)
        {
            using var workbook = new XLWorkbook(FilePath);
            var sheet = workbook.Worksheet(LendingsSheet);
            sheet.Clear();

            for (int i = 0; i < lendings.Count; i++)
            {
                var l = lendings[i];
                var row = i + 1;

                sheet.Cell(row, 1).Value = l.BookCode;
                sheet.Cell(row, 2).Value = l.MemberId;
                sheet.Cell(row, 3).Value = l.LendDate.ToString("yyyy/MM/dd");
            }
            workbook.Save();
        }
    }
}