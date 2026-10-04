
namespace Library_Management_System.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MembershipType { get; set; }

        public Member(int id, string firstName, string lastName, string membershipType)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            MembershipType = membershipType;
        }

        public override string ToString()
        {
            return $"{Id,-10}\t{FirstName,-20}\t{LastName,-20}\t{MembershipType,-15}";
        }
    }
}
