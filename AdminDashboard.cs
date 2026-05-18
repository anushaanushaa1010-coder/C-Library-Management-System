using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Library_Management_System
{
    public class AdminDashboard : Form
    {
        // -- Session
        private int adminID => SessionManager.CurrentUserID;

        // -- Layout
        private Panel pnlNavBar, pnlContent;
        private Button btnActive;

        // -- Home stats
        private Label lblWelcomeName;
        private Label lblStuCount, lblBookCount, lblIssuedCount, lblPendingCount;

        // -- Pages
        private Panel pgHome, pgLibrarians, pgStudents, pgBooks, pgIssues, pgFines, pgCategories, pgReports;

        // -- Librarians
        private DataGridView dgvLib;
        private TextBox txtLU, txtLFN, txtLLN, txtLEM, txtLPH, txtLPW;

        // -- Students
        private DataGridView dgvStu;
        private TextBox txtStuSearch;

        // -- Books
        private DataGridView dgvBooks;
        private TextBox txtBookSearch;
        private Panel pnlBookForm;
        private TextBox bfName, bfAuthor, bfISBN, bfPub, bfQty, bfPrice;
        private ComboBox bfCat;
        private int editBookID = 0;

        // -- Issues
        private DataGridView dgvIssues;
        private ComboBox cmbIssFilter;

        // -- Fines
        private DataGridView dgvFines;
        private Label lblFineTotal;

        // -- Category helper
        private class CatItem
        {
            public int    ID   { get; set; }
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        public AdminDashboard()
        {
            Text          = "AIUB Library - Admin Panel";
            WindowState   = FormWindowState.Maximized;
            MinimumSize   = new Size(1100, 650);
            StartPosition = FormStartPosition.CenterScreen;
            AppTheme.StyleForm(this);

         // BuildHeader();
            BuildNavBar();
           BuildFooter();
            BuildContent();

            this.Load += (s, e) => { LoadAll(); NavClick("Dashboard"); };
        }

        private void BuildHeader()
        {
            var hdr = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppTheme.NavyDark };

            var title = new Label
            {
                Text      = "AIUB Library Management System - Admin",
                Font      = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = AppTheme.TealLight,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(20, 0, 0, 0)
            };

            lblWelcomeName = new Label
            {
                Text      = "Admin: " + SessionManager.CurrentUserName,
                Font      = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock      = DockStyle.Right,
                Width     = 340,
                TextAlign = ContentAlignment.MiddleRight,
                Padding   = new Padding(0, 0, 20, 0)
            };

            hdr.Controls.Add(lblWelcomeName);
            hdr.Controls.Add(title);
            Controls.Add(hdr);
        }

        private void BuildNavBar()
        {
            pnlNavBar = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = AppTheme.NavyMid };
            var sep   = new Panel { Dock = DockStyle.Bottom, Height = 3, BackColor = AppTheme.Teal };
            pnlNavBar.Controls.Add(sep);

            string[] btns = {
                "Home  Dashboard", "Librarians", "Students",
                "Books", "Issues", "Fines", "Categories", "Reports", "Logout"
            };

            int x = 8;
            foreach (string lbl in btns)
            {
                bool isDanger = lbl.Contains("Logout");
                var btn = new Button
                {
                    Text      = lbl,
                    Width     = 122,
                    Height    = 44,
                    Location  = new Point(x, 3),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = isDanger ? Color.FromArgb(120, 30, 30) : AppTheme.NavyMid,
                    ForeColor = Color.FromArgb(203, 213, 225),
                    Font      = new Font(AppTheme.FontBody.FontFamily, 9f),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor    = Cursors.Hand,
                    Tag       = lbl
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = isDanger ? Color.FromArgb(180, 40, 40) : AppTheme.NavyLight;
                btn.Click += NavBtn_Click;
                pnlNavBar.Controls.Add(btn);
                x += 126;
            }
            lblWelcomeName = new Label
            {
                Text = "Admin: " + SessionManager.CurrentUserName,
                Font = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock = DockStyle.Right,
                Width = 240,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 16, 0)
            };
            pnlNavBar.Controls.Add(lblWelcomeName);

           
            Controls.Add(pnlNavBar);
        }

        private void BuildFooter()
        {
            var ftr = new Panel { Dock = DockStyle.Bottom, Height = 26, BackColor = AppTheme.NavyDark };
            ftr.Controls.Add(new Label
            {
                Text      = "AIUB Library Management System  |  Admin Portal",
                Font      = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(71, 85, 105),
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });
            Controls.Add(ftr);
        }

        private void BuildContent()
        {
            pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface };
            Controls.Add(pnlContent);

            BuildPageHome();
            BuildPageLibrarians();
            BuildPageStudents();
            BuildPageBooks();
            BuildPageIssues();
            BuildPageFines();
            BuildPageCategories();
            BuildPageReports();
        }

        private void NavBtn_Click(object sender, EventArgs e)
        {
            string tag = (sender as Button)?.Tag?.ToString() ?? "";
            if (tag.Contains("Logout"))
            {
                if (Confirm("Logout from Admin Panel?")) this.Close();
                return;
            }
            NavClick(tag);
        }

        private void NavClick(string tag)
        {
            foreach (Control c in pnlNavBar.Controls)
            {
                if (!(c is Button b)) continue;
                bool isLogout = b.Tag?.ToString().Contains("Logout") == true;
                b.BackColor = isLogout ? Color.FromArgb(120, 30, 30) : AppTheme.NavyMid;
                b.ForeColor = Color.FromArgb(203, 213, 225);
            }

            foreach (Control c in pnlNavBar.Controls)
            {
                if (!(c is Button b)) continue;
                string t = b.Tag?.ToString() ?? "";
                bool match = (tag.Contains("Dashboard") && t.Contains("Dashboard"))
                    || (!tag.Contains("Dashboard") && t.Contains(tag));
                if (match) { b.BackColor = AppTheme.Teal; b.ForeColor = Color.White; btnActive = b; }
            }

            foreach (Control p in pnlContent.Controls)
                if (p is Panel) p.Visible = false;

            if      (tag.Contains("Dashboard"))  { pgHome.Visible       = true; LoadDashStats(); }
            else if (tag.Contains("Librarian"))   { pgLibrarians.Visible = true; LoadLibrarians(); }
            else if (tag.Contains("Student"))     { pgStudents.Visible   = true; LoadStudents(); }
            else if (tag.Contains("Book"))        { pgBooks.Visible      = true; LoadBooks(); }
            else if (tag.Contains("Issue"))       { pgIssues.Visible     = true; LoadIssues(); }
            else if (tag.Contains("Fine"))        { pgFines.Visible      = true; LoadFines(); }
            else if (tag.Contains("Categor"))     { pgCategories.Visible = true; }
            else if (tag.Contains("Report"))      { pgReports.Visible    = true; }
        }

        // PAGE: HOME
        private void BuildPageHome()
        {
            pgHome = MakePage();

            var welcome = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = AppTheme.NavyDark };
            welcome.Controls.Add(new Label
            {
                Text      = "Welcome, Administrator!   |   " + DateTime.Now.ToString("dddd, dd MMMM yyyy"),
                Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = AppTheme.TealLight,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(24, 0, 0, 0)
            });
            pgHome.Controls.Add(welcome);

            var c1 = MakeCard("0", "Total Students",   AppTheme.Teal,    20,  90); lblStuCount    = GetCardVal(c1);
            var c2 = MakeCard("0", "Total Books",      AppTheme.Accent,  210, 90); lblBookCount   = GetCardVal(c2);
            var c3 = MakeCard("0", "Currently Issued", AppTheme.Warning, 400, 90); lblIssuedCount  = GetCardVal(c3);
            var c4 = MakeCard("0", "Pending Requests", AppTheme.Danger,  590, 90); lblPendingCount = GetCardVal(c4);
            pgHome.Controls.AddRange(new Control[] { c1, c2, c3, c4 });

            var btnRef  = AppTheme.MakePrimaryBtn("Refresh Stats", 150, 36);
            btnRef.Location  = new Point(20, 198); btnRef.BackColor = AppTheme.NavyMid;
            btnRef.Click    += (s, e) => LoadDashStats();

            var btnProf = AppTheme.MakeWarningBtn("My Profile", 130, 36);
            btnProf.Location = new Point(178, 198);
            btnProf.Click   += (s, e) => new AdminProfile(adminID).ShowDialog();

            pgHome.Controls.AddRange(new Control[] { btnRef, btnProf,
                new Label {
                    Text     = "Use the navigation bar above to manage Librarians, Students, Books, Issues, Fines, Categories and Reports.",
                    Font     = AppTheme.FontBody, ForeColor = AppTheme.TextMuted,
                    AutoSize = true, Location = new Point(22, 250)
                }});
        }

        // PAGE: LIBRARIANS
        private void BuildPageLibrarians()
        {
            pgLibrarians = MakePage();
            pgLibrarians.Controls.Add(PageTitle("Manage Librarians"));

            var form = new Panel { Location = new Point(10, 55), Width = 900, Height = 95,
                BackColor = AppTheme.CardBg, BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            pgLibrarians.Resize += (s, e) => form.Width = Math.Max(pgLibrarians.Width - 20, 300);
            form.Controls.Add(new Label { Text = "Add New Librarian", Font = AppTheme.FontH3, ForeColor = AppTheme.NavyDark, AutoSize = true, Location = new Point(10, 6) });

            int x = 10;
            txtLU  = FI(form, "Username *",   x, 130); x += 150;
            txtLFN = FI(form, "First Name *", x, 140); x += 160;
            txtLLN = FI(form, "Last Name",    x, 130); x += 150;
            txtLEM = FI(form, "Email",        x, 180); x += 200;
            txtLPH = FI(form, "Phone",        x, 120); x += 140;
            txtLPW = FI(form, "Password *",   x, 120, true);

            var btnAdd = AppTheme.MakeSuccessBtn("Add Librarian", 150, 32);
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdd.Location = new Point(form.Width - 162, 54);
            form.Resize += (s, e) => btnAdd.Location = new Point(form.Width - 162, 54);
            btnAdd.Click += AddLibrarian_Click;
            form.Controls.Add(btnAdd);
            pgLibrarians.Controls.Add(form);

            var btnToggle = AppTheme.MakeWarningBtn("Toggle Active", 140, 32); btnToggle.Location = new Point(10, 158);  btnToggle.Click += (s, e) => ToggleLibrarian();
            var btnDel    = AppTheme.MakeDangerBtn("Delete",         100, 32); btnDel.Location    = new Point(158, 158); btnDel.Click    += (s, e) => DeleteLibrarian();
            var btnRef    = AppTheme.MakePrimaryBtn("Refresh",       90,  32); btnRef.Location    = new Point(266, 158); btnRef.BackColor = AppTheme.NavyMid; btnRef.Click += (s, e) => LoadLibrarians();
            pgLibrarians.Controls.AddRange(new Control[] { btnToggle, btnDel, btnRef });

            dgvLib = new DataGridView { Location = new Point(10, 198), BackgroundColor = AppTheme.Surface,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            AppTheme.StyleGrid(dgvLib);
            pgLibrarians.Controls.Add(dgvLib);
            pgLibrarians.Resize += (s, e) => dgvLib.Size = new Size(pgLibrarians.Width - 20, pgLibrarians.Height - 206);
            dgvLib.Size = new Size(1000, 420);
        }

        // PAGE: STUDENTS
        private void BuildPageStudents()
        {
            pgStudents = MakePage();
            pgStudents.Controls.Add(PageTitle("Manage Students"));

            var bar = new Panel { Location = new Point(10, 55), Width = 700, Height = 46,
                BackColor = AppTheme.CardBg, BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            pgStudents.Resize += (s, e) => bar.Width = Math.Max(pgStudents.Width - 20, 300);

            txtStuSearch = AppTheme.MakeInput(280); txtStuSearch.Location = new Point(8, 8);
            txtStuSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) LoadStudents(); };
            var bS = AppTheme.MakePrimaryBtn("Search", 90, 30);   bS.Location  = new Point(296, 8); bS.Click += (s, e) => LoadStudents();
            var bA = AppTheme.MakePrimaryBtn("All", 70, 30);      bA.BackColor = AppTheme.NavyMid; bA.Location = new Point(394, 8);
            bA.Click += (s, e) => { txtStuSearch.Clear(); LoadStudents(); };
            bar.Controls.AddRange(new Control[] { txtStuSearch, bS, bA });
            pgStudents.Controls.Add(bar);

            var btnTog = AppTheme.MakeWarningBtn("Toggle Active",  140, 32); btnTog.Location = new Point(10,  110); btnTog.Click += (s, e) => ToggleStudent();
            var btnDel = AppTheme.MakeDangerBtn("Delete Student",  140, 32); btnDel.Location = new Point(158, 110); btnDel.Click += (s, e) => DeleteStudent();
            var btnRef = AppTheme.MakePrimaryBtn("Refresh",        90,  32); btnRef.Location = new Point(306, 110); btnRef.BackColor = AppTheme.NavyMid; btnRef.Click += (s, e) => LoadStudents();
            pgStudents.Controls.AddRange(new Control[] { btnTog, btnDel, btnRef });

            dgvStu = new DataGridView { Location = new Point(10, 150), BackgroundColor = AppTheme.Surface,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            AppTheme.StyleGrid(dgvStu);
            pgStudents.Controls.Add(dgvStu);
            pgStudents.Resize += (s, e) => dgvStu.Size = new Size(pgStudents.Width - 20, pgStudents.Height - 158);
            dgvStu.Size = new Size(1000, 480);
        }

        // PAGE: BOOKS
        private void BuildPageBooks()
        {
            pgBooks = MakePage();
            pgBooks.Controls.Add(PageTitle("Manage Books"));

            var bar = new Panel { Location = new Point(10, 55), Width = 800, Height = 46,
                BackColor = AppTheme.CardBg, BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            pgBooks.Resize += (s, e) => bar.Width = Math.Max(pgBooks.Width - 20, 400);

            txtBookSearch = AppTheme.MakeInput(240); txtBookSearch.Location = new Point(8, 8);
            txtBookSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) LoadBooks(); };
            var bS   = AppTheme.MakePrimaryBtn("Search", 80, 30);   bS.Location   = new Point(256, 8);  bS.Click  += (s, e) => LoadBooks();
            var bAll = AppTheme.MakePrimaryBtn("All",    55, 30);   bAll.BackColor = AppTheme.NavyMid;   bAll.Location = new Point(344, 8); bAll.Click += (s, e) => { txtBookSearch.Clear(); LoadBooks(); };
            var bAdd = AppTheme.MakeSuccessBtn("Add Book", 100, 30); bAdd.Location = new Point(408, 8);  bAdd.Click += (s, e) => ShowBookForm(0);
            var bEdt = AppTheme.MakeWarningBtn("Edit",     80, 30);  bEdt.Location = new Point(516, 8);  bEdt.Click += (s, e) => EditBook();
            var bDel = AppTheme.MakeDangerBtn("Delete",   80, 30);   bDel.Location = new Point(604, 8);  bDel.Click += (s, e) => DeleteBook();
            bar.Controls.AddRange(new Control[] { txtBookSearch, bS, bAll, bAdd, bEdt, bDel });
            pgBooks.Controls.Add(bar);

            pnlBookForm = new Panel { Location = new Point(10, 108), Width = 800, Height = 124,
                BackColor = Color.FromArgb(240, 249, 255), BorderStyle = BorderStyle.FixedSingle,
                Visible = false, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            pgBooks.Resize += (s, e) => { if (pnlBookForm != null) pnlBookForm.Width = Math.Max(pgBooks.Width - 20, 400); };
            pnlBookForm.Controls.Add(new Label { Text = "Book Details:", Font = AppTheme.FontH3, ForeColor = AppTheme.NavyDark, AutoSize = true, Location = new Point(8, 6) });

            int bx = 8;
            bfName   = BFI(pnlBookForm, "Book Name*", bx, 220); bx += 240;
            bfAuthor = BFI(pnlBookForm, "Author*",    bx, 160); bx += 180;
            bfISBN   = BFI(pnlBookForm, "ISBN",       bx, 120); bx += 140;
            bfPub    = BFI(pnlBookForm, "Publisher",  bx, 150); bx += 170;
            bfQty    = BFI(pnlBookForm, "Qty",        bx, 60);  bx += 80;
            bfPrice  = BFI(pnlBookForm, "Price(TK)",  bx, 80);  bx += 100;
            pnlBookForm.Controls.Add(new Label { Text = "Category", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(bx, 26) });
            bfCat = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.FontInput, Width = 150, Location = new Point(bx, 44) };
            pnlBookForm.Controls.Add(bfCat);

            var btnSave   = AppTheme.MakeSuccessBtn("Save",   90, 28); btnSave.Location   = new Point(8,   90); btnSave.Click   += SaveBook_Click;
            var btnCancel = AppTheme.MakeDangerBtn("Cancel",  80, 28); btnCancel.Location = new Point(106, 90); btnCancel.Click += (s, e) => { pnlBookForm.Visible = false; ReSizeBookGrid(); };
            pnlBookForm.Controls.AddRange(new Control[] { btnSave, btnCancel });
            pgBooks.Controls.Add(pnlBookForm);

            dgvBooks = new DataGridView { Location = new Point(10, 108), BackgroundColor = AppTheme.Surface,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            AppTheme.StyleGrid(dgvBooks);
            pgBooks.Controls.Add(dgvBooks);
            pgBooks.Resize += (s, e) => ReSizeBookGrid();
            dgvBooks.Size = new Size(1000, 500);
        }

        private void ReSizeBookGrid()
        {
            if (pnlBookForm != null && pnlBookForm.Visible)
            {
                dgvBooks.Location = new Point(10, 240);
                dgvBooks.Size     = new Size(pgBooks.Width - 20, pgBooks.Height - 246);
            }
            else
            {
                dgvBooks.Location = new Point(10, 108);
                dgvBooks.Size     = new Size(pgBooks.Width - 20, pgBooks.Height - 114);
            }
        }

        // PAGE: ISSUES
        private void BuildPageIssues()
        {
            pgIssues = MakePage();
            pgIssues.Controls.Add(PageTitle("All Book Issues"));

            var bar = new Panel { Location = new Point(10, 55), Width = 450, Height = 46, BackColor = AppTheme.CardBg, BorderStyle = BorderStyle.FixedSingle };
            bar.Controls.Add(new Label { Text = "Filter:", Font = AppTheme.FontLabel, AutoSize = true, Location = new Point(8, 14) });
            cmbIssFilter = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.FontInput, Width = 150, Location = new Point(55, 10) };
            cmbIssFilter.Items.AddRange(new object[] { "All", "Issued", "Overdue", "Returned" });
            cmbIssFilter.SelectedIndex = 0;
            cmbIssFilter.SelectedIndexChanged += (s, e) => LoadIssues();
            var btnR = AppTheme.MakePrimaryBtn("Refresh", 85, 30); btnR.Location = new Point(214, 8); btnR.BackColor = AppTheme.NavyMid; btnR.Click += (s, e) => LoadIssues();
            bar.Controls.AddRange(new Control[] { cmbIssFilter, btnR });
            pgIssues.Controls.Add(bar);

            dgvIssues = new DataGridView { Location = new Point(10, 110), BackgroundColor = AppTheme.Surface,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            AppTheme.StyleGrid(dgvIssues);
            pgIssues.Controls.Add(dgvIssues);
            pgIssues.Resize += (s, e) => dgvIssues.Size = new Size(pgIssues.Width - 20, pgIssues.Height - 118);
            dgvIssues.Size = new Size(1000, 500);
        }

        // PAGE: FINES
        private void BuildPageFines()
        {
            pgFines = MakePage();
            pgFines.Controls.Add(PageTitle("Fine Management"));

            lblFineTotal = new Label { Text = "Total Outstanding: TK 0", Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = AppTheme.Danger, AutoSize = true, Location = new Point(10, 55) };
            var btnPaid = AppTheme.MakeSuccessBtn("Mark Selected Paid", 180, 34); btnPaid.Location = new Point(10,  92); btnPaid.Click += MarkPaid_Click;
            var btnRef  = AppTheme.MakePrimaryBtn("Refresh",            110, 34); btnRef.Location  = new Point(198, 92); btnRef.BackColor = AppTheme.NavyMid; btnRef.Click += (s, e) => LoadFines();
            pgFines.Controls.AddRange(new Control[] { lblFineTotal, btnPaid, btnRef });

            dgvFines = new DataGridView { Location = new Point(10, 134), BackgroundColor = AppTheme.Surface,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            AppTheme.StyleGrid(dgvFines);
            pgFines.Controls.Add(dgvFines);
            pgFines.Resize += (s, e) => dgvFines.Size = new Size(pgFines.Width - 20, pgFines.Height - 140);
            dgvFines.Size = new Size(1000, 480);
        }

        // PAGE: CATEGORIES
        private void BuildPageCategories()
        {
            pgCategories = MakePage();
            pgCategories.Controls.Add(PageTitle("Book Categories"));
            var btn = AppTheme.MakePrimaryBtn("Open Category Manager", 230, 44);
            btn.Location = new Point(20, 70); btn.BackColor = AppTheme.Accent;
            btn.Click += (s, e) => new CategoryManagement(adminID).ShowDialog();
            pgCategories.Controls.Add(btn);
            pgCategories.Controls.Add(new Label { Text = "Manage book categories - add, edit or remove categories.", Font = AppTheme.FontBody, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(22, 126) });
        }

        // PAGE: REPORTS
        private void BuildPageReports()
        {
            pgReports = MakePage();
            pgReports.Controls.Add(PageTitle("Reports and Analytics"));
            var btn = AppTheme.MakePrimaryBtn("Open Reports", 190, 44);
            btn.Location = new Point(20, 70); btn.BackColor = AppTheme.Teal;
            btn.Click += (s, e) => new ReportsForm(adminID).ShowDialog();
            pgReports.Controls.Add(btn);
            pgReports.Controls.Add(new Label { Text = "View issue reports, top borrowed books, top students, and fine summaries.", Font = AppTheme.FontBody, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(22, 126) });
        }

        // LOAD DATA
        private void LoadAll()
        {
            try
            {
                object n = DatabaseHelper.ExecuteScalar(
                    "SELECT FirstName+' '+ISNULL(LastName,'') FROM Admin WHERE AdminID=@id",
                    new SqlParameter("@id", adminID));
                if (lblWelcomeName != null)
                    lblWelcomeName.Text = "Admin: " + (n?.ToString().Trim() ?? SessionManager.CurrentUserName);
            }
            catch { }

            LoadDashStats();
            LoadCatCombo();
        }

        private void LoadDashStats()
        {
            try
            {
                if (lblStuCount    != null) lblStuCount.Text    = (DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Student WHERE IsActive=1")                             ?? 0).ToString();
                if (lblBookCount   != null) lblBookCount.Text   = (DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Books")                                                  ?? 0).ToString();
                if (lblIssuedCount != null) lblIssuedCount.Text = (DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM BookIssue WHERE Status IN ('Issued','Overdue')")        ?? 0).ToString();
                if (lblPendingCount!= null) lblPendingCount.Text= (DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM BorrowRequest WHERE Status='Pending'")                  ?? 0).ToString();
            }
            catch { }
        }

        private void LoadLibrarians()
        {
            try
            {
                dgvLib.DataSource = DatabaseHelper.ExecuteQuery(
                    "SELECT LibrarianID, Username, FirstName+' '+ISNULL(LastName,'') AS [Full Name], Email, PhoneNo, " +
                    "CASE IsActive WHEN 1 THEN 'Active' ELSE 'Inactive' END AS Status FROM Librarian ORDER BY Username");
                if (dgvLib.Columns.Contains("LibrarianID")) dgvLib.Columns["LibrarianID"].Visible = false;
            }
            catch (Exception ex) { Err(ex); }
        }

        private void LoadStudents()
        {
            try
            {
                string s = txtStuSearch?.Text.Trim() ?? "";
                DataTable dt = string.IsNullOrEmpty(s)
                    ? DatabaseHelper.ExecuteQuery(
                        "SELECT StudentID, EnrollmentNo, FirstName+' '+ISNULL(LastName,'') AS [Full Name], " +
                        "Email, Department, Semester, CASE IsActive WHEN 1 THEN 'Active' ELSE 'Inactive' END AS Status FROM Student ORDER BY EnrollmentNo")
                    : DatabaseHelper.ExecuteQuery(
                        "SELECT StudentID, EnrollmentNo, FirstName+' '+ISNULL(LastName,'') AS [Full Name], " +
                        "Email, Department, Semester, CASE IsActive WHEN 1 THEN 'Active' ELSE 'Inactive' END AS Status " +
                        "FROM Student WHERE FirstName LIKE @s OR LastName LIKE @s OR EnrollmentNo LIKE @s ORDER BY EnrollmentNo",
                        new SqlParameter("@s", "%" + s + "%"));
                dgvStu.DataSource = dt;
                if (dgvStu.Columns.Contains("StudentID")) dgvStu.Columns["StudentID"].Visible = false;
            }
            catch (Exception ex) { Err(ex); }
        }

        private void LoadBooks()
        {
            try
            {
                string s = txtBookSearch?.Text.Trim() ?? "";
                string q = "SELECT b.BookID, b.BookName AS [Title], b.Author, ISNULL(c.CategoryName,'--') AS [Category], " +
                           "b.TotalQuantity AS [Total Qty], b.AvailableQuantity AS [Available], b.Price AS [Price TK], b.ISBN " +
                           "FROM Books b LEFT JOIN Category c ON b.CategoryID=c.CategoryID";
                DataTable dt = string.IsNullOrEmpty(s)
                    ? DatabaseHelper.ExecuteQuery(q + " ORDER BY b.BookName")
                    : DatabaseHelper.ExecuteQuery(q + " WHERE b.BookName LIKE @s OR b.Author LIKE @s ORDER BY b.BookName", new SqlParameter("@s", "%" + s + "%"));
                dgvBooks.DataSource = dt;
                if (dgvBooks.Columns.Contains("BookID")) dgvBooks.Columns["BookID"].Visible = false;
            }
            catch (Exception ex) { Err(ex); }
        }

        private void LoadIssues()
        {
            try
            {
                string f = cmbIssFilter?.SelectedItem?.ToString() ?? "All";
                string w = f == "All" ? "" : " AND bi.Status='" + f + "'";
                dgvIssues.DataSource = DatabaseHelper.ExecuteQuery(
                    "SELECT bi.IssueID, s.EnrollmentNo, s.FirstName+' '+ISNULL(s.LastName,'') AS [Student], " +
                    "b.BookName AS [Book], bi.IssuedDate AS [Issued], bi.DueDate AS [Due], bi.ReturnedDate AS [Returned], " +
                    "bi.Status, ISNULL(bi.FineAmount,0) AS [Fine TK] " +
                    "FROM BookIssue bi JOIN Student s ON bi.StudentID=s.StudentID JOIN Books b ON bi.BookID=b.BookID " +
                    "WHERE 1=1" + w + " ORDER BY bi.IssuedDate DESC");
                if (dgvIssues.Columns.Contains("IssueID")) dgvIssues.Columns["IssueID"].Visible = false;
            }
            catch (Exception ex) { Err(ex); }
        }

        private void LoadFines()
        {
            try
            {
                dgvFines.DataSource = DatabaseHelper.ExecuteQuery(
                    "SELECT bi.IssueID, s.EnrollmentNo, s.FirstName+' '+ISNULL(s.LastName,'') AS [Student], " +
                    "b.BookName AS [Book], bi.DueDate AS [Due], bi.ReturnedDate AS [Returned], " +
                    "bi.Status, ISNULL(bi.FineAmount,0) AS [Fine TK] " +
                    "FROM BookIssue bi JOIN Student s ON bi.StudentID=s.StudentID JOIN Books b ON bi.BookID=b.BookID " +
                    "WHERE bi.FineAmount > 0 ORDER BY bi.FineAmount DESC");
                if (dgvFines.Columns.Contains("IssueID")) dgvFines.Columns["IssueID"].Visible = false;
                object tot = DatabaseHelper.ExecuteScalar("SELECT ISNULL(SUM(FineAmount),0) FROM BookIssue WHERE FineAmount>0 AND ReturnedDate IS NULL");
                if (lblFineTotal != null) lblFineTotal.Text = "Total Outstanding: TK " + Convert.ToDecimal(tot ?? 0).ToString("N0");
            }
            catch (Exception ex) { Err(ex); }
        }

        private void LoadCatCombo()
        {
            try
            {
                if (bfCat == null) return;
                bfCat.Items.Clear();
                bfCat.Items.Add(new CatItem { ID = 0, Name = "-- Select Category --" });
                var dt = DatabaseHelper.ExecuteQuery("SELECT CategoryID, CategoryName FROM Category WHERE IsActive=1 ORDER BY CategoryName");
                if (dt != null)
                    foreach (DataRow r in dt.Rows)
                        bfCat.Items.Add(new CatItem { ID = Convert.ToInt32(r["CategoryID"]), Name = r["CategoryName"].ToString() });
                bfCat.DisplayMember = "Name";
                if (bfCat.Items.Count > 0) bfCat.SelectedIndex = 0;
            }
            catch { }
        }

        // ACTIONS: LIBRARIANS
        private void AddLibrarian_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLU.Text) || string.IsNullOrWhiteSpace(txtLFN.Text) || string.IsNullOrWhiteSpace(txtLPW.Text))
            { MessageBox.Show("Username, First Name and Password are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            bool ok = DatabaseHelper.ExecuteNonQuery(
                "INSERT INTO Librarian (Username,FirstName,LastName,Email,PhoneNo,Password,IsActive) VALUES (@u,@fn,@ln,@em,@ph,@pw,1)",
                new SqlParameter("@u",  txtLU.Text.Trim()),
                new SqlParameter("@fn", txtLFN.Text.Trim()),
                new SqlParameter("@ln", txtLLN.Text.Trim()),
                new SqlParameter("@em", txtLEM.Text.Trim()),
                new SqlParameter("@ph", txtLPH.Text.Trim()),
                new SqlParameter("@pw", txtLPW.Text));

            if (ok)
            {
                MessageBox.Show("Librarian added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtLU.Clear(); txtLFN.Clear(); txtLLN.Clear(); txtLEM.Clear(); txtLPH.Clear(); txtLPW.Clear();
                LoadLibrarians(); LoadDashStats();
            }
        }

        private void ToggleLibrarian()
        {
            if (dgvLib.SelectedRows.Count == 0) { MessageBox.Show("Select a librarian first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string user = dgvLib.SelectedRows[0].Cells["Username"].Value?.ToString();
            DatabaseHelper.ExecuteNonQuery("UPDATE Librarian SET IsActive=CASE WHEN IsActive=1 THEN 0 ELSE 1 END WHERE Username=@u", new SqlParameter("@u", user));
            LoadLibrarians();
        }

        private void DeleteLibrarian()
        {
            if (dgvLib.SelectedRows.Count == 0) { MessageBox.Show("Select a librarian first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string user = dgvLib.SelectedRows[0].Cells["Username"].Value?.ToString();
            if (!Confirm("Delete librarian '" + user + "'? This cannot be undone.")) return;
            DatabaseHelper.ExecuteNonQuery("DELETE FROM Librarian WHERE Username=@u", new SqlParameter("@u", user));
            LoadLibrarians(); LoadDashStats();
        }

        // ACTIONS: STUDENTS
        private void ToggleStudent()
        {
            if (dgvStu.SelectedRows.Count == 0) { MessageBox.Show("Select a student first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string enroll = dgvStu.SelectedRows[0].Cells["EnrollmentNo"].Value?.ToString();
            DatabaseHelper.ExecuteNonQuery("UPDATE Student SET IsActive=CASE WHEN IsActive=1 THEN 0 ELSE 1 END WHERE EnrollmentNo=@e", new SqlParameter("@e", enroll));
            LoadStudents();
        }

        private void DeleteStudent()
        {
            if (dgvStu.SelectedRows.Count == 0) { MessageBox.Show("Select a student first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string enroll = dgvStu.SelectedRows[0].Cells["EnrollmentNo"].Value?.ToString();
            object cnt = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM BookIssue bi JOIN Student s ON bi.StudentID=s.StudentID WHERE s.EnrollmentNo=@e AND bi.Status IN ('Issued','Overdue')",
                new SqlParameter("@e", enroll));
            if (Convert.ToInt32(cnt ?? 0) > 0)
            { MessageBox.Show("Cannot delete: student has active borrowed books.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!Confirm("Delete student '" + enroll + "'?")) return;
            DatabaseHelper.ExecuteNonQuery("DELETE FROM Student WHERE EnrollmentNo=@e", new SqlParameter("@e", enroll));
            LoadStudents(); LoadDashStats();
        }

        // ACTIONS: BOOKS
        private void ShowBookForm(int bookID)
        {
            editBookID = bookID;
            bfName.Clear(); bfAuthor.Clear(); bfISBN.Clear(); bfPub.Clear(); bfQty.Text = "1"; bfPrice.Text = "0";
            LoadCatCombo();
            pnlBookForm.Visible = true;
            ReSizeBookGrid();
        }

        private void EditBook()
        {
            if (dgvBooks.SelectedRows.Count == 0) { MessageBox.Show("Select a book to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            var row   = dgvBooks.SelectedRows[0];
            editBookID    = Convert.ToInt32(row.Cells["BookID"].Value);
            bfName.Text   = row.Cells["Title"].Value?.ToString();
            bfAuthor.Text = row.Cells["Author"].Value?.ToString();
            bfISBN.Text   = row.Cells["ISBN"].Value?.ToString();
            bfQty.Text    = row.Cells["Total Qty"].Value?.ToString();
            bfPrice.Text  = row.Cells["Price TK"].Value?.ToString();
            LoadCatCombo();
            pnlBookForm.Visible = true;
            ReSizeBookGrid();
        }

        private void DeleteBook()
        {
            if (dgvBooks.SelectedRows.Count == 0) { MessageBox.Show("Select a book to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            int    bid   = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["BookID"].Value);
            string title = dgvBooks.SelectedRows[0].Cells["Title"].Value?.ToString();
            object cnt   = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM BookIssue WHERE BookID=@b AND Status IN ('Issued','Overdue')", new SqlParameter("@b", bid));
            if (Convert.ToInt32(cnt ?? 0) > 0) { MessageBox.Show("Cannot delete: book is currently issued.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!Confirm("Delete book '" + title + "'?")) return;
            DatabaseHelper.ExecuteNonQuery("DELETE FROM Books WHERE BookID=@b", new SqlParameter("@b", bid));
            LoadBooks(); LoadDashStats();
        }

        private void SaveBook_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(bfName.Text) || string.IsNullOrWhiteSpace(bfAuthor.Text))
            { MessageBox.Show("Book name and author are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            int     catID  = (bfCat.SelectedItem is CatItem ci && ci.ID > 0) ? ci.ID : 0;
            int     qty    = int.TryParse(bfQty.Text, out int q) ? q : 1;
            decimal price  = decimal.TryParse(bfPrice.Text, out decimal p) ? p : 0m;
            object  catVal = catID > 0 ? (object)catID : DBNull.Value;

            bool ok = editBookID == 0
                ? DatabaseHelper.ExecuteNonQuery(
                    "INSERT INTO Books (BookName,Author,ISBN,Publication,CategoryID,TotalQuantity,AvailableQuantity,Price,AddedBy) VALUES (@n,@a,@isbn,@pub,@cat,@qty,@qty,@pr,@adm)",
                    new SqlParameter("@n",    bfName.Text.Trim()),
                    new SqlParameter("@a",    bfAuthor.Text.Trim()),
                    new SqlParameter("@isbn", bfISBN.Text.Trim()),
                    new SqlParameter("@pub",  bfPub.Text.Trim()),
                    new SqlParameter("@cat",  catVal),
                    new SqlParameter("@qty",  qty),
                    new SqlParameter("@pr",   price),
                    new SqlParameter("@adm",  adminID))
                : DatabaseHelper.ExecuteNonQuery(
                    "UPDATE Books SET BookName=@n,Author=@a,ISBN=@isbn,Publication=@pub,CategoryID=@cat,TotalQuantity=@qty,Price=@pr WHERE BookID=@bid",
                    new SqlParameter("@n",    bfName.Text.Trim()),
                    new SqlParameter("@a",    bfAuthor.Text.Trim()),
                    new SqlParameter("@isbn", bfISBN.Text.Trim()),
                    new SqlParameter("@pub",  bfPub.Text.Trim()),
                    new SqlParameter("@cat",  catVal),
                    new SqlParameter("@qty",  qty),
                    new SqlParameter("@pr",   price),
                    new SqlParameter("@bid",  editBookID));

            if (ok)
            {
                MessageBox.Show(editBookID == 0 ? "Book added!" : "Book updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pnlBookForm.Visible = false; editBookID = 0;
                ReSizeBookGrid(); LoadBooks(); LoadDashStats();
            }
        }

        // ACTIONS: FINES
        private void MarkPaid_Click(object sender, EventArgs e)
        {
            if (dgvFines.SelectedRows.Count == 0) { MessageBox.Show("Select a record first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            int issID = Convert.ToInt32(dgvFines.SelectedRows[0].Cells["IssueID"].Value);
            if (!Confirm("Mark this fine as paid? (FineAmount will be set to 0)")) return;
            DatabaseHelper.ExecuteNonQuery("UPDATE BookIssue SET FineAmount=0 WHERE IssueID=@id", new SqlParameter("@id", issID));
            LoadFines();
        }

        // HELPERS
        private Panel MakePage()
        {
            var p = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false, Padding = new Padding(10) };
            pnlContent.Controls.Add(p);
            return p;
        }

        private Label PageTitle(string text) =>
            new Label { Text = text, Font = AppTheme.FontH2, ForeColor = AppTheme.NavyDark, AutoSize = true, Location = new Point(10, 14) };

        private Panel MakeCard(string val, string label, Color color, int x, int y)
        {
            var card = new Panel { Location = new Point(x, y), Size = new Size(175, 90), BackColor = Color.White };
            card.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(new Pen(AppTheme.BorderColor), 0, 0, card.Width - 1, card.Height - 1);
                e.Graphics.FillRectangle(new SolidBrush(color), 0, card.Height - 4, card.Width, 4);
            };
            card.Controls.Add(new Label { Text = val,   Font = new Font("Segoe UI", 26f, FontStyle.Bold), ForeColor = color,             AutoSize = false, Bounds = new Rectangle(0, 10, 175, 46), TextAlign = ContentAlignment.MiddleCenter });
            card.Controls.Add(new Label { Text = label, Font = AppTheme.FontSmall,                        ForeColor = AppTheme.TextMuted, AutoSize = false, Bounds = new Rectangle(0, 58, 175, 24), TextAlign = ContentAlignment.MiddleCenter });
            return card;
        }

        private Label GetCardVal(Panel card)
        {
            foreach (Control c in card.Controls)
                if (c is Label l && l.Font.Size >= 20) return l;
            return null;
        }

        private TextBox FI(Panel p, string lbl, int x, int w, bool pwd = false)
        {
            p.Controls.Add(new Label { Text = lbl, Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(x, 28) });
            var tb = AppTheme.MakeInput(w, pwd); tb.Location = new Point(x, 46); p.Controls.Add(tb); return tb;
        }

        private TextBox BFI(Panel p, string lbl, int x, int w)
        {
            p.Controls.Add(new Label { Text = lbl, Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(x, 26) });
            var tb = AppTheme.MakeInput(w); tb.Location = new Point(x, 44); p.Controls.Add(tb); return tb;
        }

        private bool Confirm(string msg) =>
            MessageBox.Show(msg, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

        private void Err(Exception ex) =>
            MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
