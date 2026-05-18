using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Library_Management_System
{
    public class AddStudent : Form
    {
        private Panel    pnlLeft, pnlRight;
        private TextBox  txtFirstName, txtLastName;
        private TextBox  txtEnrollment, txtDepartment;
        private TextBox  txtEmail, txtPhone;
        private ComboBox cmbSemester;
        private TextBox  txtAddress;
        private TextBox  txtPassword, txtConfirmPassword;
        private Button   btnRegister, btnClear;

        public AddStudent()
        {
            InitializeComponent();
        }

        private Image LoadPicture(string filename)
        {
            string[] attempts = {
                Path.Combine(Application.StartupPath, "picture", filename),
                Path.Combine(Application.StartupPath, @"..\..", "picture", filename),
                Path.Combine(Application.StartupPath, @"..\..\..", "picture", filename),
            };
            foreach (string p in attempts)
                if (File.Exists(p)) return Image.FromFile(Path.GetFullPath(p));
            return null;
        }

        private void InitializeComponent()
        {
            this.Text            = "Student Registration";
            this.Size            = new Size(900, 620);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.BackColor       = AppTheme.NavyDark;

            BuildLeftPanel();
            BuildRightPanel();
        }

        private void BuildLeftPanel()
        {
            pnlLeft = new Panel
            {
                Size      = new Size(320, 620),
                Location  = new Point(0, 0),
                BackColor = AppTheme.NavyDark
            };

            Image img = LoadPicture("aiub_library1.jpg");
            if (img != null)
            {
                PictureBox pb = new PictureBox
                {
                    Size     = new Size(320, 620),
                    Location = new Point(0, 0),
                    Image    = img,
                    SizeMode = PictureBoxSizeMode.StretchImage
                };

                // Overlay panel at bottom with registration tips
                Panel overlay = new Panel
                {
                    Size      = new Size(320, 150),
                    Location  = new Point(0, 470),
                    BackColor = Color.FromArgb(180, 10, 15, 40)
                };

                string tips = "Registration Tips:\n• Use your official AIUB email\n• Keep password 6+ characters\n• Remember your Enrollment No.";
                Label lblTips = new Label
                {
                    Text      = tips,
                    Font      = new Font("Segoe UI", 8.5f),
                    ForeColor = Color.White,
                    Location  = new Point(12, 10),
                    Size      = new Size(296, 130)
                };

                overlay.Controls.Add(lblTips);
                pb.Controls.Add(overlay);
                pnlLeft.Controls.Add(pb);
            }
            else
            {
                // Fallback: NavyDark background with emoji
                pnlLeft.BackColor = AppTheme.NavyDark;

                Label lblIcon = new Label
                {
                    Text      = "📚",
                    Font      = new Font("Segoe UI Emoji", 52f),
                    ForeColor = AppTheme.Teal,
                    AutoSize  = true,
                    Location  = new Point(100, 160)
                };

                Label lblTitle = new Label
                {
                    Text      = "Student",
                    Font      = new Font("Segoe UI", 20f, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize  = true,
                    Location  = new Point(85, 255)
                };

                Label lblTitle2 = new Label
                {
                    Text      = "Registration",
                    Font      = new Font("Segoe UI", 14f),
                    ForeColor = AppTheme.TealLight,
                    AutoSize  = true,
                    Location  = new Point(80, 295)
                };

                // Tips panel at bottom
                Panel tipsPanel = new Panel
                {
                    Size      = new Size(320, 150),
                    Location  = new Point(0, 470),
                    BackColor = Color.FromArgb(60, AppTheme.Teal)
                };

                string tips = "Registration Tips:\n• Use your official AIUB email\n• Keep password 6+ characters\n• Remember your Enrollment No.";
                Label lblTips = new Label
                {
                    Text      = tips,
                    Font      = new Font("Segoe UI", 8.5f),
                    ForeColor = Color.White,
                    Location  = new Point(12, 10),
                    Size      = new Size(296, 130)
                };

                tipsPanel.Controls.Add(lblTips);
                pnlLeft.Controls.Add(lblIcon);
                pnlLeft.Controls.Add(lblTitle);
                pnlLeft.Controls.Add(lblTitle2);
                pnlLeft.Controls.Add(tipsPanel);
            }

            this.Controls.Add(pnlLeft);
        }

        private void BuildRightPanel()
        {
            pnlRight = new Panel
            {
                Size      = new Size(580, 620),
                Location  = new Point(320, 0),
                BackColor = AppTheme.Surface
            };

            // Top 5px teal accent bar
            Panel topAccent = new Panel
            {
                Size      = new Size(580, 5),
                Location  = new Point(0, 0),
                BackColor = AppTheme.Teal
            };

            Label lblHeading = new Label
            {
                Text      = "📝 Student Registration",
                Font      = AppTheme.FontH2,
                ForeColor = AppTheme.NavyDark,
                AutoSize  = true,
                Location  = new Point(30, 22)
            };

            Label lblSubtitle = new Label
            {
                Text      = "Create your library account",
                Font      = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                AutoSize  = true,
                Location  = new Point(33, 58)
            };

            Label lblRequired = new Label
            {
                Text      = "* = Required field",
                Font      = new Font("Segoe UI", 7.5f, FontStyle.Italic),
                ForeColor = AppTheme.TextMuted,
                AutoSize  = true,
                Location  = new Point(33, 76)
            };

            // ---- Row 1: First Name | Last Name ----
            int col1X = 30, col2X = 300;
            int rowY  = 100;
            int fieldW = 220;
            int fieldH = 26;
            int rowGap = 68;

            pnlRight.Controls.Add(MakeLabel("First Name *", col1X, rowY));
            txtFirstName          = MakeTextBox(fieldW, fieldH);
            txtFirstName.Location = new Point(col1X, rowY + 20);

            pnlRight.Controls.Add(MakeLabel("Last Name", col2X, rowY));
            txtLastName          = MakeTextBox(fieldW, fieldH);
            txtLastName.Location = new Point(col2X, rowY + 20);

            rowY += rowGap;

            // ---- Row 2: Enrollment No | Department ----
            pnlRight.Controls.Add(MakeLabel("Enrollment No. *", col1X, rowY));
            txtEnrollment          = MakeTextBox(fieldW, fieldH);
            txtEnrollment.Location = new Point(col1X, rowY + 20);

            pnlRight.Controls.Add(MakeLabel("Department", col2X, rowY));
            txtDepartment          = MakeTextBox(fieldW, fieldH);
            txtDepartment.Location = new Point(col2X, rowY + 20);

            rowY += rowGap;

            // ---- Row 3: Email | Phone ----
            pnlRight.Controls.Add(MakeLabel("Email", col1X, rowY));
            txtEmail          = MakeTextBox(fieldW, fieldH);
            txtEmail.Location = new Point(col1X, rowY + 20);

            pnlRight.Controls.Add(MakeLabel("Phone", col2X, rowY));
            txtPhone          = MakeTextBox(fieldW, fieldH);
            txtPhone.Location = new Point(col2X, rowY + 20);

            rowY += rowGap;

            // ---- Row 4: Semester | Address ----
            pnlRight.Controls.Add(MakeLabel("Semester", col1X, rowY));
            cmbSemester = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = AppTheme.FontInput,
                Location      = new Point(col1X, rowY + 20),
                Width         = fieldW
            };
            cmbSemester.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8" });

            pnlRight.Controls.Add(MakeLabel("Address", col2X, rowY));
            txtAddress          = MakeTextBox(fieldW, fieldH);
            txtAddress.Location = new Point(col2X, rowY + 20);

            rowY += rowGap;

            // ---- Row 5: Password | Confirm Password ----
            pnlRight.Controls.Add(MakeLabel("Password *", col1X, rowY));
            txtPassword = new TextBox
            {
                Font         = AppTheme.FontInput,
                PasswordChar = '●',
                Size         = new Size(fieldW, fieldH),
                Location     = new Point(col1X, rowY + 20)
            };

            pnlRight.Controls.Add(MakeLabel("Confirm Password *", col2X, rowY));
            txtConfirmPassword = new TextBox
            {
                Font         = AppTheme.FontInput,
                PasswordChar = '●',
                Size         = new Size(fieldW, fieldH),
                Location     = new Point(col2X, rowY + 20)
            };

            rowY += rowGap + 8;

            // ---- Buttons ----
            btnRegister          = AppTheme.MakePrimaryBtn("Register", 410, 42);
            btnRegister.Location = new Point(col1X, rowY);
            btnRegister.Click   += BtnRegister_Click;

            btnClear          = AppTheme.MakePrimaryBtn("Clear", 130, 42);
            btnClear.BackColor = AppTheme.NavyMid;
            btnClear.Location  = new Point(col1X + 420, rowY);
            btnClear.Click    += (s, e) => ClearFields();

            // Add all controls to right panel
            pnlRight.Controls.Add(topAccent);
            pnlRight.Controls.Add(lblHeading);
            pnlRight.Controls.Add(lblSubtitle);
            pnlRight.Controls.Add(lblRequired);

            pnlRight.Controls.Add(txtFirstName);
            pnlRight.Controls.Add(txtLastName);
            pnlRight.Controls.Add(txtEnrollment);
            pnlRight.Controls.Add(txtDepartment);
            pnlRight.Controls.Add(txtEmail);
            pnlRight.Controls.Add(txtPhone);
            pnlRight.Controls.Add(cmbSemester);
            pnlRight.Controls.Add(txtAddress);
            pnlRight.Controls.Add(txtPassword);
            pnlRight.Controls.Add(txtConfirmPassword);
            pnlRight.Controls.Add(btnRegister);
            pnlRight.Controls.Add(btnClear);

            this.Controls.Add(pnlRight);
        }

        private Label MakeLabel(string text, int x, int y)
        {
            return new Label
            {
                Text      = text,
                Font      = AppTheme.FontLabel,
                ForeColor = AppTheme.NavyDark,
                AutoSize  = true,
                Location  = new Point(x, y)
            };
        }

        private TextBox MakeTextBox(int width, int height)
        {
            return new TextBox
            {
                Font = AppTheme.FontInput,
                Size = new Size(width, height)
            };
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string firstName   = txtFirstName.Text.Trim();
            string lastName    = txtLastName.Text.Trim();
            string enrollment  = txtEnrollment.Text.Trim();
            string department  = txtDepartment.Text.Trim();
            string email       = txtEmail.Text.Trim();
            string phone       = txtPhone.Text.Trim();
            string semester    = cmbSemester.SelectedItem?.ToString() ?? "";
            string address     = txtAddress.Text.Trim();
            string password    = txtPassword.Text;
            string confirmPwd  = txtConfirmPassword.Text;

            // Validation
            if (string.IsNullOrWhiteSpace(firstName))
            {
                MessageBox.Show("First Name is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirstName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(enrollment))
            {
                MessageBox.Show("Enrollment No. is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEnrollment.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Password is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (password != confirmPwd)
            {
                MessageBox.Show("Password and Confirm Password do not match.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            try
            {
                // Check for duplicate enrollment
                object countObj = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Student WHERE EnrollmentNo=@e",
                    new SqlParameter[] { new SqlParameter("@e", enrollment) });

                int count = countObj != null ? Convert.ToInt32(countObj) : 0;
                if (count > 0)
                {
                    MessageBox.Show("A student with this Enrollment No. already exists.", "Duplicate Entry",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtEnrollment.Focus();
                    return;
                }

                // Insert student
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@e",    enrollment),
                    new SqlParameter("@fn",   firstName),
                    new SqlParameter("@ln",   string.IsNullOrEmpty(lastName)   ? (object)DBNull.Value : lastName),
                    new SqlParameter("@em",   string.IsNullOrEmpty(email)      ? (object)DBNull.Value : email),
                    new SqlParameter("@ph",   string.IsNullOrEmpty(phone)      ? (object)DBNull.Value : phone),
                    new SqlParameter("@dep",  string.IsNullOrEmpty(department) ? (object)DBNull.Value : department),
                    new SqlParameter("@sem",  string.IsNullOrEmpty(semester)   ? (object)DBNull.Value : (object)Convert.ToInt32(semester)),
                    new SqlParameter("@addr", string.IsNullOrEmpty(address)    ? (object)DBNull.Value : address),
                    new SqlParameter("@pwd",  password)
                };

                DatabaseHelper.ExecuteNonQuery(
                    "INSERT INTO Student (EnrollmentNo, FirstName, LastName, Email, PhoneNo, Department, Semester, Address, Password, IsActive) " +
                    "VALUES (@e, @fn, @ln, @em, @ph, @dep, @sem, @addr, @pwd, 1)",
                    parameters);

                MessageBox.Show("✅ Registration successful! You can now login.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Registration error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtEnrollment.Clear();
            txtDepartment.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            cmbSemester.SelectedIndex = -1;
            txtFirstName.Focus();
        }
    }
}
