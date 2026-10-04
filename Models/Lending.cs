namespace Library_Management_System.Models
{
    public class Lending
    {
        public int BookCode { get; set; }
        public int MemberId { get; set; }
        public DateTime LendDate { get; set; }

        public Lending(int bookCode, int memberId, DateTime lendDate)
        {
            BookCode = bookCode;
            MemberId = memberId;
            LendDate = lendDate;
        }

        public override string ToString()
        {
            return $"{LendDate:yyyy/MM/dd}\t{BookCode,-12}\t{MemberId,-12}";
        }
    }
}
