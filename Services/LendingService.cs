using Library_Management_System.Data;
using Library_Management_System.Exceptions;
using Library_Management_System.Models;

namespace Library_Management_System.Services
{
    public static class LendingService
    {
        public static List<Lending> GetAll()
        {
            return ExcelContext.GetLendings();
        }

        public static Lending? FindByBookCode(int bookCode)
        {
            return GetAll().FirstOrDefault(l => l.BookCode == bookCode);
        }

        public static bool IsLent(int bookCode)
        {
            return GetAll().Any(l => l.BookCode == bookCode);
        }

        public static bool HasAnyLending(int memberId)
        {
            return GetAll().Any(l => l.MemberId == memberId);
        }

        public static void Lend(int bookCode, int memberId)
        {
            if (BookService.FindByCode(bookCode) == null)
                throw new NotFoundException($"Book with code {bookCode} was not found.");

            if (MemberService.FindById(memberId) == null)
                throw new NotFoundException($"Member with ID {memberId} was not found.");

            if (IsLent(bookCode))
                throw new LendingException($"Book with code {bookCode} is currently lent out.");

            var lendings = GetAll();
            lendings.Add(new Lending(bookCode, memberId, DateTime.Now));
            ExcelContext.SaveLendings(lendings);
        }

        public static void Return(int bookCode)
        {
            var lendings = GetAll();
            var lending = lendings.FirstOrDefault(l => l.BookCode == bookCode);

            if (lending == null)
                throw new LendingException($"Book with code {bookCode} is not currently lent.");

            lendings.Remove(lending);
            ExcelContext.SaveLendings(lendings);
        }

        public static List<Lending> GetByMember(int memberId)
        {
            return GetAll().Where(l => l.MemberId == memberId).ToList();
        }
    }
}