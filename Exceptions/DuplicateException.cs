namespace Library_Management_System.Exceptions
{
    public class DuplicateException : AppException
    {
        public DuplicateException(string message) : base(message) { }
    }
}
