using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Library_Management_System
{
    public class UserDashboard : Form
    {
        // ── IDs ───────────────────────────────────────────────────
        private int    StudentID => SessionManager.CurrentUserID;
        private string EnrollNo  => SessionManager.EnrollmentNo;
        private string FullName  => SessionManager.CurrentFullName;

        // ── Layout ────────────────────────────────────────────────
        private Panel pnlContent;

        // ── Pages ─────────────────────────────────────────────────
        private Panel pgHome, pgSearch, pgBorrow, pgMyBooks, pgRequests, pgFines, pgProfile;

        // ── Home stats ────────────────────────────────────────────
        private Label lblHomeBorrowed, lblHomePending, lblHomeFine, lblHomeOverdue;

        // ── Search ────────────────────────────────────────────────
        private DataGridView dgvSearch;
        private TextBox txtSearch;
        private ComboBox cmbSearchCat;

        // ── Borrow Request ────────────────────────────────────────
        private TextBox txtBorrowISBN, txtBorrowTitle;
        private Label lblBorrowMsg;

        // ── My Books ──────────────────────────────────────────────
        private DataGridView dgvMyBooks;

        // ── My Requests ───────────────────────────────────────────
        private DataGridView dgvMyReqs;

        // ── Fines ─────────────────────────────────────────────────
        private DataGridView dgvFines;
        private Label lblFineTotal;

        // ── Profile ───────────────────────────────────────────────
        private Label lblProfId, lblProfName, lblProfEmail, lblProfPhone, lblProfDept, lblProfSem;
        private TextBox txtOldPass, txtNewPass, txtConfPass;

        // ── Category helper ───────────────────────────────────────
        private class CatItem { public int ID; public string Name; public override string ToString() => Name; }

        public UserDashboard()
        {
            Text = "AIUB Library — Student Portal";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            AppTheme.StyleForm(this);

           // BuildHeader();
            BuildNavBar();   
            BuildFooter();
            BuildContent();
            ShowPage(pgHome);
            LoadHomeStats();
        }

        // ═══════════════════════════════════════════════════════════
        // LAYOUT — Header / NavBar / Footer / Content
        // ═══════════════════════════════════════════════════════════
        private void BuildHeader()
        {
            Panel hdr = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppTheme.NavyDark };

            Label title = new Label
            {
                Text      = "AIUB Library Management System",
                Font      = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = AppTheme.TealLight,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(20, 0, 0, 0)
            };

            Label userLbl = new Label
            {
                Text      = "Student: " + EnrollNo + "  |  " + FullName,
                Font      = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock      = DockStyle.Right,
                Width     = 380,
                TextAlign = ContentAlignment.MiddleRight,
                Padding   = new Padding(0, 0, 20, 0)
            };

            hdr.Controls.Add(userLbl);
            hdr.Controls.Add(title);
            Controls.Add(hdr);
        }

        private void BuildNavBar()
        {
            Panel nav = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = AppTheme.NavyMid };

            // Teal accent line at bottom
            Panel line = new Panel { Dock = DockStyle.Bottom, Height = 3, BackColor = AppTheme.Teal };
            nav.Controls.Add(line);

            string[] items = { "Dashboard", "Search Books", "Request a Book", "My Borrowed Books", "My Requests", "My Fines", "My Profile", "Logout" };
            int x = 8;
            foreach (string item in items)
            {
                bool isLogout = item == "Logout";
                Button btn = new Button
                {
                    Text      = item,
                    Width     = item.Length > 10 ? 150 : 120,
                    Height    = 44,
                    Location  = new Point(x, 3),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = isLogout ? Color.FromArgb(120, 30, 30) : AppTheme.NavyMid,
                    ForeColor = Color.FromArgb(203, 213, 225),
                    Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor    = Cursors.Hand,
                    Tag       = item
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = isLogout ? Color.FromArgb(180, 30, 30) : AppTheme.NavyLight;
                btn.Click += NavBtn_Click;
                nav.Controls.Add(btn);
                x += btn.Width + 4;
            }

            Controls.Add(nav);
        }

        private void BuildFooter()
        {
            Panel ftr = new Panel { Dock = DockStyle.Bottom, Height = 28, BackColor = AppTheme.NavyDark };
            ftr.Controls.Add(new Label
            {
                Text      = "AIUB Library Management System  |  Student Portal",
                Font      = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(100, 116, 139),
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
            BuildPageSearch();
            BuildPageBorrow();
            BuildPageMyBooks();
            BuildPageMyRequests();
            BuildPageFines();
            BuildPageProfile();
        }

        private void NavBtn_Click(object sender, EventArgs e)
        {
            string tag = ((Button)sender).Tag?.ToString() ?? "";
            if (tag == "Logout")
            {
                if (MessageBox.Show("Logout?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SessionManager.Logout();
                    new LoginForm().Show();
                    Close();
                }
                return;
            }
            if      (tag == "Dashboard")         { ShowPage(pgHome);     LoadHomeStats(); }
            else if (tag == "Search Books")       { ShowPage(pgSearch); }
            else if (tag == "Request a Book")     { ShowPage(pgBorrow);   ClearBorrow(); }
            else if (tag == "My Borrowed Books")  { ShowPage(pgMyBooks);  LoadMyBooks(); }
            else if (tag == "My Requests")        { ShowPage(pgRequests); LoadMyRequests(); }
            else if (tag == "My Fines")           { ShowPage(pgFines);    LoadFines(); }
            else if (tag == "My Profile")         { ShowPage(pgProfile);  LoadProfile(); }
        }

        private void ShowPage(Panel pg)
        {
            foreach (Panel p in new[] { pgHome, pgSearch, pgBorrow, pgMyBooks, pgRequests, pgFines, pgProfile })
                p.Visible = false;
            pg.Visible = true;
        }

        // ═══════════════════════════════════════════════════════════
        // PAGE: HOME / DASHBOARD
        // ═══════════════════════════════════════════════════════════
        private void BuildPageHome()
        {
            pgHome = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = true };

            // ── Welcome banner ──────────────────────────────────
            Panel welcome = new Panel { Bounds = new Rectangle(0, 0, 2000, 70), BackColor = AppTheme.NavyDark, Dock = DockStyle.Top };
            Label wlbl = new Label
            {
                Text      = "Welcome back, " + FullName + "!   |   Enrollment: " + EnrollNo,
                Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = AppTheme.TealLight,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(24, 0, 0, 0)
            };
            Label dateLbl = new Label
            {
                Text      = DateTime.Now.ToString("dddd, dd MMMM yyyy"),
                Font      = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock      = DockStyle.Right,
                Width     = 260,
                TextAlign = ContentAlignment.MiddleRight,
                Padding   = new Padding(0, 0, 20, 0)
            };
            welcome.Controls.Add(dateLbl);
            welcome.Controls.Add(wlbl);
            pgHome.Controls.Add(welcome);

            // ── Stat cards ──────────────────────────────────────
            lblHomeBorrowed = StatVal(AppTheme.Teal);
            lblHomePending  = StatVal(AppTheme.Warning);
            lblHomeFine     = StatVal(AppTheme.Danger);
            lblHomeOverdue  = StatVal(AppTheme.Accent);

            Panel c1 = MakeCard(lblHomeBorrowed, "Books Borrowed",   AppTheme.Teal);
            Panel c2 = MakeCard(lblHomePending,  "Pending Requests", AppTheme.Warning);
            Panel c3 = MakeCard(lblHomeFine,     "Fine Due (BDT)",   AppTheme.Danger);
            Panel c4 = MakeCard(lblHomeOverdue,  "Overdue Books",    AppTheme.Accent);

            c1.Location = new Point(30, 88);
            c2.Location = new Point(220, 88);
            c3.Location = new Point(410, 88);
            c4.Location = new Point(600, 88);
            pgHome.Controls.AddRange(new Control[] { c1, c2, c3, c4 });

            // ── Quick actions ───────────────────────────────────
            Label qa = AppTheme.MakeSectionTitle("Quick Actions");
            qa.Location = new Point(30, 205);
            pgHome.Controls.Add(qa);

            Button bSrch  = AppTheme.MakePrimaryBtn("Search Books",  145, 42);
            Button bBrw   = AppTheme.MakeSuccessBtn("Request Book",  145, 42);
            Button bMyBks = AppTheme.MakeWarningBtn("My Books",      120, 42);
            bSrch.Location  = new Point(30,  240); bSrch.Click  += (s, e) => ShowPage(pgSearch);
            bBrw.Location   = new Point(190, 240); bBrw.Click   += (s, e) => { ShowPage(pgBorrow);  ClearBorrow(); };
            bMyBks.Location = new Point(350, 240); bMyBks.Click += (s, e) => { ShowPage(pgMyBooks); LoadMyBooks(); };
            pgHome.Controls.AddRange(new Control[] { bSrch, bBrw, bMyBks });

            // ── Due date alert grid ─────────────────────────────
            Label alertLbl = AppTheme.MakeSectionTitle("Due Date Alerts");
            alertLbl.Location = new Point(30, 305);
            pgHome.Controls.Add(alertLbl);

            DataGridView dgvAlert = new DataGridView
            {
                Location = new Point(30, 335),
                Size     = new Size(780, 200),
                Anchor   = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };
            AppTheme.StyleGrid(dgvAlert);
            pgHome.Controls.Add(dgvAlert);
            pgHome.Tag = dgvAlert;

            pnlContent.Controls.Add(pgHome);
        }

        private Label StatVal(Color c) =>
            new Label { Font = AppTheme.FontCard, ForeColor = c, Bounds = new Rectangle(0, 12, 175, 48), TextAlign = ContentAlignment.MiddleCenter, Text = "0" };

        private Panel MakeCard(Label val, string caption, Color accent)
        {
            Panel card = new Panel { Width = 175, Height = 95, BackColor = AppTheme.CardBg };
            card.Paint += (s, e) =>
            {
                using (Pen p = new Pen(accent, 3))
                    e.Graphics.DrawLine(p, 0, card.Height - 3, card.Width, card.Height - 3);
                e.Graphics.DrawRectangle(new Pen(AppTheme.BorderColor), 0, 0, card.Width - 1, card.Height - 1);
            };
            card.Controls.Add(val);
            card.Controls.Add(new Label { Text = caption, Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = false, Bounds = new Rectangle(0, 58, 175, 26), TextAlign = ContentAlignment.MiddleCenter });
            return card;
        }

        private void LoadHomeStats()
        {
            object r;
            r = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM BookIssue WHERE StudentID=@s AND ReturnedDate IS NULL", new SqlParameter("@s", StudentID));
            lblHomeBorrowed.Text = r != null ? r.ToString() : "0";

            r = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM BorrowRequest WHERE StudentID=@s AND Status='Pending'", new SqlParameter("@s", StudentID));
            lblHomePending.Text = r != null ? r.ToString() : "0";

            r = DatabaseHelper.ExecuteScalar("SELECT ISNULL(SUM(FineAmount),0) FROM BookIssue WHERE StudentID=@s AND ReturnedDate IS NULL AND FineAmount>0", new SqlParameter("@s", StudentID));
            lblHomeFine.Text = r != null ? r.ToString() : "0";

            r = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM BookIssue WHERE StudentID=@s AND ReturnedDate IS NULL AND DueDate < GETDATE()", new SqlParameter("@s", StudentID));
            lblHomeOverdue.Text = r != null ? r.ToString() : "0";

            if (pgHome.Tag is DataGridView dg)
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "SELECT b.BookName AS Title, i.IssuedDate, i.DueDate, DATEDIFF(day,GETDATE(),i.DueDate) AS DaysLeft, i.Status " +
                    "FROM BookIssue i JOIN Books b ON i.BookID=b.BookID " +
                    "WHERE i.StudentID=@s AND i.ReturnedDate IS NULL ORDER BY i.DueDate",
                    new SqlParameter("@s", StudentID));
                if (dt != null) dg.DataSource = dt;
            }
        }

        // ═══════════════════════════════════════════════════════════
        // PAGE: SEARCH BOOKS
        // ═══════════════════════════════════════════════════════════
        private void BuildPageSearch()
        {
            pgSearch = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false };
            pgSearch.Controls.Add(MakePageHeader("Search Books", "Find books in the library"));

            Panel tb = new Panel { Bounds = new Rectangle(20, 8, 980, 46), BackColor = AppTheme.Surface };

            txtSearch = AppTheme.MakeInput(260);
            SetPH(txtSearch, "Search by title or author");
            txtSearch.Location = new Point(0, 5);

            cmbSearchCat = new ComboBox { Location = new Point(270, 7), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.FontInput };
            LoadSearchCats();

            Button btnSrch = AppTheme.MakePrimaryBtn("Search", 90, 36);
            btnSrch.Location = new Point(460, 5);
            btnSrch.Click += (s, e) => SearchBooks();

            Button btnReq = AppTheme.MakeSuccessBtn("Request Selected", 155, 36);
            btnReq.Location = new Point(560, 5);
            btnReq.Click += (s, e) => QuickRequest();

            tb.Controls.AddRange(new Control[] { txtSearch, cmbSearchCat, btnSrch, btnReq });
            pgSearch.Controls.Add(tb);

            dgvSearch = new DataGridView { Bounds = new Rectangle(20, 60, 1020, 500), Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom };
            AppTheme.StyleGrid(dgvSearch);
            pgSearch.Controls.Add(dgvSearch);
            pnlContent.Controls.Add(pgSearch);

            pgSearch.VisibleChanged += (s, e) => { if (pgSearch.Visible && dgvSearch.Rows.Count == 0) SearchBooks(); };
        }

        private void LoadSearchCats()
        {
            cmbSearchCat.Items.Clear();
            cmbSearchCat.Items.Add(new CatItem { ID = 0, Name = "All Categories" });
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT CategoryID, CategoryName FROM Category WHERE IsActive=1 ORDER BY CategoryName");
            if (dt != null)
                foreach (DataRow r in dt.Rows)
                    cmbSearchCat.Items.Add(new CatItem { ID = Convert.ToInt32(r["CategoryID"]), Name = r["CategoryName"].ToString() });
            cmbSearchCat.SelectedIndex = 0;
        }

        private void SearchBooks()
        {
            string kw  = GetTxt(txtSearch, "Search by title or author");
            int    cat = cmbSearchCat.SelectedItem != null ? ((CatItem)cmbSearchCat.SelectedItem).ID : 0;
            DataTable dt;

            if (kw == "" && cat == 0)
                dt = DatabaseHelper.ExecuteQuery(
                    "SELECT b.BookID, b.BookName AS Title, b.Author, b.ISBN, b.AvailableQuantity AS Available, c.CategoryName AS Category " +
                    "FROM Books b LEFT JOIN Category c ON b.CategoryID=c.CategoryID ORDER BY b.BookName");
            else if (kw == "" && cat != 0)
                dt = DatabaseHelper.ExecuteQuery(
                    "SELECT b.BookID, b.BookName AS Title, b.Author, b.ISBN, b.AvailableQuantity AS Available, c.CategoryName AS Category " +
                    "FROM Books b LEFT JOIN Category c ON b.CategoryID=c.CategoryID WHERE b.CategoryID=@c ORDER BY b.BookName",
                    new SqlParameter("@c", cat));
            else if (kw != "" && cat == 0)
                dt = DatabaseHelper.ExecuteQuery(
                    "SELECT b.BookID, b.BookName AS Title, b.Author, b.ISBN, b.AvailableQuantity AS Available, c.CategoryName AS Category " +
                    "FROM Books b LEFT JOIN Category c ON b.CategoryID=c.CategoryID WHERE b.BookName LIKE @k OR b.Author LIKE @k ORDER BY b.BookName",
                    new SqlParameter("@k", "%" + kw + "%"));
            else
                dt = DatabaseHelper.ExecuteQuery(
                    "SELECT b.BookID, b.BookName AS Title, b.Author, b.ISBN, b.AvailableQuantity AS Available, c.CategoryName AS Category " +
                    "FROM Books b LEFT JOIN Category c ON b.CategoryID=c.CategoryID WHERE (b.BookName LIKE @k OR b.Author LIKE @k) AND b.CategoryID=@c ORDER BY b.BookName",
                    new SqlParameter("@k", "%" + kw + "%"), new SqlParameter("@c", cat));

            if (dt != null) dgvSearch.DataSource = dt;
        }

        private void QuickRequest()
        {
            if (dgvSearch.SelectedRows.Count == 0) { MessageBox.Show("Select a book first."); return; }
            int    bookID = Convert.ToInt32(dgvSearch.SelectedRows[0].Cells["BookID"].Value);
            int    avail  = Convert.ToInt32(dgvSearch.SelectedRows[0].Cells["Available"].Value);
            string title  = dgvSearch.SelectedRows[0].Cells["Title"].Value.ToString();
            if (avail < 1) { MessageBox.Show("Sorry, no copies available right now."); return; }
            object ex = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM BorrowRequest WHERE StudentID=@s AND BookID=@b AND Status='Pending'",
                new SqlParameter("@s", StudentID), new SqlParameter("@b", bookID));
            if (ex != null && Convert.ToInt32(ex) > 0) { MessageBox.Show("You already have a pending request for this book."); return; }
            if (DatabaseHelper.ExecuteNonQuery("INSERT INTO BorrowRequest(BookID,StudentID,RequestDate,Status) VALUES(@b,@s,GETDATE(),'Pending')",
                new SqlParameter("@b", bookID), new SqlParameter("@s", StudentID)))
                MessageBox.Show("Request submitted for '" + title + "'!");
        }

        // ═══════════════════════════════════════════════════════════
        // PAGE: REQUEST A BOOK
        // ═══════════════════════════════════════════════════════════
        private void BuildPageBorrow()
        {
            pgBorrow = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false };
            pgBorrow.Controls.Add(MakePageHeader("Request a Book", "Submit a borrow request to the librarian"));

            int lx = 60, fx = 250, y = 100, gap = 52;

            pgBorrow.Controls.Add(L("Book ISBN", lx, y));
            txtBorrowISBN = AppTheme.MakeInput(300); txtBorrowISBN.Location = new Point(fx, y); pgBorrow.Controls.Add(txtBorrowISBN); y += gap;

            pgBorrow.Controls.Add(L("OR Book Title", lx, y));
            txtBorrowTitle = AppTheme.MakeInput(300); txtBorrowTitle.Location = new Point(fx, y); pgBorrow.Controls.Add(txtBorrowTitle); y += gap;

            Button btnLookup = AppTheme.MakeWarningBtn("Lookup Book", 130, 38);
            btnLookup.Location = new Point(fx, y);
            btnLookup.Click += (s, e) => LookupBook();
            pgBorrow.Controls.Add(btnLookup);
            y += 55;

            Button btnSubmit = AppTheme.MakePrimaryBtn("Submit Request", 155, 42);
            btnSubmit.Location = new Point(fx, y);
            btnSubmit.Click += (s, e) => SubmitRequest();
            pgBorrow.Controls.Add(btnSubmit);

            lblBorrowMsg = new Label { Bounds = new Rectangle(fx, y + 54, 550, 28), Font = AppTheme.FontBody, ForeColor = AppTheme.Success, Text = "" };
            pgBorrow.Controls.Add(lblBorrowMsg);
            pnlContent.Controls.Add(pgBorrow);
        }

        private void ClearBorrow() { txtBorrowISBN.Text = txtBorrowTitle.Text = ""; lblBorrowMsg.Text = ""; }

        private void LookupBook()
        {
            string isbn  = txtBorrowISBN.Text.Trim();
            string title = txtBorrowTitle.Text.Trim();
            if (isbn == "" && title == "") { MessageBox.Show("Enter ISBN or title."); return; }
            DataTable dt = isbn != ""
                ? DatabaseHelper.ExecuteQuery("SELECT BookName, Author, AvailableQuantity FROM Books WHERE ISBN=@i", new SqlParameter("@i", isbn))
                : DatabaseHelper.ExecuteQuery("SELECT BookName, Author, AvailableQuantity FROM Books WHERE BookName LIKE @t", new SqlParameter("@t", "%" + title + "%"));
            if (dt == null || dt.Rows.Count == 0) { lblBorrowMsg.ForeColor = AppTheme.Danger; lblBorrowMsg.Text = "Book not found."; return; }
            int avail = Convert.ToInt32(dt.Rows[0]["AvailableQuantity"]);
            lblBorrowMsg.ForeColor = avail > 0 ? AppTheme.Success : AppTheme.Warning;
            lblBorrowMsg.Text = "Found: " + dt.Rows[0]["BookName"] + "  by  " + dt.Rows[0]["Author"] + "   |   Available copies: " + avail;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // UserDashboard
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "UserDashboard";
            this.Load += new System.EventHandler(this.UserDashboard_Load);
            this.ResumeLayout(false);

        }

        private void UserDashboard_Load(object sender, EventArgs e)
        {

        }

        private void SubmitRequest()
        {
            string isbn  = txtBorrowISBN.Text.Trim();
            string title = txtBorrowTitle.Text.Trim();
            if (isbn == "" && title == "") { MessageBox.Show("Enter ISBN or title.", "Validation"); return; }
            DataTable bt = isbn != ""
                ? DatabaseHelper.ExecuteQuery("SELECT BookID, AvailableQuantity FROM Books WHERE ISBN=@i", new SqlParameter("@i", isbn))
                : DatabaseHelper.ExecuteQuery("SELECT TOP 1 BookID, AvailableQuantity FROM Books WHERE BookName LIKE @t", new SqlParameter("@t", "%" + title + "%"));
            if (bt == null || bt.Rows.Count == 0) { lblBorrowMsg.ForeColor = AppTheme.Danger; lblBorrowMsg.Text = "Book not found."; return; }
            int bookID = Convert.ToInt32(bt.Rows[0]["BookID"]);
            int avail  = Convert.ToInt32(bt.Rows[0]["AvailableQuantity"]);
            if (avail < 1) { lblBorrowMsg.ForeColor = AppTheme.Warning; lblBorrowMsg.Text = "Book not available."; return; }
            object ex = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM BorrowRequest WHERE StudentID=@s AND BookID=@b AND Status='Pending'",
                new SqlParameter("@s", StudentID), new SqlParameter("@b", bookID));
            if (ex != null && Convert.ToInt32(ex) > 0) { lblBorrowMsg.ForeColor = AppTheme.Warning; lblBorrowMsg.Text = "Already have a pending request for this book."; return; }
            if (DatabaseHelper.ExecuteNonQuery("INSERT INTO BorrowRequest(BookID,StudentID,RequestDate,Status) VALUES(@b,@s,GETDATE(),'Pending')",
                new SqlParameter("@b", bookID), new SqlParameter("@s", StudentID)))
            { lblBorrowMsg.ForeColor = AppTheme.Success; lblBorrowMsg.Text = "Request submitted successfully!"; txtBorrowISBN.Text = txtBorrowTitle.Text = ""; LoadHomeStats(); }
        }

        // ═══════════════════════════════════════════════════════════
        // PAGE: MY BORROWED BOOKS
        // ═══════════════════════════════════════════════════════════
        private void BuildPageMyBooks()
        {
            pgMyBooks = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false };
            pgMyBooks.Controls.Add(MakePageHeader("My Borrowed Books", "Books you currently have issued"));
            dgvMyBooks = new DataGridView { Bounds = new Rectangle(20, 82, 1020, 540), Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom };
            AppTheme.StyleGrid(dgvMyBooks);
            pgMyBooks.Controls.Add(dgvMyBooks);
            pnlContent.Controls.Add(pgMyBooks);
        }

        private void LoadMyBooks()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT b.BookName AS Title, b.Author, i.IssuedDate, i.DueDate, i.Status, " +
                "CASE WHEN i.DueDate < GETDATE() AND i.ReturnedDate IS NULL THEN DATEDIFF(day,i.DueDate,GETDATE())*5 ELSE 0 END AS EstFineTK " +
                "FROM BookIssue i JOIN Books b ON i.BookID=b.BookID " +
                "WHERE i.StudentID=@s AND i.ReturnedDate IS NULL ORDER BY i.DueDate",
                new SqlParameter("@s", StudentID));
            if (dt != null) dgvMyBooks.DataSource = dt;
        }

        // ═══════════════════════════════════════════════════════════
        // PAGE: MY REQUESTS
        // ═══════════════════════════════════════════════════════════
        private void BuildPageMyRequests()
        {
            pgRequests = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false };
           // pgRequests.Controls.Add(MakePageHeader("My Borrow Requests", "Track and manage your book requests"));

            // toolbar কে Dock.Top দাও, fixed position না
            Panel tb = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = AppTheme.Surface, Padding = new Padding(50, 4, 0, 0) };

            Button btnRef = AppTheme.MakePrimaryBtn("Refresh", 100, 36);
            btnRef.Location = new Point(0, 4);
            btnRef.Click += (s, e) => LoadMyRequests();

            Button btnCan = AppTheme.MakeDangerBtn("Cancel Request", 150, 36);
            btnCan.Location = new Point(110, 4);
            btnCan.Click += (s, e) => CancelRequest();

            tb.Controls.AddRange(new Control[] { btnRef, btnCan });
            pgRequests.Controls.Add(tb);

            // DataGridView কে Dock.Fill দাও
            dgvMyReqs = new DataGridView { Dock = DockStyle.Fill };
            AppTheme.StyleGrid(dgvMyReqs);
            pgRequests.Controls.Add(dgvMyReqs);
            pnlContent.Controls.Add(pgRequests);
        }

        private void LoadMyRequests()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT r.RequestID, b.BookName AS Title, b.Author, r.RequestDate, r.Status " +
                "FROM BorrowRequest r JOIN Books b ON r.BookID=b.BookID " +
                "WHERE r.StudentID=@s ORDER BY r.RequestDate DESC",
                new SqlParameter("@s", StudentID));
            if (dt != null)
            {
                dgvMyReqs.DataSource = dt;
                if (dgvMyReqs.Columns.Contains("RequestID"))
                    dgvMyReqs.Columns["RequestID"].Visible = false;
            }
        }

        private void CancelRequest()
        {
            if (dgvMyReqs.SelectedRows.Count == 0) { MessageBox.Show("Select a request to cancel."); return; }
            int    reqID = Convert.ToInt32(dgvMyReqs.SelectedRows[0].Cells["RequestID"].Value);
            string stat  = dgvMyReqs.SelectedRows[0].Cells["Status"].Value.ToString();
            if (stat != "Pending") { MessageBox.Show("Only pending requests can be cancelled."); return; }
            if (MessageBox.Show("Cancel this request?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            if (DatabaseHelper.ExecuteNonQuery("DELETE FROM BorrowRequest WHERE RequestID=@id AND StudentID=@s",
                new SqlParameter("@id", reqID), new SqlParameter("@s", StudentID)))
            { MessageBox.Show("Request cancelled."); LoadMyRequests(); LoadHomeStats(); }
        }

        // ═══════════════════════════════════════════════════════════
        // PAGE: FINES
        // ═══════════════════════════════════════════════════════════
        private void BuildPageFines()
        {
            pgFines = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false };
            pgFines.Controls.Add(MakePageHeader("My Fines", "Outstanding and paid fine details"));

            lblFineTotal = new Label { Bounds = new Rectangle(20, 82, 600, 30), Font = AppTheme.FontH3, ForeColor = AppTheme.Danger, Text = "Total Outstanding: BDT 0" };
            pgFines.Controls.Add(lblFineTotal);

            dgvFines = new DataGridView { Bounds = new Rectangle(20, 120, 1020, 490), Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom };
            AppTheme.StyleGrid(dgvFines);
            pgFines.Controls.Add(dgvFines);
            pnlContent.Controls.Add(pgFines);
        }

        private void LoadFines()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT b.BookName AS Title, i.IssuedDate, i.DueDate, i.ReturnedDate, ISNULL(i.FineAmount,0) AS FineAmount, i.Status " +
                "FROM BookIssue i JOIN Books b ON i.BookID=b.BookID WHERE i.StudentID=@s ORDER BY i.DueDate DESC",
                new SqlParameter("@s", StudentID));
            if (dt != null) dgvFines.DataSource = dt;

            object total = DatabaseHelper.ExecuteScalar(
                "SELECT ISNULL(SUM(FineAmount),0) FROM BookIssue WHERE StudentID=@s AND ReturnedDate IS NULL AND FineAmount>0",
                new SqlParameter("@s", StudentID));
            lblFineTotal.Text = "Total Outstanding Fine: BDT " + (total != null ? total.ToString() : "0");
        }

        // ═══════════════════════════════════════════════════════════
        // PAGE: PROFILE
        // ═══════════════════════════════════════════════════════════
        private void BuildPageProfile()
        {
            pgProfile = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false };
            pgProfile.Controls.Add(MakePageHeader("My Profile", "View your info and change your password"));

            int lx = 60, fx = 240, y = 90, gap = 46;

            pgProfile.Controls.Add(IL("Enrollment No:",  lx, y)); lblProfId    = VL(fx, y); pgProfile.Controls.Add(lblProfId);    y += gap;
            pgProfile.Controls.Add(IL("Full Name:",      lx, y)); lblProfName  = VL(fx, y); pgProfile.Controls.Add(lblProfName);  y += gap;
            pgProfile.Controls.Add(IL("Email:",          lx, y)); lblProfEmail = VL(fx, y); pgProfile.Controls.Add(lblProfEmail); y += gap;
            pgProfile.Controls.Add(IL("Phone:",          lx, y)); lblProfPhone = VL(fx, y); pgProfile.Controls.Add(lblProfPhone); y += gap;
            pgProfile.Controls.Add(IL("Department:",     lx, y)); lblProfDept  = VL(fx, y); pgProfile.Controls.Add(lblProfDept);  y += gap;
            pgProfile.Controls.Add(IL("Semester:",       lx, y)); lblProfSem   = VL(fx, y); pgProfile.Controls.Add(lblProfSem);   y += gap + 10;

            pgProfile.Controls.Add(new Panel { Bounds = new Rectangle(lx, y, 560, 2), BackColor = AppTheme.BorderColor }); y += 18;
            Label ch = AppTheme.MakeSectionTitle("Change Password"); ch.Location = new Point(lx, y); pgProfile.Controls.Add(ch); y += 36;

            pgProfile.Controls.Add(L("Current Password", lx, y)); txtOldPass  = AppTheme.MakeInput(280, true); txtOldPass.Location  = new Point(fx, y); pgProfile.Controls.Add(txtOldPass);  y += gap;
            pgProfile.Controls.Add(L("New Password",     lx, y)); txtNewPass  = AppTheme.MakeInput(280, true); txtNewPass.Location  = new Point(fx, y); pgProfile.Controls.Add(txtNewPass);  y += gap;
            pgProfile.Controls.Add(L("Confirm Password", lx, y)); txtConfPass = AppTheme.MakeInput(280, true); txtConfPass.Location = new Point(fx, y); pgProfile.Controls.Add(txtConfPass); y += gap;

            Button btnCP = AppTheme.MakePrimaryBtn("Change Password", 175, 40);
            btnCP.Location = new Point(fx, y); btnCP.Click += (s, e) => ChangePassword();
            pgProfile.Controls.Add(btnCP);
            pnlContent.Controls.Add(pgProfile);
        }

        private void LoadProfile()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT EnrollmentNo, FirstName+' '+ISNULL(LastName,'') AS FullName, Email, PhoneNo, Department, Semester FROM Student WHERE StudentID=@id",
                new SqlParameter("@id", StudentID));
            if (dt == null || dt.Rows.Count == 0) return;
            DataRow r = dt.Rows[0];
            lblProfId.Text    = r["EnrollmentNo"].ToString();
            lblProfName.Text  = r["FullName"].ToString();
            lblProfEmail.Text = r["Email"]      == DBNull.Value ? "—" : r["Email"].ToString();
            lblProfPhone.Text = r["PhoneNo"]    == DBNull.Value ? "—" : r["PhoneNo"].ToString();
            lblProfDept.Text  = r["Department"] == DBNull.Value ? "—" : r["Department"].ToString();
            lblProfSem.Text   = r["Semester"]   == DBNull.Value ? "—" : r["Semester"].ToString();
            txtOldPass.Text   = txtNewPass.Text = txtConfPass.Text = "";
        }

        private void ChangePassword()
        {
            if (string.IsNullOrWhiteSpace(txtOldPass.Text) || string.IsNullOrWhiteSpace(txtNewPass.Text))
            { MessageBox.Show("Fill all password fields."); return; }
            if (txtNewPass.Text != txtConfPass.Text) { MessageBox.Show("Passwords do not match."); return; }
            object chk = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Student WHERE StudentID=@id AND Password=@old",
                new SqlParameter("@id", StudentID), new SqlParameter("@old", txtOldPass.Text));
            if (chk == null || Convert.ToInt32(chk) == 0) { MessageBox.Show("Current password incorrect."); return; }
            if (DatabaseHelper.ExecuteNonQuery("UPDATE Student SET Password=@np WHERE StudentID=@id",
                new SqlParameter("@np", txtNewPass.Text), new SqlParameter("@id", StudentID)))
            { MessageBox.Show("Password changed successfully!"); txtOldPass.Text = txtNewPass.Text = txtConfPass.Text = ""; }
        }

        // ═══════════════════════════════════════════════════════════
        // HELPERS
        // ═══════════════════════════════════════════════════════════
        private Panel MakePageHeader(string title, string sub)
        {
            Panel p = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = AppTheme.NavyDark };
            p.Controls.Add(new Label { Text = title, Font = AppTheme.FontH2, ForeColor = AppTheme.TealLight, AutoSize = false, Bounds = new Rectangle(20, 10, 700, 30), TextAlign = ContentAlignment.MiddleLeft });
            p.Controls.Add(new Label { Text = sub,   Font = AppTheme.FontSmall, ForeColor = Color.FromArgb(148,163,184), AutoSize = false, Bounds = new Rectangle(22, 40, 700, 22), TextAlign = ContentAlignment.MiddleLeft });
            return p;
        }

        private Label L(string text, int x, int y)  => new Label { Text = text, Font = AppTheme.FontLabel,  ForeColor = AppTheme.TextPrimary, Location = new Point(x, y + 4), AutoSize = true };
        private Label IL(string text, int x, int y) => new Label { Text = text, Font = AppTheme.FontLabel,  ForeColor = AppTheme.TextMuted,   Location = new Point(x, y),     AutoSize = true };
        private Label VL(int x, int y)              => new Label { Font = AppTheme.FontH3, ForeColor = AppTheme.TextPrimary, Location = new Point(x, y - 2), AutoSize = true };

        private void SetPH(TextBox tb, string ph)
        {
            tb.Text = ph; tb.ForeColor = AppTheme.TextMuted;
            tb.GotFocus  += (s, e) => { if (tb.ForeColor == AppTheme.TextMuted) { tb.Text = ""; tb.ForeColor = AppTheme.TextPrimary; } };
            tb.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(tb.Text))  { tb.Text = ph; tb.ForeColor = AppTheme.TextMuted; } };
        }
        private string GetTxt(TextBox tb, string ph) => tb.ForeColor == AppTheme.TextMuted ? "" : tb.Text.Trim();
    }
}
