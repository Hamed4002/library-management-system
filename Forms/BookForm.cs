using Library_Management_System.Exceptions;
using Library_Management_System.Models;
using Library_Management_System.Services;

namespace Library_Management_System.Forms
{
    public class BookForm : Form
    {
        private TextBox codeText = null!;
        private TextBox titleText = null!;
        private TextBox authorText = null!;
        private TextBox genreText = null!;

        private readonly Book? existingBook;

        public BookForm(Book? book = null)
        {
            existingBook = book;
            InitializeUI();

            if (book != null)
            {
                Text = "Edit Book";
                codeText.Text = book.Code.ToString();
                titleText.Text = book.Title;
                authorText.Text = book.Author;
                genreText.Text = book.Genre;
            }
            else
            {
                Text = "Add Book";
            }
        }

        private void InitializeUI()
        {
            Width = 420;
            Height = 300;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F);

            var lblCode = new Label { Text = "Code:", Left = 20, Top = 25, Width = 100 };
            codeText = new TextBox { Left = 130, Top = 22, Width = 250 };

            var lblTitle = new Label { Text = "Title:", Left = 20, Top = 65, Width = 100 };
            titleText = new TextBox { Left = 130, Top = 62, Width = 250 };

            var lblAuthor = new Label { Text = "Author:", Left = 20, Top = 105, Width = 100 };
            authorText = new TextBox { Left = 130, Top = 102, Width = 250 };

            var lblGenre = new Label { Text = "Genre:", Left = 20, Top = 145, Width = 100 };
            genreText = new TextBox { Left = 130, Top = 142, Width = 250 };

            var saveBtn = new Button { Text = "Save", Left = 130, Top = 200, Width = 110, Height = 30 };
            saveBtn.Click += SaveBtn_Click;

            var cancelBtn = new Button { Text = "Cancel", Left = 260, Top = 200, Width = 110, Height = 30 };
            cancelBtn.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(lblCode);
            Controls.Add(codeText);
            Controls.Add(lblTitle);
            Controls.Add(titleText);
            Controls.Add(lblAuthor);
            Controls.Add(authorText);
            Controls.Add(lblGenre);
            Controls.Add(genreText);
            Controls.Add(saveBtn);
            Controls.Add(cancelBtn);

            AcceptButton = saveBtn;
            CancelButton = cancelBtn;
        }

        private void SaveBtn_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(codeText.Text.Trim(), out int code))
            {
                MessageBox.Show("Code must be a valid number.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(titleText.Text) ||
                string.IsNullOrWhiteSpace(authorText.Text))
            {
                MessageBox.Show("Title and author are required.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var book = new Book(
                code,
                titleText.Text.Trim(),
                authorText.Text.Trim(),
                genreText.Text.Trim()
            );

            try
            {
                if (existingBook == null)
                    BookService.Add(book);
                else
                    BookService.Update(existingBook.Code, book);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}