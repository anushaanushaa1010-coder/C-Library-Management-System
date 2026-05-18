using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Library_Management_System
{
    public class LibrarianProfile : Form
    {
        // ── private fields ───────────────────────────────────────────────────
        private int librarianID;

        // Layout
        private TabControl tabProfile;

        // Profile Info tab
        private TabPage tabInfo;
        private Label lblUsernameValue;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtEmail;
        private TextBox txtPhoneNo;
        private Button btnSaveProfile;

        // Change Password tab
        private TabPage tabPassword;
        private TextBox txtCurrentPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnChangePassword;

        // ────────────────────────────────────────────────────────────────────
        public LibrarianProfile(int lid)
        {
            librarianID = lid;
            InitializeComponent();
        }

        // ════════════════════════════════════════════════════════════════════
        //  BUILD UI
        // ════════════════════════════════════════════════════════════════════
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // LibrarianProfile
            // 
            this.ClientSize = new System.Drawing.Size(800, 520);
            this.Name = "LibrarianProfile";
            this.Load += new System.EventHandler(this.LibrarianProfile_Load_1);

            // Top navigation bar (horizontal) so profile content sits below
            Panel nav = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppTheme.NavyMid };
            var sepNav = new Panel { Height = 2, Dock = DockStyle.Bottom, BackColor = AppTheme.Teal };
            nav.Controls.Add(sepNav);
            string[] navItems = { "Dashboard", "All Books", "Add Book", "Issue Book", "Return Book", "Students", "Requests", "Reports", "My Profile", "Logout" };
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
                // optional: navigate inside profile later
                nav.Controls.Add(b);
                nx += 148;
            }
            this.Controls.Add(nav);

            // Build and add tab control for profile content
            BuildProfileInfoTab();
            BuildChangePasswordTab();
            tabProfile = new TabControl { Dock = DockStyle.Fill, Font = AppTheme.FontBody };
            tabProfile.Controls.AddRange(new TabPage[] { tabInfo, tabPassword });
            this.Controls.Add(tabProfile);
            this.ResumeLayout(false);

        }

        // ════════════════════════════════════════════════════════════════════
        //  TAB: PROFILE INFO
        // ════════════════════════════════════════════════════════════════════
        private void BuildProfileInfoTab()
        {
            tabInfo = new TabPage("📋  Profile Info");
            tabInfo.BackColor = AppTheme.Surface;

            var card = new Panel
            {
                BackColor = AppTheme.CardBg,
                Location = new Point(24, 18),
                Size = new Size(580, 380),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lbl = new Label
            {
                Text = "Account Information",
                Font = AppTheme.FontH2,
                ForeColor = AppTheme.TextPrimary,
                AutoSize = true,
                Location = new Point(16, 14)
            };
            card.Controls.Add(lbl);

            Label ML(string t) => new Label
            {
                Text = t,
                Font = AppTheme.FontLabel,
                ForeColor = AppTheme.TextPrimary,
                AutoSize = true
            };

            int cx = 16, cy = 54, rh = 58;

            // Username (read-only)
            card.Controls.Add(ML("Username")); card.Controls[card.Controls.Count - 1].Location = new Point(cx, cy);
            lblUsernameValue = new Label
            {
                Text = "—",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.Teal,
                AutoSize = false,
                Width = 260,
                Height = 28,
                Location = new Point(cx, cy + 20),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = AppTheme.NavyMid,
                Padding = new Padding(4, 4, 0, 0)
            };
            card.Controls.Add(lblUsernameValue);

            cy += rh;
            card.Controls.Add(ML("First Name")); card.Controls[card.Controls.Count - 1].Location = new Point(cx, cy);
            txtFirstName = AppTheme.MakeInput(240, false); txtFirstName.Location = new Point(cx, cy + 18); card.Controls.Add(txtFirstName);

            card.Controls.Add(ML("Last Name")); card.Controls[card.Controls.Count - 1].Location = new Point(cx + 270, cy);
            txtLastName = AppTheme.MakeInput(240, false); txtLastName.Location = new Point(cx + 270, cy + 18); card.Controls.Add(txtLastName);

            cy += rh;
            card.Controls.Add(ML("Email Address")); card.Controls[card.Controls.Count - 1].Location = new Point(cx, cy);
            txtEmail = AppTheme.MakeInput(520, false); txtEmail.Location = new Point(cx, cy + 18); card.Controls.Add(txtEmail);

            cy += rh;
            card.Controls.Add(ML("Phone Number")); card.Controls[card.Controls.Count - 1].Location = new Point(cx, cy);
            txtPhoneNo = AppTheme.MakeInput(240, false); txtPhoneNo.Location = new Point(cx, cy + 18); card.Controls.Add(txtPhoneNo);

            cy += rh + 8;
            btnSaveProfile = AppTheme.MakeSuccessBtn("💾  Save Changes", 160, 38);
            btnSaveProfile.Location = new Point(cx, cy);
            btnSaveProfile.Click += BtnSaveProfile_Click;
            card.Controls.Add(btnSaveProfile);

            tabInfo.Controls.Add(card);
        }

        // ════════════════════════════════════════════════════════════════════
        //  TAB: CHANGE PASSWORD
        // ════════════════════════════════════════════════════════════════════
        private void BuildChangePasswordTab()
        {
            tabPassword = new TabPage("🔒  Change Password");
            tabPassword.BackColor = AppTheme.Surface;

            var card = new Panel
            {
                BackColor = AppTheme.CardBg,
                Location = new Point(24, 18),
                Size = new Size(540, 310),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lbl = new Label
            {
                Text = "Change Password",
                Font = AppTheme.FontH2,
                ForeColor = AppTheme.TextPrimary,
                AutoSize = true,
                Location = new Point(16, 14)
            };
            card.Controls.Add(lbl);

            Label ML(string t) => new Label
            {
                Text = t,
                Font = AppTheme.FontLabel,
                ForeColor = AppTheme.TextPrimary,
                AutoSize = true
            };

            int cx = 16, cy = 54, rh = 62;

            card.Controls.Add(ML("Current Password")); card.Controls[card.Controls.Count - 1].Location = new Point(cx, cy);
            txtCurrentPassword = AppTheme.MakeInput(340, true); txtCurrentPassword.Location = new Point(cx, cy + 18); card.Controls.Add(txtCurrentPassword);

            cy += rh;
            card.Controls.Add(ML("New Password (min. 6 characters)")); card.Controls[card.Controls.Count - 1].Location = new Point(cx, cy);
            txtNewPassword = AppTheme.MakeInput(340, true); txtNewPassword.Location = new Point(cx, cy + 18); card.Controls.Add(txtNewPassword);

            cy += rh;
            card.Controls.Add(ML("Confirm New Password")); card.Controls[card.Controls.Count - 1].Location = new Point(cx, cy);
            txtConfirmPassword = AppTheme.MakeInput(340, true); txtConfirmPassword.Location = new Point(cx, cy + 18); card.Controls.Add(txtConfirmPassword);

            cy += rh + 6;
            btnChangePassword = AppTheme.MakePrimaryBtn("🔒  Change Password", 180, 38);
            btnChangePassword.Location = new Point(cx, cy);
            btnChangePassword.Click += BtnChangePassword_Click;
            card.Controls.Add(btnChangePassword);

            tabPassword.Controls.Add(card);
        }

        // ════════════════════════════════════════════════════════════════════
        //  FORM LOAD
        // ════════════════════════════════════════════════════════════════════
        private void LibrarianProfile_Load(object sender, EventArgs e)
        {
            LoadProfileData();
        }

        private void LoadProfileData()
        {
            try
            {
                string sql = @"
                    SELECT Username, FirstName, ISNULL(LastName,'') AS LastName,
                           ISNULL(Email,'') AS Email, ISNULL(PhoneNo,'') AS PhoneNo
                    FROM Librarian
                    WHERE LibrarianID = @id";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql,
                    new SqlParameter("@id", librarianID));

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Librarian record not found.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow row = dt.Rows[0];
                lblUsernameValue.Text = row["Username"].ToString();
                txtFirstName.Text     = row["FirstName"].ToString();
                txtLastName.Text      = row["LastName"].ToString();
                txtEmail.Text         = row["Email"].ToString();
                txtPhoneNo.Text       = row["PhoneNo"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load profile:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  SAVE PROFILE
        // ════════════════════════════════════════════════════════════════════
        private void BtnSaveProfile_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName  = txtLastName.Text.Trim();
            string email     = txtEmail.Text.Trim();
            string phone     = txtPhoneNo.Text.Trim();

            if (string.IsNullOrWhiteSpace(firstName))
            {
                MessageBox.Show("First Name is required.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirstName.Focus();
                return;
            }

            if (!string.IsNullOrEmpty(email) && !email.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            try
            {
                string sql = @"
                    UPDATE Librarian
                    SET FirstName = @fn,
                        LastName  = @ln,
                        Email     = @em,
                        PhoneNo   = @ph
                    WHERE LibrarianID = @id";

                DatabaseHelper.ExecuteNonQuery(sql,
                    new SqlParameter("@fn", firstName),
                    new SqlParameter("@ln", (object)lastName ?? DBNull.Value),
                    new SqlParameter("@em", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email),
                    new SqlParameter("@ph", string.IsNullOrEmpty(phone) ? (object)DBNull.Value : phone),
                    new SqlParameter("@id", librarianID));

                MessageBox.Show("Profile updated successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update profile:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  CHANGE PASSWORD
        // ════════════════════════════════════════════════════════════════════
        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            string currentPwd = txtCurrentPassword.Text;
            string newPwd     = txtNewPassword.Text;
            string confirmPwd = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(currentPwd))
            {
                MessageBox.Show("Please enter your current password.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCurrentPassword.Focus();
                return;
            }

            if (string.IsNullOrEmpty(newPwd) || newPwd.Length < 6)
            {
                MessageBox.Show("New password must be at least 6 characters.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            if (newPwd != confirmPwd)
            {
                MessageBox.Show("New password and confirmation do not match.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            // Verify current password
            try
            {
                object result = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Librarian WHERE LibrarianID=@id AND Password=@pwd",
                    new SqlParameter("@id",  librarianID),
                    new SqlParameter("@pwd", currentPwd));

                int count = result != null ? Convert.ToInt32(result) : 0;

                if (count == 0)
                {
                    MessageBox.Show("Current password is incorrect.",
                        "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCurrentPassword.Focus();
                    return;
                }

                // Update password
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE Librarian SET Password=@pwd WHERE LibrarianID=@id",
                    new SqlParameter("@pwd", newPwd),
                    new SqlParameter("@id",  librarianID));

                MessageBox.Show("Password changed successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtCurrentPassword.Clear();
                txtNewPassword.Clear();
                txtConfirmPassword.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to change password:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LibrarianProfile_Load_1(object sender, EventArgs e)
        {

        }
    }
}
