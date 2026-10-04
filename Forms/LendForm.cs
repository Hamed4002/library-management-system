using Library_Management_System.Exceptions;
using Library_Management_System.Services;

namespace Library_Management_System.Forms
{
    public class LendForm : Form
    {
        private ComboBox bookCombo = null!;
        private ComboBox memberCombo = null!;
        private Label selectedInfoLabel = null!;

        public LendForm()
        {
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            Text = "Lend Book";
            Width = 500;
            Height = 300;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F);

            var lblBook = new Label { Text = "Book:", Left = 20, Top = 25, Width = 110 };
            bookCombo = new ComboBox
            {
                Left = 140,
                Top = 22,
                Width = 320,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            bookCombo.SelectedIndexChanged += UpdateSelectedInfo;

            var lblMember = new Label { Text = "Member:", Left = 20, Top = 70, Width = 110 };
            memberCombo = new ComboBox
            {
                Left = 140,
                Top = 67,
                Width = 320,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            memberCombo.SelectedIndexChanged += UpdateSelectedInfo;

            selectedInfoLabel = new Label
            {
                Left = 20,
                Top = 110,
                Width = 440,
                Height = 55,
                ForeColor = Color.DimGray,
                Text = "Lending:\n  Book: (not selected)\n  To:   (not selected)"
            };

            var lendBtn = new Button { Text = "Lend", Left = 140, Top = 180, Width = 145, Height = 32 };
            lendBtn.Click += LendBtn_Click;

            var cancelBtn = new Button { Text = "Cancel", Left = 315, Top = 180, Width = 145, Height = 32 };
            cancelBtn.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(lblBook);
            Controls.Add(bookCombo);
            Controls.Add(lblMember);
            Controls.Add(memberCombo);
            Controls.Add(selectedInfoLabel);
            Controls.Add(lendBtn);
            Controls.Add(cancelBtn);

            AcceptButton = lendBtn;
            CancelButton = cancelBtn;
        }

        private void LoadData()
        {
            var lentCodes = LendingService.GetAll().Select(l => l.BookCode).ToHashSet();
            var availableBooks = BookService.GetAll()
                .Where(b => !lentCodes.Contains(b.Code))
                .ToList();

            if (availableBooks.Count == 0)
            {
                MessageBox.Show("There are no available books to lend.", "No Available Books",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            var members = MemberService.GetAll();
            if (members.Count == 0)
            {
                MessageBox.Show("There are no members to lend to.", "No Members",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            // Books: placeholder first, then available books
            bookCombo.Items.Clear();
            bookCombo.Items.Add(new ComboItem(-1, "-- Select a book --"));
            foreach (var b in availableBooks)
                bookCombo.Items.Add(new ComboItem(b.Code, $"{b.Code} - {b.Title}"));
            bookCombo.SelectedIndex = 0;

            // Members: placeholder first, then all members
            memberCombo.Items.Clear();
            memberCombo.Items.Add(new ComboItem(-1, "-- Select a member --"));
            foreach (var m in members)
                memberCombo.Items.Add(new ComboItem(m.Id, $"{m.Id} - {m.FirstName} {m.LastName}"));
            memberCombo.SelectedIndex = 0;
        }

        private void UpdateSelectedInfo(object? sender, EventArgs e)
        {
            string book = bookCombo.SelectedItem is ComboItem bi && bi.Id != -1
                ? bi.Display
                : "(not selected)";

            string member = memberCombo.SelectedItem is ComboItem mi && mi.Id != -1
                ? mi.Display
                : "(not selected)";

            selectedInfoLabel.Text = $"Lending:\n  Book: {book}\n  To:   {member}";
        }

        private void LendBtn_Click(object? sender, EventArgs e)
        {
            if (bookCombo.SelectedItem is not ComboItem bookItem || bookItem.Id == -1)
            {
                MessageBox.Show("Please select a book.", "Invalid Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (memberCombo.SelectedItem is not ComboItem memberItem || memberItem.Id == -1)
            {
                MessageBox.Show("Please select a member.", "Invalid Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                LendingService.Lend(bookItem.Id, memberItem.Id);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        private class ComboItem
        {
            public int Id { get; }
            public string Display { get; }

            public ComboItem(int id, string display)
            {
                Id = id;
                Display = display;
            }

            public override string ToString() => Display;
        }
    }
}