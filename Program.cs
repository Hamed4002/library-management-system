using Library_Management_System.Data;

namespace Library_Management_System
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ExcelContext.EnsureFileExists();

            ApplicationConfiguration.Initialize();
            Application.Run(new MainWindow());
        }
    }
}