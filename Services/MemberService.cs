using Library_Management_System.Data;
using Library_Management_System.Exceptions;
using Library_Management_System.Models;

namespace Library_Management_System.Services
{
    public static class MemberService
    {
        public static List<Member> GetAll()
        {
            return ExcelContext.GetMembers();
        }

        public static Member? FindById(int id)
        {
            return GetAll().FirstOrDefault(m => m.Id == id);
        }

        public static void Add(Member member)
        {
            var members = GetAll();

            if (members.Any(m => m.Id == member.Id))
                throw new DuplicateException($"A member with ID {member.Id} already exists.");

            members.Add(member);
            ExcelContext.SaveMembers(members);
        }

        public static void Update(int oldId, Member updated)
        {
            if (oldId != updated.Id && LendingService.HasAnyLending(oldId))
                throw new LendingException($"Cannot change ID of member {oldId}: they still have borrowed books.");

            var members = GetAll();
            var index = members.FindIndex(m => m.Id == oldId);

            if (index == -1)
                throw new NotFoundException($"Member with ID {oldId} was not found.");

            if (oldId != updated.Id && members.Any(m => m.Id == updated.Id))
                throw new DuplicateException($"ID {updated.Id} is already in use.");

            members[index] = updated;
            ExcelContext.SaveMembers(members);
        }

        public static void Delete(int id)
        {
            if (LendingService.HasAnyLending(id))
                throw new LendingException($"Cannot delete member {id}: they still have borrowed books.");

            var members = GetAll();
            var member = members.FirstOrDefault(m => m.Id == id);

            if (member == null)
                throw new NotFoundException($"Member with ID {id} was not found.");

            members.Remove(member);
            ExcelContext.SaveMembers(members);
        }

        public static List<Member> Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return GetAll();

            text = text.Trim().ToLower();
            return GetAll().Where(m =>
                m.Id.ToString().Contains(text) ||
                m.FirstName.ToLower().Contains(text) ||
                m.LastName.ToLower().Contains(text)
            ).ToList();
        }
    }
}