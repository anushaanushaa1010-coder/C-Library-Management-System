using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Library_Management_System
{
    public class AdminProfile : Form
    {
        // ── private fields ────────────────────────────────────────────────────
        private int adminID;

        // Layout
        private TabControl tabProfile;

        // Profile Info tab
        private TabPage tabInfo;
        private Label lblUsernameValue;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtEmail;
        private Button btnSaveProfile;

        // Change Password tab
        private TabPage tabPassword;
        private TextBox txtCurrentPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnChangePassword;

        // ────────────────────────────────────────────────────────────────────
        public AdminProfile(int aid)
        {
            adminID = aid;
            InitializeComponent();
        }

        // ════════════════════════════════════════════════════════════════════
        //  BUILD UI
        // ════════════════════════════════════════════════════════════════════
        private void InitializeComponent()
        {
            AppTheme.StyleForm(this);
            this.Text = "Admin Profile";
            this.Size = new Size(620, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // ── Header ───────────────────────────────────────────────────────
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = AppTheme.NavyDark
            };

            var pnlAccent = new Panel
            {
                Dock = DockStyle.Left,
                Width = 6,
                BackColor = AppTheme.Teal
            };
            pnlHeader.Controls.Add(pnlAccent);

            var lblTitle = new Label
            {
                Text = "⚙️  Admin Profile",
                Font = AppTheme.FontTitle,
                ForeColor = AppTheme.TealLight,
                AutoSize = false,
                Width = 500,
                Height = 30,
                Location = new Point(18, 8)
            };
            pnlHeader.Controls.Add(lblTitle);

            var lblSub = new Label
            {
                Text = "Manage your account information",
                Font = AppTheme.FontH3,
                ForeColor = AppTheme.Teal,
                AutoSize = false,
                Width = 500,
                Height = 20,
                Location = new Point(20, 40)
            };
            pnlHeader.Controls.Add(lblSub);

            // ── TabControl ───────────────────────────────────────────────────
            tabProfile = new TabControl
            {
                Dock = DockStyle.Fill,
                Padding = new Point(14, 6)
            };
            AppTheme.StyleTabs(tabProfile);

            BuildProfileInfoTab();
            BuildChangePasswordTab();

            tabProfile.Controls.Add(tabInfo);
            tabProfile.Controls.Add(tabPassword);

            this.Controls.Add(tabProfile);
            this.Controls.Add(pnlHeader);

            this.Load += AdminProfile_Load;
        }

        // ════════════════════════════════════════════════════════════════════
        //  TAB: PROFILE INFO
        // ════════════════════════════════════════════════════════════════════
        private void BuildProfileInfoTab()
        {
            tabInfo = new TabPage("📋  Profile");
            tabInfo.BackColor = AppTheme.Surface;

            var card = new Panel
            {
                BackColor = AppTheme.CardBg,
                Location = new Point(24, 18),
                Size = new Size(554, 330),
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

            // Username — read only
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
                BackColor = AppTheme.NavyLight,
                Padding = new Padding(4, 4, 0, 0)
            };
            card.Controls.Add(lblUsernameValue);

            cy += rh;
            card.Controls.Add(ML("First Name")); card.Controls[card.Controls.Count - 1].Location = new Point(cx, cy);
            txtFirstName = AppTheme.MakeInput(240); txtFirstName.Location = new Point(cx, cy + 18); card.Controls.Add(txtFirstName);

            card.Controls.Add(ML("Last Name")); card.Controls[card.Controls.Count - 1].Location = new Point(cx + 260, cy);
            txtLastName = AppTheme.MakeInput(240); txtLastName.Location = new Point(cx + 260, cy + 18); card.Controls.Add(txtLastName);

            cy += rh;
            card.Controls.Add(ML("Email Address")); card.Controls[card.Controls.Count - 1].Location = new Point(cx, cy);
            txtEmail = AppTheme.MakeInput(500); txtEmail.Location = new Point(cx, cy + 18); card.Controls.Add(txtEmail);

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
                Size = new Size(554, 300),
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
        private void AdminProfile_Load(object sender, EventArgs e)
        {
            LoadProfileData();
        }

        private void LoadProfileData()
        {
            try
            {
                string sql = "SELECT Username, FirstName, ISNULL(LastName,'') AS LastName, ISNULL(Email,'') AS Email " +
                             "FROM Admin WHERE AdminID=@id";
                DataTable dt = DatabaseHelper.ExecuteQuery(sql, new SqlParameter("@id", adminID));

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("Admin record not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow row = dt.Rows[0];
                lblUsernameValue.Text = row["Username"].ToString();
                txtFirstName.Text     = row["FirstName"].ToString();
                txtLastName.Text      = row["LastName"].ToString();
                txtEmail.Text         = row["Email"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load profile:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (string.IsNullOrWhiteSpace(firstName))
            {
                MessageBox.Show("First Name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirstName.Focus();
                return;
            }
            if (!string.IsNullOrEmpty(email) && !email.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            try
            {
                string sql = "UPDATE Admin SET FirstName=@fn, LastName=@ln, Email=@em WHERE AdminID=@id";
                DatabaseHelper.ExecuteNonQuery(sql,
                    new SqlParameter("@fn", firstName),
                    new SqlParameter("@ln", string.IsNullOrEmpty(lastName) ? (object)DBNull.Value : lastName),
                    new SqlParameter("@em", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email),
                    new SqlParameter("@id", adminID));

                MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update profile:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Please enter your current password.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCurrentPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(newPwd) || newPwd.Length < 6)
            {
                MessageBox.Show("New password must be at least 6 characters.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }
            if (newPwd != confirmPwd)
            {
                MessageBox.Show("New password and confirmation do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            try
            {
                // Verify current password
                object result = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Admin WHERE AdminID=@id AND Password=@pwd",
                    new SqlParameter("@id",  adminID),
                    new SqlParameter("@pwd", currentPwd));

                int count = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                if (count == 0)
                {
                    MessageBox.Show("Current password is incorrect.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCurrentPassword.Focus();
                    return;
                }

                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE Admin SET Password=@p WHERE AdminID=@id",
                    new SqlParameter("@p",  newPwd),
                    new SqlParameter("@id", adminID));

                MessageBox.Show("Password changed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtCurrentPassword.Clear();
                txtNewPassword.Clear();
                txtConfirmPassword.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to change password:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
