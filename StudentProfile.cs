using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Library_Management_System
{
    /// <summary>
    /// Student Feature 10 + 11: View Profile & Change Password
    /// </summary>
    public class StudentProfile : Form
    {
        private int studentID;

        // Profile fields
        private TextBox txtFirstName, txtLastName, txtEmail, txtPhone, txtDept, txtAddress;
        private Label   lblEnrollment, lblSemester;
        private Button  btnSaveProfile;

        // Password change fields
        private TextBox txtCurrentPass, txtNewPass, txtConfirmPass;
        private Button  btnSavePass;

        public StudentProfile(int sid)
        {
            studentID = sid;
            InitializeComponent();
            LoadProfile();
        }

        private void InitializeComponent()
        {
            this.Text            = "My Profile";
            this.Size            = new Size(700, 640);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            AppTheme.StyleForm(this);

            // Top navigation bar (horizontal) - placed above header
            Panel nav = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppTheme.NavyMid };
            var sepNav = new Panel { Height = 2, Dock = DockStyle.Bottom, BackColor = AppTheme.Teal };
            nav.Controls.Add(sepNav);
            string[] navItems = { "Dashboard", "Search Books", "Request a Book", "My Borrowed Books", "My Requests", "My Fines", "My Profile", "Logout" };
            int nx = 8;
            foreach (var ni in navItems)
            {
                bool isLogout = ni.Contains("Logout");
                var b = new Button
                {
                    Text = ni,
                    Width = 140,
                    Height = 48,
                    Location = new Point(nx, 4),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = isLogout ? Color.FromArgb(120,30,30) : AppTheme.NavyMid,
                    ForeColor = Color.FromArgb(203,213,225),
                    Font = new Font(AppTheme.FontBody.FontFamily, 9f),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand
                };
                b.FlatAppearance.BorderSize = 0;
                b.FlatAppearance.MouseOverBackColor = isLogout ? Color.FromArgb(180,30,30) : AppTheme.NavyLight;
                nav.Controls.Add(b);
                nx += 148;
            }
            this.Controls.Add(nav);

            // Header (placed under nav) - hidden per request
            Panel header = AppTheme.MakeHeader("👤 My Profile", "View and edit your information");
            header.Dock = DockStyle.Top;
            header.Visible = false;
            this.Controls.Add(header);

            // TabControl fills remaining area under header
            TabControl tabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = AppTheme.FontBody
            };

            TabPage tabProfile = new TabPage("  📋 Profile Info  ");
            TabPage tabPass    = new TabPage("  🔒 Change Password  ");

            BuildProfileTab(tabProfile);
            BuildPasswordTab(tabPass);

            tabs.TabPages.Add(tabProfile);
            tabs.TabPages.Add(tabPass);
            AppTheme.StyleTabs(tabs);

            this.Controls.Add(tabs);
        }

        private void BuildProfileTab(TabPage tab)
        {
            tab.BackColor = AppTheme.Surface;
            Panel p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            int y = 20;

            // Enrollment (readonly)
            p.Controls.Add(MakeRow("Enrollment No.", ref y, out lblEnrollment, readOnly: true));
            // Semester (readonly)
            p.Controls.Add(MakeRow("Semester", ref y, out lblSemester, readOnly: true));

            p.Controls.Add(MakeLbl("First Name", y));
            txtFirstName = AppTheme.MakeInput(280); txtFirstName.Location = new Point(160, y); y += 42;

            p.Controls.Add(MakeLbl("Last Name", y));
            txtLastName = AppTheme.MakeInput(280); txtLastName.Location = new Point(160, y); y += 42;

            p.Controls.Add(MakeLbl("Email", y));
            txtEmail = AppTheme.MakeInput(280); txtEmail.Location = new Point(160, y); y += 42;

            p.Controls.Add(MakeLbl("Phone", y));
            txtPhone = AppTheme.MakeInput(280); txtPhone.Location = new Point(160, y); y += 42;

            p.Controls.Add(MakeLbl("Department", y));
            txtDept = AppTheme.MakeInput(280); txtDept.Location = new Point(160, y); y += 42;

            p.Controls.Add(MakeLbl("Address", y));
            txtAddress = AppTheme.MakeInput(280); txtAddress.Location = new Point(160, y); y += 50;

            btnSaveProfile = AppTheme.MakePrimaryBtn("💾  Save Profile", 200, 40);
            btnSaveProfile.Location = new Point(160, y);
            btnSaveProfile.Click   += BtnSaveProfile_Click;

            p.Controls.Add(txtFirstName);
            p.Controls.Add(txtLastName);
            p.Controls.Add(txtEmail);
            p.Controls.Add(txtPhone);
            p.Controls.Add(txtDept);
            p.Controls.Add(txtAddress);
            p.Controls.Add(btnSaveProfile);

            tab.Controls.Add(p);
        }

        private Panel MakeRow(string labelText, ref int y, out Label displayLabel, bool readOnly = false)
        {
            Label lbl = MakeLbl(labelText, y);
            displayLabel = new Label
            {
                Font = AppTheme.FontInput, ForeColor = AppTheme.NavyDark,
                Location = new Point(160, y + 2), AutoSize = true
            };
            var container = new Panel { Location = new Point(0, 0), Size = new Size(600, 1) };
            container.Controls.Add(lbl);
            container.Controls.Add(displayLabel);
            y += 36;
            return container;
        }

        private Label MakeLbl(string text, int y)
        {
            return new Label
            {
                Text = text, Font = AppTheme.FontLabel, ForeColor = AppTheme.TextMuted,
                Location = new Point(20, y + 4), AutoSize = true
            };
        }

        private void BuildPasswordTab(TabPage tab)
        {
            tab.BackColor = AppTheme.Surface;
            Panel p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            int y = 30;

            p.Controls.Add(MakeLbl2("Current Password", y));
            txtCurrentPass = AppTheme.MakeInput(300, true); txtCurrentPass.Location = new Point(200, y); y += 50;

            p.Controls.Add(MakeLbl2("New Password", y));
            txtNewPass = AppTheme.MakeInput(300, true); txtNewPass.Location = new Point(200, y); y += 50;

            p.Controls.Add(MakeLbl2("Confirm New Password", y));
            txtConfirmPass = AppTheme.MakeInput(300, true); txtConfirmPass.Location = new Point(200, y); y += 60;

            btnSavePass = AppTheme.MakePrimaryBtn("🔐  Update Password", 220, 42);
            btnSavePass.Location = new Point(200, y);
            btnSavePass.Click   += BtnSavePass_Click;

            p.Controls.Add(txtCurrentPass);
            p.Controls.Add(txtNewPass);
            p.Controls.Add(txtConfirmPass);
            p.Controls.Add(btnSavePass);

            // Password rules hint
            Label hint = new Label
            {
                Text = "Password must be at least 6 characters.",
                Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted,
                Location = new Point(200, y + 55), AutoSize = true
            };
            p.Controls.Add(hint);

            tab.Controls.Add(p);
        }

        private Label MakeLbl2(string text, int y)
        {
            return new Label
            {
                Text = text, Font = AppTheme.FontLabel, ForeColor = AppTheme.NavyDark,
                Location = new Point(30, y + 4), AutoSize = true
            };
        }

        private void LoadProfile()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT * FROM Student WHERE StudentID=@id",
                new SqlParameter[] { new SqlParameter("@id", studentID) });

            if (dt == null || dt.Rows.Count == 0) return;
            DataRow r = dt.Rows[0];

            if (lblEnrollment != null) lblEnrollment.Text = r["EnrollmentNo"].ToString();
            if (lblSemester   != null) lblSemester.Text   = "Semester " + r["Semester"].ToString();

            txtFirstName.Text = r["FirstName"].ToString();
            txtLastName.Text  = r["LastName"].ToString();
            txtEmail.Text     = r["Email"].ToString();
            txtPhone.Text     = r["PhoneNo"].ToString();
            txtDept.Text      = r["Department"].ToString();
            txtAddress.Text   = r["Address"].ToString();
        }

        private void BtnSaveProfile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("First name cannot be empty.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool ok = DatabaseHelper.ExecuteNonQuery(
                "UPDATE Student SET FirstName=@fn, LastName=@ln, Email=@em, PhoneNo=@ph, Department=@dep, Address=@addr WHERE StudentID=@id",
                new SqlParameter[] {
                    new SqlParameter("@fn",   txtFirstName.Text.Trim()),
                    new SqlParameter("@ln",   txtLastName.Text.Trim()),
                    new SqlParameter("@em",   txtEmail.Text.Trim()),
                    new SqlParameter("@ph",   txtPhone.Text.Trim()),
                    new SqlParameter("@dep",  txtDept.Text.Trim()),
                    new SqlParameter("@addr", txtAddress.Text.Trim()),
                    new SqlParameter("@id",   studentID)
                });

            if (ok) MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSavePass_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCurrentPass.Text) || string.IsNullOrEmpty(txtNewPass.Text))
            {
                MessageBox.Show("Please fill all password fields.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtNewPass.Text != txtConfirmPass.Text)
            {
                MessageBox.Show("New password and confirm password do not match.", "Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtNewPass.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.", "Too Short", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verify current password
            object result = DatabaseHelper.ExecuteScalar(
                "SELECT StudentID FROM Student WHERE StudentID=@id AND Password=@p",
                new SqlParameter[] { new SqlParameter("@id", studentID), new SqlParameter("@p", txtCurrentPass.Text) });

            if (result == null)
            {
                MessageBox.Show("Current password is incorrect.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool ok = DatabaseHelper.ExecuteNonQuery(
                "UPDATE Student SET Password=@p WHERE StudentID=@id",
                new SqlParameter[] { new SqlParameter("@p", txtNewPass.Text), new SqlParameter("@id", studentID) });

            if (ok)
            {
                MessageBox.Show("Password changed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtCurrentPass.Clear(); txtNewPass.Clear(); txtConfirmPass.Clear();
            }
        }
    }
}
