using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Library_Management_System
{
    public class LoginForm : Form
    {
        private Panel    pnlLeft, pnlRight;
        private Label    lblWelcome, lblSub;
        private Label    lblUserType, lblUsername, lblPassword;
        private ComboBox cmbUserType;
        private TextBox  txtUsername, txtPassword;
        private Button   btnLogin, btnRegister, btnExit;

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private CheckBox chkShowPass;

        public LoginForm()
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
            this.SuspendLayout();
            // 
            // LoginForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(904, 541);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AIUB Library Management System";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);

            BuildLeftPanel();
            BuildRightPanel();
        }

        private void BuildLeftPanel()
        {
            pnlLeft = new Panel
            {
                Size      = new Size(380, 580),
                Location  = new Point(0, 0),
                BackColor = AppTheme.NavyDark,
                Anchor    = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom
            };

            Image img = LoadPicture("aiub_library.jpg");
            if (img != null)
            {
                // Use a Panel with BackgroundImage instead of PictureBox with child controls
                Panel imgPanel = new Panel
                {
                    Size              = new Size(380, 580),
                    Location          = new Point(0, 0),
                    BackgroundImage   = img,
                    BackgroundImageLayout = ImageLayout.Stretch
                };

                // Semi-transparent overlay at bottom
                Panel overlay = new Panel
                {
                    Size      = new Size(380, 80),
                    Location  = new Point(0, 500),
                    BackColor = Color.FromArgb(170, 10, 15, 40)
                };

                Label lblOverlayText = new Label
                {
                    Text      = "AIUB Library Management System",
                    Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock      = DockStyle.Fill
                };

                overlay.Controls.Add(lblOverlayText);
                imgPanel.Controls.Add(overlay);
                pnlLeft.Controls.Add(imgPanel);
            }
            else
            {
                // Fallback: NavyDark background with emoji icon
                pnlLeft.BackColor = AppTheme.NavyDark;

                Label lblIcon = new Label
                {
                    Text      = "📚",
                    Font      = new Font("Segoe UI Emoji", 52f),
                    ForeColor = AppTheme.Teal,
                    AutoSize  = true,
                    Location  = new Point(130, 160)
                };

                Label lblAppName = new Label
                {
                    Text      = "AIUB Library",
                    Font      = new Font("Segoe UI", 22f, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize  = true,
                    Location  = new Point(75, 265)
                };

                Label lblTagline = new Label
                {
                    Text      = "Management System",
                    Font      = new Font("Segoe UI", 12f),
                    ForeColor = AppTheme.TealLight,
                    AutoSize  = true,
                    Location  = new Point(90, 305)
                };

                pnlLeft.Controls.Add(lblIcon);
                pnlLeft.Controls.Add(lblAppName);
                pnlLeft.Controls.Add(lblTagline);
            }

            this.Controls.Add(pnlLeft);
        }

        private void BuildRightPanel()
        {
            pnlRight = new Panel
            {
                Size      = new Size(540, 580),
                Location  = new Point(380, 0),
                BackColor = AppTheme.Surface,
                Anchor    = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };

            // Top 5px teal accent bar
            Panel topAccent = new Panel
            {
                Size      = new Size(540, 5),
                Location  = new Point(0, 0),
                BackColor = AppTheme.Teal
            };

            lblWelcome = new Label
            {
                Text      = "Welcome Back!",
                Font      = AppTheme.FontH2,
                ForeColor = AppTheme.NavyDark,
                AutoSize  = true,
                Location  = new Point(60, 55)
            };

            lblSub = new Label
            {
                Text      = "Sign in to continue",
                Font      = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                AutoSize  = true,
                Location  = new Point(62, 95)
            };

            lblUserType = new Label
            {
                Text      = "Sign in as:",
                Font      = AppTheme.FontLabel,
                ForeColor = AppTheme.NavyDark,
                AutoSize  = true,
                Location  = new Point(60, 140)
            };

            cmbUserType = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = AppTheme.FontInput,
                Location      = new Point(60, 162),
                Width         = 420
            };
            cmbUserType.Items.AddRange(new object[] { "Student", "Librarian", "Admin" });
            cmbUserType.SelectedIndex = 0;

            lblUsername = new Label
            {
                Text      = "Username / Enrollment:",
                Font      = AppTheme.FontLabel,
                ForeColor = AppTheme.NavyDark,
                AutoSize  = true,
                Location  = new Point(60, 210)
            };

            txtUsername          = AppTheme.MakeInput(420);
            txtUsername.Location = new Point(60, 232);

            lblPassword = new Label
            {
                Text      = "Password:",
                Font      = AppTheme.FontLabel,
                ForeColor = AppTheme.NavyDark,
                AutoSize  = true,
                Location  = new Point(60, 280)
            };

            txtPassword          = AppTheme.MakeInput(420, isPassword: true);
            txtPassword.Location = new Point(60, 302);

            chkShowPass = new CheckBox
            {
                Text      = "Show password",
                Font      = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                AutoSize  = true,
                Location  = new Point(62, 340),
                Cursor    = Cursors.Hand
            };
            chkShowPass.CheckedChanged += (s, e) =>
                txtPassword.PasswordChar = chkShowPass.Checked ? '\0' : '●';

            btnLogin          = AppTheme.MakePrimaryBtn("🔐  LOGIN", 420, 44);
            btnLogin.Location = new Point(60, 376);
            btnLogin.Font     = new Font("Segoe UI", 12f, FontStyle.Bold);
            btnLogin.Click   += BtnLogin_Click;

            // Register and Exit row
            Panel btnRow = new Panel
            {
                Location  = new Point(60, 434),
                Size      = new Size(420, 44),
                BackColor = Color.Transparent
            };

            btnRegister               = AppTheme.MakePrimaryBtn("📝  Register", 204, 44);
            btnRegister.BackColor     = AppTheme.Accent;
            btnRegister.FlatAppearance.MouseOverBackColor = Color.FromArgb(67, 56, 202);
            btnRegister.Location      = new Point(0, 0);
            btnRegister.Click        += (s, e) => new AddStudent().ShowDialog();

            btnExit          = AppTheme.MakeDangerBtn("✖  Exit", 204, 44);
            btnExit.Location = new Point(216, 0);
            btnExit.Click   += (s, e) => Application.Exit();

            btnRow.Controls.Add(btnRegister);
            btnRow.Controls.Add(btnExit);

            // Enter key triggers login
            txtUsername.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnLogin_Click(null, null); };
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnLogin_Click(null, null); };
            cmbUserType.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnLogin_Click(null, null); };

            pnlRight.Controls.Add(topAccent);
            pnlRight.Controls.Add(lblWelcome);
            pnlRight.Controls.Add(lblSub);
            pnlRight.Controls.Add(lblUserType);
            pnlRight.Controls.Add(cmbUserType);
            pnlRight.Controls.Add(lblUsername);
            pnlRight.Controls.Add(txtUsername);
            pnlRight.Controls.Add(lblPassword);
            pnlRight.Controls.Add(txtPassword);
            pnlRight.Controls.Add(chkShowPass);
            pnlRight.Controls.Add(btnLogin);
            pnlRight.Controls.Add(btnRow);
            this.Controls.Add(pnlRight);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string role     = cmbUserType.SelectedItem?.ToString();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter username and password.", "Missing Fields",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text    = "Signing in...";

            try
            {
                int    userId   = 0;
                string fullName = "";
                string email    = "";
                string enroll   = "";

                if (role == "Student")
                {
                    DataTable dt = DatabaseHelper.ExecuteQuery(
                        "SELECT StudentID, FirstName + ' ' + ISNULL(LastName,'') AS FullName, Email, EnrollmentNo " +
                        "FROM Student WHERE EnrollmentNo=@u AND Password=@p AND IsActive=1",
                        new SqlParameter[]
                        {
                            new SqlParameter("@u", username),
                            new SqlParameter("@p", password)
                        });

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        userId   = Convert.ToInt32(dt.Rows[0]["StudentID"]);
                        fullName = dt.Rows[0]["FullName"].ToString().Trim();
                        email    = dt.Rows[0]["Email"].ToString();
                        enroll   = dt.Rows[0]["EnrollmentNo"].ToString();
                    }
                }
                else if (role == "Librarian")
                {
                    DataTable dt = DatabaseHelper.ExecuteQuery(
                        "SELECT LibrarianID, FirstName + ' ' + ISNULL(LastName,'') AS FullName, Email " +
                        "FROM Librarian WHERE Username=@u AND Password=@p AND IsActive=1",
                        new SqlParameter[]
                        {
                            new SqlParameter("@u", username),
                            new SqlParameter("@p", password)
                        });

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        userId   = Convert.ToInt32(dt.Rows[0]["LibrarianID"]);
                        fullName = dt.Rows[0]["FullName"].ToString().Trim();
                        email    = dt.Rows[0]["Email"].ToString();
                        DatabaseHelper.ExecuteNonQuery(
                            "UPDATE Librarian SET LastLogin=GETDATE() WHERE LibrarianID=@id",
                            new SqlParameter[] { new SqlParameter("@id", userId) });
                    }
                }
                else if (role == "Admin")
                {
                    DataTable dt = DatabaseHelper.ExecuteQuery(
                        "SELECT AdminID, FirstName + ' ' + ISNULL(LastName,'') AS FullName, Email " +
                        "FROM Admin WHERE Username=@u AND Password=@p AND IsActive=1",
                        new SqlParameter[]
                        {
                            new SqlParameter("@u", username),
                            new SqlParameter("@p", password)
                        });

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        userId   = Convert.ToInt32(dt.Rows[0]["AdminID"]);
                        fullName = dt.Rows[0]["FullName"].ToString().Trim();
                        email    = dt.Rows[0]["Email"].ToString();
                        DatabaseHelper.ExecuteNonQuery(
                            "UPDATE Admin SET LastLogin=GETDATE() WHERE AdminID=@id",
                            new SqlParameter[] { new SqlParameter("@id", userId) });
                    }
                }


                if (userId > 0)
                {
                    SessionManager.Login(userId, username, fullName, email, role, enroll);
                    this.Hide();

                    if      (role == "Admin")     new AdminDashboard().ShowDialog();
                    else if (role == "Librarian") new LibrarianDashboard().ShowDialog();
                    else                          new UserDashboard().ShowDialog();

                    SessionManager.Logout();
                    txtUsername.Clear();
                    txtPassword.Clear();
                    this.Show();
                }
                else
                {
                    MessageBox.Show("Invalid credentials. Please try again.", "Login Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text    = "🔐  LOGIN";
            }
        }
    }
}
