using Library_Management_System.Exceptions;
using Library_Management_System.Models;
using Library_Management_System.Services;

namespace Library_Management_System.Forms
{
    public class MemberForm : Form
    {
        private TextBox idText = null!;
        private TextBox firstNameText = null!;
        private TextBox lastNameText = null!;
        private ComboBox membershipCombo = null!;

        private readonly Member? existingMember;

        public MemberForm(Member? member = null)
        {
            existingMember = member;
            InitializeUI();

            if (member != null)
            {
                Text = "Edit Member";
                idText.Text = member.Id.ToString();
                firstNameText.Text = member.FirstName;
                lastNameText.Text = member.LastName;
                membershipCombo.SelectedItem = member.MembershipType;
            }
            else
            {
                Text = "Add Member";
                membershipCombo.SelectedIndex = 0;
            }
        }

        private void InitializeUI()
        {
            Width = 400;
            Height = 280;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9F);

            var lblId = new Label { Text = "ID:", Left = 20, Top = 25, Width = 100 };
            idText = new TextBox { Left = 130, Top = 22, Width = 220 };

            var lblFirst = new Label { Text = "First Name:", Left = 20, Top = 65, Width = 100 };
            firstNameText = new TextBox { Left = 130, Top = 62, Width = 220 };

            var lblLast = new Label { Text = "Last Name:", Left = 20, Top = 105, Width = 100 };
            lastNameText = new TextBox { Left = 130, Top = 102, Width = 220 };

            var lblType = new Label { Text = "Membership Type:", Left = 20, Top = 145, Width = 110 };
            membershipCombo = new ComboBox
            {
                Left = 130,
                Top = 142,
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            membershipCombo.Items.AddRange(new object[] { "Standard", "Premium", "Student" });

            var saveBtn = new Button { Text = "Save", Left = 130, Top = 195, Width = 100, Height = 30 };
            saveBtn.Click += SaveBtn_Click;

            var cancelBtn = new Button { Text = "Cancel", Left = 250, Top = 195, Width = 100, Height = 30 };
            cancelBtn.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(lblId);
            Controls.Add(idText);
            Controls.Add(lblFirst);
            Controls.Add(firstNameText);
            Controls.Add(lblLast);
            Controls.Add(lastNameText);
            Controls.Add(lblType);
            Controls.Add(membershipCombo);
            Controls.Add(saveBtn);
            Controls.Add(cancelBtn);

            AcceptButton = saveBtn;
            CancelButton = cancelBtn;
        }

        private void SaveBtn_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(idText.Text.Trim(), out int id))
            {
                MessageBox.Show("ID must be a valid number.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(firstNameText.Text) ||
                string.IsNullOrWhiteSpace(lastNameText.Text))
            {
                MessageBox.Show("First name and last name are required.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (membershipCombo.SelectedItem == null)
            {
                MessageBox.Show("Please select a membership type.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var member = new Member(
                id,
                firstNameText.Text.Trim(),
                lastNameText.Text.Trim(),
                membershipCombo.SelectedItem.ToString()!
            );

            try
            {
                if (existingMember == null)
                    MemberService.Add(member);
                else
                    MemberService.Update(existingMember.Id, member);

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