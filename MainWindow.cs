using Library_Management_System.Exceptions;
using Library_Management_System.Forms;
using Library_Management_System.Models;
using Library_Management_System.Services;

namespace Library_Management_System
{
    public class MainWindow : Form
    {
        private TabControl tabControl = null!;
        private DataGridView membersGrid = null!;
        private DataGridView booksGrid = null!;
        private DataGridView lendingGrid = null!;
        private TextBox memberSearch = null!;
        private TextBox bookSearch = null!;

        public MainWindow()
        {
            InitializeUI();
            LoadAllData();
        }

        private void InitializeUI()
        {
            Text = "Library Management System";
            Width = 1050;
            Height = 680;
            MinimumSize = new Size(850, 520);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F);

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Padding = new Point(15, 8)
            };

            tabControl.TabPages.Add(CreateMembersTab());
            tabControl.TabPages.Add(CreateBooksTab());
            tabControl.TabPages.Add(CreateLendingTab());

            Controls.Add(tabControl);
        }

        private DataGridView CreateGrid()
        {
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false
            };
            grid.RowTemplate.Height = 28;
            return grid;
        }

        private Button CreateButton(string text, int left)
        {
            return new Button
            {
                Text = text,
                Left = left,
                Top = 12,
                Width = 110,
                Height = 30
            };
        }

        private Panel CreateTopPanel(out TextBox searchBox, EventHandler onSearch)
        {
            var panel = new Panel { Dock = DockStyle.Top, Height = 50 };
            var label = new Label { Text = "Search:", Left = 10, Top = 15, Width = 60 };
            searchBox = new TextBox { Left = 75, Top = 12, Width = 280 };
            searchBox.TextChanged += onSearch;

            panel.Controls.Add(label);
            panel.Controls.Add(searchBox);
            return panel;
        }

        // MEMBERS TAB
        private TabPage CreateMembersTab()
        {
            var page = new TabPage("Members") { Padding = new Padding(10) };

            var top = CreateTopPanel(out memberSearch, (s, e) => LoadMembers(memberSearch.Text));

            membersGrid = CreateGrid();
            membersGrid.Columns.Add("Id", "ID");
            membersGrid.Columns.Add("FirstName", "First Name");
            membersGrid.Columns.Add("LastName", "Last Name");
            membersGrid.Columns.Add("MembershipType", "Membership Type");
            membersGrid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) EditMember(); };

            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 55 };
            var addBtn = CreateButton("Add Member", 10);
            addBtn.Click += (s, e) => AddMember();
            var editBtn = CreateButton("Edit", 130);
            editBtn.Click += (s, e) => EditMember();
            var delBtn = CreateButton("Delete", 250);
            delBtn.Click += (s, e) => DeleteMember();
            var refBtn = CreateButton("Refresh", 370);
            refBtn.Click += (s, e) => LoadMembers(memberSearch.Text);

            bottom.Controls.Add(addBtn);
            bottom.Controls.Add(editBtn);
            bottom.Controls.Add(delBtn);
            bottom.Controls.Add(refBtn);

            page.Controls.Add(membersGrid);
            page.Controls.Add(top);
            page.Controls.Add(bottom);
            return page;
        }

        private void LoadMembers(string filter = "")
        {
            membersGrid.Rows.Clear();
            var members = MemberService.Search(filter);
            foreach (var m in members)
            {
                membersGrid.Rows.Add(m.Id, m.FirstName, m.LastName, m.MembershipType);
            }
        }

        private void AddMember()
        {
            using var form = new MemberForm();
            if (form.ShowDialog(this) == DialogResult.OK)
                LoadMembers(memberSearch.Text);
        }

        private void EditMember()
        {
            if (membersGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a member first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = Convert.ToInt32(membersGrid.SelectedRows[0].Cells[0].Value);
            var member = MemberService.FindById(id);
            if (member == null) return;

            using var form = new MemberForm(member);
            if (form.ShowDialog(this) == DialogResult.OK)
                LoadMembers(memberSearch.Text);
        }

        private void DeleteMember()
        {
            if (membersGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a member first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = Convert.ToInt32(membersGrid.SelectedRows[0].Cells[0].Value);
            var confirm = MessageBox.Show($"Delete member with ID {id}?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                MemberService.Delete(id);
                LoadMembers(memberSearch.Text);
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // BOOKS TAB 
        private TabPage CreateBooksTab()
        {
            var page = new TabPage("Books") { Padding = new Padding(10) };

            var top = CreateTopPanel(out bookSearch, (s, e) => LoadBooks(bookSearch.Text));

            booksGrid = CreateGrid();
            booksGrid.Columns.Add("Code", "Code");
            booksGrid.Columns.Add("Title", "Title");
            booksGrid.Columns.Add("Author", "Author");
            booksGrid.Columns.Add("Genre", "Genre");
            booksGrid.Columns.Add("Status", "Status");
            booksGrid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) EditBook(); };

            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 55 };
            var addBtn = CreateButton("Add Book", 10);
            addBtn.Click += (s, e) => AddBook();
            var editBtn = CreateButton("Edit", 130);
            editBtn.Click += (s, e) => EditBook();
            var delBtn = CreateButton("Delete", 250);
            delBtn.Click += (s, e) => DeleteBook();
            var refBtn = CreateButton("Refresh", 370);
            refBtn.Click += (s, e) => LoadBooks(bookSearch.Text);

            bottom.Controls.Add(addBtn);
            bottom.Controls.Add(editBtn);
            bottom.Controls.Add(delBtn);
            bottom.Controls.Add(refBtn);

            page.Controls.Add(booksGrid);
            page.Controls.Add(top);
            page.Controls.Add(bottom);
            return page;
        }

        private void LoadBooks(string filter = "")
        {
            booksGrid.Rows.Clear();
            var books = BookService.Search(filter);
            var lentCodes = LendingService.GetAll().Select(l => l.BookCode).ToHashSet();
            foreach (var b in books)
            {
                string status = lentCodes.Contains(b.Code) ? "Lent" : "Available";
                booksGrid.Rows.Add(b.Code, b.Title, b.Author, b.Genre, status);
            }
        }

        private void AddBook()
        {
            using var form = new BookForm();
            if (form.ShowDialog(this) == DialogResult.OK)
                LoadBooks(bookSearch.Text);
        }

        private void EditBook()
        {
            if (booksGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int code = Convert.ToInt32(booksGrid.SelectedRows[0].Cells[0].Value);
            var book = BookService.FindByCode(code);
            if (book == null) return;

            using var form = new BookForm(book);
            if (form.ShowDialog(this) == DialogResult.OK)
                LoadBooks(bookSearch.Text);
        }

        private void DeleteBook()
        {
            if (booksGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int code = Convert.ToInt32(booksGrid.SelectedRows[0].Cells[0].Value);
            var confirm = MessageBox.Show($"Delete book with code {code}?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                BookService.Delete(code);
                LoadBooks(bookSearch.Text);
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // LENDING TAB 
        private TabPage CreateLendingTab()
        {
            var page = new TabPage("Lending") { Padding = new Padding(10) };

            lendingGrid = CreateGrid();
            lendingGrid.Columns.Add("LendDate", "Lend Date");
            lendingGrid.Columns.Add("BookCode", "Book Code");
            lendingGrid.Columns.Add("BookTitle", "Book Title");
            lendingGrid.Columns.Add("MemberId", "Member ID");
            lendingGrid.Columns.Add("MemberName", "Member Name");

            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 55 };
            var lendBtn = CreateButton("Lend Book", 10);
            lendBtn.Click += (s, e) => LendBook();
            var returnBtn = CreateButton("Return Book", 130);
            returnBtn.Click += (s, e) => ReturnBook();
            var refBtn = CreateButton("Refresh", 250);
            refBtn.Click += (s, e) => LoadLendings();

            bottom.Controls.Add(lendBtn);
            bottom.Controls.Add(returnBtn);
            bottom.Controls.Add(refBtn);

            page.Controls.Add(lendingGrid);
            page.Controls.Add(bottom);
            return page;
        }

        private void LoadLendings()
        {
            lendingGrid.Rows.Clear();
            var lendings = LendingService.GetAll();
            var books = BookService.GetAll().ToDictionary(b => b.Code, b => b.Title);
            var members = MemberService.GetAll()
                .ToDictionary(m => m.Id, m => $"{m.FirstName} {m.LastName}");

            foreach (var l in lendings)
            {
                string title = books.ContainsKey(l.BookCode) ? books[l.BookCode] : "(unknown)";
                string name = members.ContainsKey(l.MemberId) ? members[l.MemberId] : "(unknown)";
                lendingGrid.Rows.Add(l.LendDate.ToString("yyyy/MM/dd"), l.BookCode, title, l.MemberId, name);
            }
        }

        private void LendBook()
        {
            using var form = new LendForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                LoadLendings();
                LoadBooks(bookSearch.Text);
            }
        }

        private void ReturnBook()
        {
            if (lendingGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a lending record first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int bookCode = Convert.ToInt32(lendingGrid.SelectedRows[0].Cells[1].Value);
            var confirm = MessageBox.Show($"Return book with code {bookCode}?", "Confirm Return",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                LendingService.Return(bookCode);
                LoadLendings();
                LoadBooks(bookSearch.Text);
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAllData()
        {
            LoadMembers();
            LoadBooks();
            LoadLendings();
        }
    }
}