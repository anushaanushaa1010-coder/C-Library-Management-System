using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Library_Management_System
{
    public class LibrarianDashboard : Form
    {
        // ── IDs ───────────────────────────────────────────────────
        private int LibrarianID => SessionManager.CurrentUserID;

        // ── Layout ────────────────────────────────────────────────
        private Panel pnlSidebar;
        private Panel pnlContent;

        // ── Pages ─────────────────────────────────────────────────
        private Panel pgHome, pgBooks, pgAddBook, pgIssue, pgReturn, pgStudents, pgRequests, pgReports, pgProfile;

        // ── Home stats ────────────────────────────────────────────
        private Label lblTotalBooks, lblIssuedBooks, lblPendingReq, lblTotalStudents;

        // ── Books page ────────────────────────────────────────────
        private DataGridView dgvBooks;
        private TextBox txtBookSearch;

        // ── Add/Edit Book ─────────────────────────────────────────
        private Label lblAddBookTitle;
        private TextBox txtBName, txtBAuthor, txtBISBN, txtBPub, txtBQty, txtBPrice, txtBDesc;
        private ComboBox cmbBCat;
        private int _editBookID = 0;

        // ── Issue Book ────────────────────────────────────────────
        private TextBox txtIssEnroll, txtIssISBN;
        private DateTimePicker dtpDue;
        private Label lblIssMsg;

        // ── Return Book ───────────────────────────────────────────
        private DataGridView dgvIssued;
        private TextBox txtRetSearch;
        private Label lblRetMsg;

        // ── Students ──────────────────────────────────────────────
        private DataGridView dgvStudents;
        private TextBox txtStudSearch;

        // ── Requests ──────────────────────────────────────────────
        private DataGridView dgvRequests;

        // ── Reports ───────────────────────────────────────────────
        private DataGridView dgvReport;
        private DateTimePicker dtpRptFrom, dtpRptTo;

        // ── Profile ───────────────────────────────────────────────
        private Label lblProfName, lblProfEmail, lblProfPhone, lblProfId;
        private TextBox txtOldPass, txtNewPass, txtConfPass;

        // ── Category helper ───────────────────────────────────────
        private class CatItem { public int ID; public string Name; public override string ToString() => Name; }

        public LibrarianDashboard()
        {
            Text = "AIUB Library — Librarian";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            AppTheme.StyleForm(this);

            BuildFooter();
            BuildSidebar();
           // BuildHeader();
            BuildContent();
            ShowPage(pgHome);
            LoadHomeStats();
        }

        // ═══════════════════════════════════════════════════════════
        // LAYOUT
        // ═══════════════════════════════════════════════════════════
        private void BuildHeader()
        {
            Panel hdr = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = AppTheme.NavyDark };
            Label t = new Label
            {
                Text = "AIUB Library Management System",
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = AppTheme.TealLight,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            Label userInfo = new Label
            {
                Text = "Librarian: " + SessionManager.CurrentUserName + "  |  " + SessionManager.CurrentFullName,
                Font = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock = DockStyle.Right,
                Width = 360,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 20, 0)
            };
            hdr.Controls.Add(userInfo);
            hdr.Controls.Add(t);
            Controls.Add(hdr);
        }

        private void BuildFooter()
        {
            Panel ftr = new Panel { Dock = DockStyle.Bottom, Height = 24, BackColor = AppTheme.NavyDark };
            ftr.Controls.Add(new Label { Text = "Librarian Portal  |  AIUB Library", Font = AppTheme.FontSmall, ForeColor = Color.FromArgb(100, 116, 139), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
            Controls.Add(ftr);
        }

        private void BuildSidebar()
        {
            // Top navigation bar (horizontal) so content sits below
            pnlSidebar = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = AppTheme.NavyMid };
            var sep = new Panel { Height = 3, Dock = DockStyle.Bottom, BackColor = AppTheme.Teal };
            pnlSidebar.Controls.Add(sep);

            string[] items = { "Dashboard", "All Books", "Add Book", "Issue Book", "Return Book", "Students", "Requests", "Reports", "My Profile", "Logout" };
            int x = 10;
            foreach (string item in items)
            {
                bool isLogout = item.Contains("Logout");
                var btn = new Button
                {
                    Text = item,
                    Width = 130,
                    Height = 48,
                    Location = new Point(x, 4),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = isLogout ? Color.FromArgb(120, 30, 30) : AppTheme.NavyMid,
                    ForeColor = Color.FromArgb(203, 213, 225),
                    Font = new Font(AppTheme.FontBody.FontFamily, 9f),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Padding = new Padding(0),
                    Cursor = Cursors.Hand,
                    Tag = item,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = isLogout ? Color.FromArgb(180, 30, 30) : AppTheme.NavyLight;
                btn.Click += SidebarBtn_Click;
                pnlSidebar.Controls.Add(btn);
                x += 140;
            }

            // Sidebar এর শেষে Controls.Add(pnlSidebar) এর আগে:
            var lblName = new Label
            {
                Text = "Librarian: " + SessionManager.CurrentFullName,
                Font = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock = DockStyle.Right,
                Width = 220,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 16, 0)
            };
            pnlSidebar.Controls.Add(lblName);


            Controls.Add(pnlSidebar);
        }

        private void BuildContent()
        {
            pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface };
            Controls.Add(pnlContent);
            BuildPageHome();
            BuildPageBooks();
            BuildPageAddBook();
            BuildPageIssue();
            BuildPageReturn();
            BuildPageStudents();
            BuildPageRequests();
            BuildPageReports();
            BuildPageProfile();
        }

        private void SidebarBtn_Click(object sender, EventArgs e)
        {
            string txt = ((Button)sender).Text.Trim();
            if (txt.Contains("Logout"))
            {
                if (MessageBox.Show("Logout?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SessionManager.Logout();
                    new LoginForm().Show();
                    Close();
                }
                return;
            }
            if (txt.Contains("Dashboard")) { ShowPage(pgHome); LoadHomeStats(); }
            else if (txt.Contains("All Books")) { ShowPage(pgBooks); LoadBooks(""); }
            else if (txt.Contains("Add Book")) { ShowPage(pgAddBook); ClearBookForm(); }
            else if (txt.Contains("Issue")) { ShowPage(pgIssue); lblIssMsg.Text = ""; }
            else if (txt.Contains("Return")) { ShowPage(pgReturn); LoadIssued(""); }
            else if (txt.Contains("Students")) { ShowPage(pgStudents); LoadStudents(""); }
            else if (txt.Contains("Requests")) { ShowPage(pgRequests); LoadRequests(); }
            else if (txt.Contains("Reports")) { ShowPage(pgReports); }
            else if (txt.Contains("Profile")) { ShowPage(pgProfile); LoadProfile(); }
        }

        private void ShowPage(Panel pg)
        {
            foreach (Panel p in new[] { pgHome, pgBooks, pgAddBook, pgIssue, pgReturn, pgStudents, pgRequests, pgReports, pgProfile })
                p.Visible = false;
            pg.Visible = true;
        }

        // ═══════════════════════════════════════════════════════════
        // DASHBOARD HOME
        // ═══════════════════════════════════════════════════════════
        private void BuildPageHome()
        {
            pgHome = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = true };

            // Welcome banner
            Panel welcome = new Panel { Location = new Point(0, 0), Height = 68, Width = 2000, BackColor = AppTheme.NavyDark };
            welcome.Controls.Add(new Label { Text = "Welcome, " + SessionManager.CurrentFullName + "!   |   " + DateTime.Now.ToString("dddd, dd MMMM yyyy"), Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = AppTheme.TealLight, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(24, 0, 0, 0) });
            pgHome.Controls.Add(welcome);

            lblTotalBooks = MakeStatVal(AppTheme.Teal);
            lblIssuedBooks = MakeStatVal(AppTheme.Accent);
            lblPendingReq = MakeStatVal(AppTheme.Warning);
            lblTotalStudents = MakeStatVal(AppTheme.Success);

            Panel c1 = MakeCard(lblTotalBooks, "Total Books", AppTheme.Teal);
            Panel c2 = MakeCard(lblIssuedBooks, "Currently Issued", AppTheme.Accent);
            Panel c3 = MakeCard(lblPendingReq, "Pending Requests", AppTheme.Warning);
            Panel c4 = MakeCard(lblTotalStudents, "Students", AppTheme.Success);

            c1.Location = new Point(30, 90); c2.Location = new Point(215, 90);
            c3.Location = new Point(400, 90); c4.Location = new Point(585, 90);
            pgHome.Padding = new Padding(0, 68, 0, 0);
            pgHome.Controls.AddRange(new Control[] { c1, c2, c3, c4 });

            Label qa = AppTheme.MakeSectionTitle("Quick Actions");
            qa.Location = new Point(30, 210);
            pgHome.Controls.Add(qa);

            Button bIssue = AppTheme.MakePrimaryBtn("Issue a Book", 140, 40);
            Button bReturn = AppTheme.MakeSuccessBtn("Return Book", 140, 40);
            Button bReq = AppTheme.MakeWarningBtn("View Requests", 140, 40);
            bIssue.Location = new Point(30, 250); bIssue.Click += (s, e) => { ShowPage(pgIssue); lblIssMsg.Text = ""; };
            bReturn.Location = new Point(185, 250); bReturn.Click += (s, e) => { ShowPage(pgReturn); LoadIssued(""); };
            bReq.Location = new Point(340, 250); bReq.Click += (s, e) => { ShowPage(pgRequests); LoadRequests(); };
            pgHome.Controls.AddRange(new Control[] { bIssue, bReturn, bReq });

            pnlContent.Controls.Add(pgHome);
        }

        private Label MakeStatVal(Color c) => new Label { Font = AppTheme.FontCard, ForeColor = c, Bounds = new Rectangle(0, 12, 170, 48), TextAlign = ContentAlignment.MiddleCenter, Text = "0" };

        private Panel MakeCard(Label val, string caption, Color accent)
        {
            Panel card = new Panel { Width = 170, Height = 95, BackColor = AppTheme.CardBg };
            card.Paint += (s, e) =>
            {
                using (Pen p = new Pen(accent, 3)) e.Graphics.DrawLine(p, 0, card.Height - 3, card.Width, card.Height - 3);
                e.Graphics.DrawRectangle(new Pen(AppTheme.BorderColor), 0, 0, card.Width - 1, card.Height - 1);
            };
            card.Controls.Add(val);
            card.Controls.Add(new Label { Text = caption, Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = false, Bounds = new Rectangle(0, 58, 170, 26), TextAlign = ContentAlignment.MiddleCenter });
            return card;
        }

        private void LoadHomeStats()
        {
            object r;
            r = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Books");
            lblTotalBooks.Text = r != null ? r.ToString() : "0";
            r = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM BookIssue WHERE ReturnedDate IS NULL");
            lblIssuedBooks.Text = r != null ? r.ToString() : "0";
            r = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM BorrowRequest WHERE Status='Pending'");
            lblPendingReq.Text = r != null ? r.ToString() : "0";
            r = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Student WHERE IsActive=1");
            lblTotalStudents.Text = r != null ? r.ToString() : "0";
        }

        // ═══════════════════════════════════════════════════════════
        // ALL BOOKS
        // ═══════════════════════════════════════════════════════════
        private void BuildPageBooks()
        {
            pgBooks = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false };
            pgBooks.Controls.Add(AppTheme.MakeHeader("Book Management", "View, edit and delete books"));

            Panel tb = new Panel { Bounds = new Rectangle(20, 78, 800, 44), BackColor = AppTheme.Surface };
            txtBookSearch = AppTheme.MakeInput(240);
            SetPlaceholder(txtBookSearch, "Search title / author / ISBN");
            txtBookSearch.Location = new Point(0, 4);

            Button btnSrch = AppTheme.MakePrimaryBtn("Search", 90, 36);
            btnSrch.Location = new Point(250, 4);
            btnSrch.Click += (s, e) => LoadBooks(GetTxt(txtBookSearch, "Search title / author / ISBN"));

            Button btnAdd = AppTheme.MakeSuccessBtn("Add Book", 100, 36);
            btnAdd.Location = new Point(350, 4);
            btnAdd.Click += (s, e) => { ShowPage(pgAddBook); ClearBookForm(); };

            Button btnEdit = AppTheme.MakeWarningBtn("Edit", 80, 36);
            btnEdit.Location = new Point(460, 4);
            btnEdit.Click += (s, e) => EditBook();

            Button btnDel = AppTheme.MakeDangerBtn("Delete", 90, 36);
            btnDel.Location = new Point(550, 4);
            btnDel.Click += (s, e) => DeleteBook();

            tb.Controls.AddRange(new Control[] { txtBookSearch, btnSrch, btnAdd, btnEdit, btnDel });
            pgBooks.Controls.Add(tb);

            dgvBooks = new DataGridView { Bounds = new Rectangle(20, 126, 1020, 480), Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom };
            AppTheme.StyleGrid(dgvBooks);
            pgBooks.Controls.Add(dgvBooks);
            pnlContent.Controls.Add(pgBooks);
        }

        private void LoadBooks(string search)
        {
            DataTable dt;
            if (string.IsNullOrWhiteSpace(search))
                dt = DatabaseHelper.ExecuteQuery(
                    "SELECT b.BookID, b.BookName AS Title, b.Author, b.ISBN, b.TotalQuantity AS Qty, b.AvailableQuantity AS Available, c.CategoryName AS Category " +
                    "FROM Books b LEFT JOIN Category c ON b.CategoryID=c.CategoryID ORDER BY b.BookName");
            else
                dt = DatabaseHelper.ExecuteQuery(
                    "SELECT b.BookID, b.BookName AS Title, b.Author, b.ISBN, b.TotalQuantity AS Qty, b.AvailableQuantity AS Available, c.CategoryName AS Category " +
                    "FROM Books b LEFT JOIN Category c ON b.CategoryID=c.CategoryID " +
                    "WHERE b.BookName LIKE @s OR b.Author LIKE @s OR b.ISBN LIKE @s ORDER BY b.BookName",
                    new SqlParameter("@s", "%" + search + "%"));
            if (dt != null) dgvBooks.DataSource = dt;
        }

        private void EditBook()
        {
            if (dgvBooks.SelectedRows.Count == 0) { MessageBox.Show("Select a book first."); return; }
            int id = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["BookID"].Value);
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Books WHERE BookID=@id", new SqlParameter("@id", id));
            if (dt == null || dt.Rows.Count == 0) return;
            DataRow r = dt.Rows[0];
            _editBookID = id;
            lblAddBookTitle.Text = "Edit Book";
            txtBName.Text = r["BookName"].ToString();
            txtBAuthor.Text = r["Author"].ToString();
            txtBISBN.Text = r["ISBN"] == DBNull.Value ? "" : r["ISBN"].ToString();
            txtBPub.Text = r["Publication"] == DBNull.Value ? "" : r["Publication"].ToString();
            txtBQty.Text = r["TotalQuantity"].ToString();
            txtBPrice.Text = r["Price"] == DBNull.Value ? "" : r["Price"].ToString();
            txtBDesc.Text = "";
            int catId = r["CategoryID"] == DBNull.Value ? 0 : Convert.ToInt32(r["CategoryID"]);
            foreach (CatItem ci in cmbBCat.Items) if (ci.ID == catId) { cmbBCat.SelectedItem = ci; break; }
            ShowPage(pgAddBook);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // LibrarianDashboard
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "LibrarianDashboard";
            this.Load += new System.EventHandler(this.LibrarianDashboard_Load);
            this.ResumeLayout(false);

        }

        private void LibrarianDashboard_Load(object sender, EventArgs e)
        {

        }

        private void DeleteBook()
        {
            if (dgvBooks.SelectedRows.Count == 0) { MessageBox.Show("Select a book to delete."); return; }
            int id = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["BookID"].Value);
            string title = dgvBooks.SelectedRows[0].Cells["Title"].Value.ToString();
            if (MessageBox.Show("Delete '" + title + "'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            if (DatabaseHelper.ExecuteNonQuery("DELETE FROM Books WHERE BookID=@id", new SqlParameter("@id", id)))
            { MessageBox.Show("Book deleted."); LoadBooks(""); }
        }

        // ═══════════════════════════════════════════════════════════
        // ADD / EDIT BOOK
        // ═══════════════════════════════════════════════════════════
        private void BuildPageAddBook()
        {
            pgAddBook = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false };
            lblAddBookTitle = new Label { Text = "Add New Book", Font = AppTheme.FontH2, ForeColor = AppTheme.NavyDark, Bounds = new Rectangle(30, 20, 400, 36) };
            pgAddBook.Controls.Add(lblAddBookTitle);

            int lx = 30, fx = 200, y = 70, gap = 44;
            pgAddBook.Controls.Add(L("Book Name *", lx, y)); txtBName = AppTheme.MakeInput(350); txtBName.Location = new Point(fx, y); pgAddBook.Controls.Add(txtBName); y += gap;
            pgAddBook.Controls.Add(L("Author *", lx, y)); txtBAuthor = AppTheme.MakeInput(350); txtBAuthor.Location = new Point(fx, y); pgAddBook.Controls.Add(txtBAuthor); y += gap;
            pgAddBook.Controls.Add(L("ISBN", lx, y)); txtBISBN = AppTheme.MakeInput(200); txtBISBN.Location = new Point(fx, y); pgAddBook.Controls.Add(txtBISBN); y += gap;
            pgAddBook.Controls.Add(L("Publication", lx, y)); txtBPub = AppTheme.MakeInput(220); txtBPub.Location = new Point(fx, y); pgAddBook.Controls.Add(txtBPub); y += gap;
            pgAddBook.Controls.Add(L("Quantity *", lx, y)); txtBQty = AppTheme.MakeInput(80); txtBQty.Location = new Point(fx, y); pgAddBook.Controls.Add(txtBQty); y += gap;
            pgAddBook.Controls.Add(L("Price (BDT)", lx, y)); txtBPrice = AppTheme.MakeInput(120); txtBPrice.Location = new Point(fx, y); pgAddBook.Controls.Add(txtBPrice); y += gap;
            pgAddBook.Controls.Add(L("Category", lx, y));
            cmbBCat = new ComboBox { Location = new Point(fx, y), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.FontInput };
            LoadCats();
            pgAddBook.Controls.Add(cmbBCat); y += gap;
            pgAddBook.Controls.Add(L("Description", lx, y));
            txtBDesc = new TextBox { Location = new Point(fx, y), Width = 350, Height = 60, Multiline = true, Font = AppTheme.FontInput, BorderStyle = BorderStyle.FixedSingle };
            pgAddBook.Controls.Add(txtBDesc); y += 72;

            Button btnSave = AppTheme.MakePrimaryBtn("Save Book", 130, 38);
            btnSave.Location = new Point(fx, y); btnSave.Click += (s, e) => SaveBook();
            Button btnCancel = AppTheme.MakeDangerBtn("Cancel", 100, 38);
            btnCancel.Location = new Point(fx + 145, y); btnCancel.Click += (s, e) => { ShowPage(pgBooks); LoadBooks(""); };
            pgAddBook.Controls.AddRange(new Control[] { btnSave, btnCancel });
            pnlContent.Controls.Add(pgAddBook);
        }

        private void LoadCats()
        {
            cmbBCat.Items.Clear();
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT CategoryID, CategoryName FROM Category WHERE IsActive=1 ORDER BY CategoryName");
            if (dt != null) foreach (DataRow r in dt.Rows) cmbBCat.Items.Add(new CatItem { ID = Convert.ToInt32(r["CategoryID"]), Name = r["CategoryName"].ToString() });
            if (cmbBCat.Items.Count > 0) cmbBCat.SelectedIndex = 0;
        }

        private void ClearBookForm()
        {
            _editBookID = 0; lblAddBookTitle.Text = "Add New Book";
            txtBName.Text = txtBAuthor.Text = txtBISBN.Text = txtBPub.Text = txtBQty.Text = txtBPrice.Text = txtBDesc.Text = "";
            LoadCats();
        }

        private void SaveBook()
        {
            if (string.IsNullOrWhiteSpace(txtBName.Text) || string.IsNullOrWhiteSpace(txtBAuthor.Text))
            { MessageBox.Show("Book name and author are required.", "Validation"); return; }
            int qty; if (!int.TryParse(txtBQty.Text, out qty) || qty < 1) { MessageBox.Show("Enter valid quantity.", "Validation"); return; }
            decimal price = 0; decimal.TryParse(txtBPrice.Text, out price);
            int catId = cmbBCat.SelectedItem != null ? ((CatItem)cmbBCat.SelectedItem).ID : 0;

            bool ok;
            if (_editBookID == 0)
                ok = DatabaseHelper.ExecuteNonQuery(
                    "INSERT INTO Books (BookName, Author, ISBN, Publication, CategoryID, TotalQuantity, AvailableQuantity, Price, AddedBy) " +
                    "VALUES (@n,@a,@i,@p,@c,@q,@aq,@pr,@by)",
                    new SqlParameter("@n", txtBName.Text.Trim()),
                    new SqlParameter("@a", txtBAuthor.Text.Trim()),
                    new SqlParameter("@i", txtBISBN.Text.Trim()),
                    new SqlParameter("@p", txtBPub.Text.Trim()),
                    new SqlParameter("@c", catId),
                    new SqlParameter("@q", qty),
                    new SqlParameter("@aq", qty),
                    new SqlParameter("@pr", price),
                    new SqlParameter("@by", LibrarianID));
            else
                ok = DatabaseHelper.ExecuteNonQuery(
                    "UPDATE Books SET BookName=@n, Author=@a, ISBN=@i, Publication=@p, CategoryID=@c, TotalQuantity=@q, Price=@pr, LastModified=GETDATE() WHERE BookID=@id",
                    new SqlParameter("@n", txtBName.Text.Trim()),
                    new SqlParameter("@a", txtBAuthor.Text.Trim()),
                    new SqlParameter("@i", txtBISBN.Text.Trim()),
                    new SqlParameter("@p", txtBPub.Text.Trim()),
                    new SqlParameter("@c", catId),
                    new SqlParameter("@q", qty),
                    new SqlParameter("@pr", price),
                    new SqlParameter("@id", _editBookID));

            if (ok) { MessageBox.Show(_editBookID == 0 ? "Book added!" : "Book updated!"); ShowPage(pgBooks); LoadBooks(""); }
        }

        // ═══════════════════════════════════════════════════════════
        // ISSUE BOOK
        // ═══════════════════════════════════════════════════════════
        private void BuildPageIssue()
        {
            pgIssue = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false };
            pgIssue.Controls.Add(AppTheme.MakeHeader("Issue Book", "Issue a book to a student"));

            int lx = 60, fx = 240, y = 100, gap = 50;
            pgIssue.Controls.Add(L("Enrollment No *", lx, y));
            txtIssEnroll = AppTheme.MakeInput(260); txtIssEnroll.Location = new Point(fx, y); pgIssue.Controls.Add(txtIssEnroll); y += gap;
            pgIssue.Controls.Add(L("Book ISBN *", lx, y));
            txtIssISBN = AppTheme.MakeInput(260); txtIssISBN.Location = new Point(fx, y); pgIssue.Controls.Add(txtIssISBN); y += gap;
            pgIssue.Controls.Add(L("Due Date *", lx, y));
            dtpDue = new DateTimePicker { Location = new Point(fx, y), Width = 200, Font = AppTheme.FontInput, MinDate = DateTime.Today.AddDays(1), Value = DateTime.Today.AddDays(14) };
            pgIssue.Controls.Add(dtpDue); y += gap;

            Button btnIss = AppTheme.MakePrimaryBtn("Issue Book", 140, 40);
            btnIss.Location = new Point(fx, y); btnIss.Click += (s, e) => IssueBook();
            pgIssue.Controls.Add(btnIss);

            lblIssMsg = new Label { Bounds = new Rectangle(fx, y + 50, 500, 24), Font = AppTheme.FontBody, ForeColor = AppTheme.Success, Text = "" };
            pgIssue.Controls.Add(lblIssMsg);
            pnlContent.Controls.Add(pgIssue);
        }

        private void IssueBook()
        {
            lblIssMsg.Text = "";
            string enroll = txtIssEnroll.Text.Trim();
            string isbn = txtIssISBN.Text.Trim();
            if (enroll == "" || isbn == "") { Msg(lblIssMsg, "Enrollment No and ISBN are required.", false); return; }

            DataTable st = DatabaseHelper.ExecuteQuery("SELECT StudentID FROM Student WHERE EnrollmentNo=@e AND IsActive=1", new SqlParameter("@e", enroll));
            if (st == null || st.Rows.Count == 0) { Msg(lblIssMsg, "Student not found or inactive.", false); return; }
            int stuID = Convert.ToInt32(st.Rows[0]["StudentID"]);

            DataTable bt = DatabaseHelper.ExecuteQuery("SELECT BookID, AvailableQuantity FROM Books WHERE ISBN=@i", new SqlParameter("@i", isbn));
            if (bt == null || bt.Rows.Count == 0) { Msg(lblIssMsg, "Book not found.", false); return; }
            int bookID = Convert.ToInt32(bt.Rows[0]["BookID"]);
            int avail = Convert.ToInt32(bt.Rows[0]["AvailableQuantity"]);
            if (avail < 1) { Msg(lblIssMsg, "No available copies.", false); return; }

            bool ok = DatabaseHelper.ExecuteNonQuery(
                "INSERT INTO BookIssue(BookID,StudentID,IssuedDate,DueDate,Status,IssuedBy) VALUES(@b,@s,GETDATE(),@d,'Issued',@lib); " +
                "UPDATE Books SET AvailableQuantity=AvailableQuantity-1 WHERE BookID=@b2",
                new SqlParameter("@b", bookID),
                new SqlParameter("@s", stuID),
                new SqlParameter("@d", dtpDue.Value.Date),
                new SqlParameter("@lib", LibrarianID),
                new SqlParameter("@b2", bookID));

            if (ok) { Msg(lblIssMsg, "Book issued successfully!", true); txtIssEnroll.Text = ""; txtIssISBN.Text = ""; LoadHomeStats(); }
        }

        // ═══════════════════════════════════════════════════════════
        // RETURN BOOK
        // ═══════════════════════════════════════════════════════════
        private void BuildPageReturn()
        {
            pgReturn = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false };
            pgReturn.Controls.Add(AppTheme.MakeHeader("Return Book", "Process book returns"));

            Panel tb = new Panel { Bounds = new Rectangle(20, 78, 700, 44), BackColor = AppTheme.Surface };
            txtRetSearch = AppTheme.MakeInput(220);
            SetPlaceholder(txtRetSearch, "Search enrollment / book name");
            txtRetSearch.Location = new Point(0, 4);

            Button btnSrch = AppTheme.MakePrimaryBtn("Search", 90, 36);
            btnSrch.Location = new Point(230, 4);
            btnSrch.Click += (s, e) => LoadIssued(GetTxt(txtRetSearch, "Search enrollment / book name"));

            Button btnRet = AppTheme.MakeSuccessBtn("Mark Returned", 130, 36);
            btnRet.Location = new Point(330, 4);
            btnRet.Click += (s, e) => ReturnBook();

            tb.Controls.AddRange(new Control[] { txtRetSearch, btnSrch, btnRet });
            pgReturn.Controls.Add(tb);

            lblRetMsg = new Label { Bounds = new Rectangle(20, 125, 500, 24), Font = AppTheme.FontBody, ForeColor = AppTheme.Success, Text = "" };
            pgReturn.Controls.Add(lblRetMsg);

            dgvIssued = new DataGridView { Bounds = new Rectangle(20, 150, 1020, 440), Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom };
            AppTheme.StyleGrid(dgvIssued);
            pgReturn.Controls.Add(dgvIssued);
            pnlContent.Controls.Add(pgReturn);
        }

        private void LoadIssued(string search)
        {
            DataTable dt;
            if (string.IsNullOrWhiteSpace(search))
                dt = DatabaseHelper.ExecuteQuery(
                    "SELECT i.IssueID, s.EnrollmentNo, s.FirstName+' '+ISNULL(s.LastName,'') AS Student, b.BookName AS Book, i.IssuedDate, i.DueDate, i.Status " +
                    "FROM BookIssue i JOIN Student s ON i.StudentID=s.StudentID JOIN Books b ON i.BookID=b.BookID " +
                    "WHERE i.ReturnedDate IS NULL ORDER BY i.DueDate");
            else
                dt = DatabaseHelper.ExecuteQuery(
                    "SELECT i.IssueID, s.EnrollmentNo, s.FirstName+' '+ISNULL(s.LastName,'') AS Student, b.BookName AS Book, i.IssuedDate, i.DueDate, i.Status " +
                    "FROM BookIssue i JOIN Student s ON i.StudentID=s.StudentID JOIN Books b ON i.BookID=b.BookID " +
                    "WHERE i.ReturnedDate IS NULL AND (s.EnrollmentNo LIKE @s OR b.BookName LIKE @s OR s.FirstName LIKE @s) ORDER BY i.DueDate",
                    new SqlParameter("@s", "%" + search + "%"));
            if (dt != null) dgvIssued.DataSource = dt;
        }

        private void ReturnBook()
        {
            if (dgvIssued.SelectedRows.Count == 0) { MessageBox.Show("Select a record to return."); return; }
            int issID = Convert.ToInt32(dgvIssued.SelectedRows[0].Cells["IssueID"].Value);
            string book = dgvIssued.SelectedRows[0].Cells["Book"].Value.ToString();
            if (MessageBox.Show("Mark '" + book + "' as returned?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            bool ok = DatabaseHelper.ExecuteNonQuery(
                "UPDATE BookIssue SET ReturnedDate=GETDATE(), Status='Returned' WHERE IssueID=@id; " +
                "UPDATE Books SET AvailableQuantity=AvailableQuantity+1 WHERE BookID=(SELECT BookID FROM BookIssue WHERE IssueID=@id2)",
                new SqlParameter("@id", issID),
                new SqlParameter("@id2", issID));

            if (ok) { Msg(lblRetMsg, "Book returned successfully!", true); LoadIssued(GetTxt(txtRetSearch, "Search enrollment / book name")); LoadHomeStats(); }
        }

        // ═══════════════════════════════════════════════════════════
        // STUDENTS
        // ═══════════════════════════════════════════════════════════
        private void BuildPageStudents()
        {
            pgStudents = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false };
            pgStudents.Controls.Add(AppTheme.MakeHeader("Students", "Registered library students"));

            Panel tb = new Panel { Bounds = new Rectangle(20, 78, 500, 44), BackColor = AppTheme.Surface };
            txtStudSearch = AppTheme.MakeInput(240);
            SetPlaceholder(txtStudSearch, "Search by enrollment or name");
            txtStudSearch.Location = new Point(0, 4);
            Button btnSrch = AppTheme.MakePrimaryBtn("Search", 90, 36);
            btnSrch.Location = new Point(250, 4);
            btnSrch.Click += (s, e) => LoadStudents(GetTxt(txtStudSearch, "Search by enrollment or name"));
            tb.Controls.AddRange(new Control[] { txtStudSearch, btnSrch });
            pgStudents.Controls.Add(tb);

            dgvStudents = new DataGridView { Bounds = new Rectangle(20, 126, 1020, 480), Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom };
            AppTheme.StyleGrid(dgvStudents);
            pgStudents.Controls.Add(dgvStudents);
            pnlContent.Controls.Add(pgStudents);
        }

        private void LoadStudents(string search)
        {
            DataTable dt;
            if (string.IsNullOrWhiteSpace(search))
                dt = DatabaseHelper.ExecuteQuery(
                    "SELECT EnrollmentNo, FirstName+' '+ISNULL(LastName,'') AS Name, Email, PhoneNo, Department, Semester, CASE IsActive WHEN 1 THEN 'Active' ELSE 'Inactive' END AS Status FROM Student ORDER BY EnrollmentNo");
            else
                dt = DatabaseHelper.ExecuteQuery(
                    "SELECT EnrollmentNo, FirstName+' '+ISNULL(LastName,'') AS Name, Email, PhoneNo, Department, Semester, CASE IsActive WHEN 1 THEN 'Active' ELSE 'Inactive' END AS Status FROM Student WHERE EnrollmentNo LIKE @s OR FirstName LIKE @s OR LastName LIKE @s ORDER BY EnrollmentNo",
                    new SqlParameter("@s", "%" + search + "%"));
            if (dt != null) dgvStudents.DataSource = dt;
        }

        // ═══════════════════════════════════════════════════════════
        // BORROW REQUESTS
        // ═══════════════════════════════════════════════════════════
        private void BuildPageRequests()
        {
            pgRequests = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false };
            pgRequests.Controls.Add(AppTheme.MakeHeader("Borrow Requests", "Pending student requests"));

            Panel tb = new Panel { Bounds = new Rectangle(20, 78, 500, 44), BackColor = AppTheme.Surface };
            Button btnApp = AppTheme.MakeSuccessBtn("Approve", 120, 36); btnApp.Location = new Point(0, 4); btnApp.Click += (s, e) => HandleRequest("Approved");
            Button btnRej = AppTheme.MakeDangerBtn("Reject", 110, 36); btnRej.Location = new Point(130, 4); btnRej.Click += (s, e) => HandleRequest("Rejected");
            Button btnRef = AppTheme.MakePrimaryBtn("Refresh", 110, 36); btnRef.Location = new Point(250, 4); btnRef.Click += (s, e) => LoadRequests();
            tb.Controls.AddRange(new Control[] { btnApp, btnRej, btnRef });
            pgRequests.Controls.Add(tb);

            dgvRequests = new DataGridView { Bounds = new Rectangle(20, 126, 1020, 470), Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom };
            AppTheme.StyleGrid(dgvRequests);
            pgRequests.Controls.Add(dgvRequests);
            pnlContent.Controls.Add(pgRequests);
        }

        private void LoadRequests()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT r.RequestID, s.EnrollmentNo, s.FirstName+' '+ISNULL(s.LastName,'') AS Student, b.BookName AS Book, r.RequestDate, r.Status " +
                "FROM BorrowRequest r JOIN Student s ON r.StudentID=s.StudentID JOIN Books b ON r.BookID=b.BookID " +
                "ORDER BY CASE r.Status WHEN 'Pending' THEN 0 ELSE 1 END, r.RequestDate DESC");
            if (dt != null) dgvRequests.DataSource = dt;
        }

        private void HandleRequest(string newStatus)
        {
            if (dgvRequests.SelectedRows.Count == 0) { MessageBox.Show("Select a request."); return; }
            int reqID = Convert.ToInt32(dgvRequests.SelectedRows[0].Cells["RequestID"].Value);
            string cur = dgvRequests.SelectedRows[0].Cells["Status"].Value.ToString();
            if (cur != "Pending") { MessageBox.Show("Only pending requests can be updated."); return; }

            bool ok = DatabaseHelper.ExecuteNonQuery(
                "UPDATE BorrowRequest SET Status=@st, ProcessedBy=@lib, ProcessedDate=GETDATE() WHERE RequestID=@id",
                new SqlParameter("@st", newStatus),
                new SqlParameter("@lib", LibrarianID),
                new SqlParameter("@id", reqID));

            if (ok && newStatus == "Approved")
            {
                DataTable bt = DatabaseHelper.ExecuteQuery(
                    "SELECT r.BookID, r.StudentID, b.AvailableQuantity FROM BorrowRequest r JOIN Books b ON r.BookID=b.BookID WHERE r.RequestID=@rid",
                    new SqlParameter("@rid", reqID));
                if (bt != null && bt.Rows.Count > 0)
                {
                    int bookID = Convert.ToInt32(bt.Rows[0]["BookID"]);
                    int stuID = Convert.ToInt32(bt.Rows[0]["StudentID"]);
                    int avail = Convert.ToInt32(bt.Rows[0]["AvailableQuantity"]);
                    if (avail > 0)
                        DatabaseHelper.ExecuteNonQuery(
                            "INSERT INTO BookIssue(BookID,StudentID,IssuedDate,DueDate,Status,IssuedBy) VALUES(@b,@s,GETDATE(),@d,'Issued',@lib); " +
                            "UPDATE Books SET AvailableQuantity=AvailableQuantity-1 WHERE BookID=@b2",
                            new SqlParameter("@b", bookID),
                            new SqlParameter("@s", stuID),
                            new SqlParameter("@d", DateTime.Today.AddDays(14)),
                            new SqlParameter("@lib", LibrarianID),
                            new SqlParameter("@b2", bookID));
                }
                MessageBox.Show("Request approved and book issued!");
            }
            else if (ok) MessageBox.Show("Request " + newStatus.ToLower() + ".");

            LoadRequests(); LoadHomeStats();
        }

        // ═══════════════════════════════════════════════════════════
        // REPORTS
        // ═══════════════════════════════════════════════════════════
        private void BuildPageReports()
        {
            pgReports = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false };
            pgReports.Controls.Add(AppTheme.MakeHeader("Reports", "Issue history"));

            Panel fl = new Panel { Bounds = new Rectangle(20, 78, 700, 44), BackColor = AppTheme.Surface };
            fl.Controls.Add(new Label { Text = "From:", Font = AppTheme.FontLabel, Location = new Point(0, 8), AutoSize = true });
            dtpRptFrom = new DateTimePicker { Location = new Point(52, 4), Width = 160, Font = AppTheme.FontInput, Value = DateTime.Today.AddDays(-30) };
            fl.Controls.Add(dtpRptFrom);
            fl.Controls.Add(new Label { Text = "To:", Font = AppTheme.FontLabel, Location = new Point(225, 8), AutoSize = true });
            dtpRptTo = new DateTimePicker { Location = new Point(250, 4), Width = 160, Font = AppTheme.FontInput, Value = DateTime.Today };
            fl.Controls.Add(dtpRptTo);
            Button btnLoad = AppTheme.MakePrimaryBtn("Load Report", 130, 36);
            btnLoad.Location = new Point(425, 4); btnLoad.Click += (s, e) => LoadReport();
            fl.Controls.Add(btnLoad);
            pgReports.Controls.Add(fl);

            dgvReport = new DataGridView { Bounds = new Rectangle(20, 126, 1020, 470), Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom };
            AppTheme.StyleGrid(dgvReport);
            pgReports.Controls.Add(dgvReport);
            pnlContent.Controls.Add(pgReports);
        }

        private void LoadReport()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT i.IssueID, s.EnrollmentNo, s.FirstName+' '+ISNULL(s.LastName,'') AS Student, b.BookName AS Book, i.IssuedDate, i.DueDate, i.ReturnedDate, i.Status, ISNULL(i.FineAmount,0) AS Fine " +
                "FROM BookIssue i JOIN Student s ON i.StudentID=s.StudentID JOIN Books b ON i.BookID=b.BookID " +
                "WHERE i.IssuedDate BETWEEN @f AND @t ORDER BY i.IssuedDate DESC",
                new SqlParameter("@f", dtpRptFrom.Value.Date),
                new SqlParameter("@t", dtpRptTo.Value.Date.AddDays(1).AddSeconds(-1)));
            if (dt != null) dgvReport.DataSource = dt;
        }

        // ═══════════════════════════════════════════════════════════
        // PROFILE
        // ═══════════════════════════════════════════════════════════
        private void BuildPageProfile()
        {
            pgProfile = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Visible = false };
            pgProfile.Controls.Add(AppTheme.MakeHeader("My Profile", "View and update your account"));

            int lx = 60, fx = 220, y = 100, gap = 46;
            pgProfile.Controls.Add(new Label { Text = "Librarian ID:", Font = AppTheme.FontLabel, ForeColor = AppTheme.TextMuted, Location = new Point(lx, y), AutoSize = true });
            lblProfId = new Label { Font = AppTheme.FontH3, ForeColor = AppTheme.TextPrimary, Location = new Point(fx, y), AutoSize = true }; pgProfile.Controls.Add(lblProfId); y += gap;
            pgProfile.Controls.Add(new Label { Text = "Full Name:", Font = AppTheme.FontLabel, ForeColor = AppTheme.TextMuted, Location = new Point(lx, y), AutoSize = true });
            lblProfName = new Label { Font = AppTheme.FontH3, ForeColor = AppTheme.TextPrimary, Location = new Point(fx, y), AutoSize = true }; pgProfile.Controls.Add(lblProfName); y += gap;
            pgProfile.Controls.Add(new Label { Text = "Email:", Font = AppTheme.FontLabel, ForeColor = AppTheme.TextMuted, Location = new Point(lx, y), AutoSize = true });
            lblProfEmail = new Label { Font = AppTheme.FontH3, ForeColor = AppTheme.TextPrimary, Location = new Point(fx, y), AutoSize = true }; pgProfile.Controls.Add(lblProfEmail); y += gap;
            pgProfile.Controls.Add(new Label { Text = "Phone:", Font = AppTheme.FontLabel, ForeColor = AppTheme.TextMuted, Location = new Point(lx, y), AutoSize = true });
            lblProfPhone = new Label { Font = AppTheme.FontH3, ForeColor = AppTheme.TextPrimary, Location = new Point(fx, y), AutoSize = true }; pgProfile.Controls.Add(lblProfPhone); y += gap + 10;

            pgProfile.Controls.Add(new Panel { Bounds = new Rectangle(lx, y, 500, 2), BackColor = AppTheme.BorderColor }); y += 20;
            Label ch = AppTheme.MakeSectionTitle("Change Password"); ch.Location = new Point(lx, y); pgProfile.Controls.Add(ch); y += 34;

            pgProfile.Controls.Add(L("Current Password", lx, y)); txtOldPass = AppTheme.MakeInput(260, true); txtOldPass.Location = new Point(fx, y); pgProfile.Controls.Add(txtOldPass); y += gap;
            pgProfile.Controls.Add(L("New Password", lx, y)); txtNewPass = AppTheme.MakeInput(260, true); txtNewPass.Location = new Point(fx, y); pgProfile.Controls.Add(txtNewPass); y += gap;
            pgProfile.Controls.Add(L("Confirm Password", lx, y)); txtConfPass = AppTheme.MakeInput(260, true); txtConfPass.Location = new Point(fx, y); pgProfile.Controls.Add(txtConfPass); y += gap;

            Button btnCP = AppTheme.MakePrimaryBtn("Change Password", 170, 38);
            btnCP.Location = new Point(fx, y); btnCP.Click += (s, e) => ChangePassword();
            pgProfile.Controls.Add(btnCP);
            pnlContent.Controls.Add(pgProfile);
        }

        private void LoadProfile()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT LibrarianID, Username, FirstName+' '+ISNULL(LastName,'') AS FullName, Email, PhoneNo FROM Librarian WHERE LibrarianID=@id", new SqlParameter("@id", LibrarianID));
            if (dt == null || dt.Rows.Count == 0) return;
            DataRow r = dt.Rows[0];
            lblProfId.Text = r["Username"].ToString();
            lblProfName.Text = r["FullName"].ToString();
            lblProfEmail.Text = r["Email"] == DBNull.Value ? "" : r["Email"].ToString();
            lblProfPhone.Text = r["PhoneNo"] == DBNull.Value ? "" : r["PhoneNo"].ToString();
            txtOldPass.Text = txtNewPass.Text = txtConfPass.Text = "";
        }

        private void ChangePassword()
        {
            if (string.IsNullOrWhiteSpace(txtOldPass.Text) || string.IsNullOrWhiteSpace(txtNewPass.Text)) { MessageBox.Show("Fill all password fields."); return; }
            if (txtNewPass.Text != txtConfPass.Text) { MessageBox.Show("Passwords do not match."); return; }
            object chk = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Librarian WHERE LibrarianID=@id AND Password=@old", new SqlParameter("@id", LibrarianID), new SqlParameter("@old", txtOldPass.Text));
            if (chk == null || Convert.ToInt32(chk) == 0) { MessageBox.Show("Current password incorrect."); return; }
            if (DatabaseHelper.ExecuteNonQuery("UPDATE Librarian SET Password=@np WHERE LibrarianID=@id", new SqlParameter("@np", txtNewPass.Text), new SqlParameter("@id", LibrarianID)))
            { MessageBox.Show("Password changed!"); txtOldPass.Text = txtNewPass.Text = txtConfPass.Text = ""; }
        }

        // ═══════════════════════════════════════════════════════════
        // HELPERS
        // ═══════════════════════════════════════════════════════════
        private Label L(string text, int x, int y) => new Label { Text = text, Font = AppTheme.FontLabel, ForeColor = AppTheme.TextPrimary, Location = new Point(x, y + 4), AutoSize = true };
        private void SetPlaceholder(TextBox tb, string ph) { tb.Text = ph; tb.ForeColor = AppTheme.TextMuted; tb.GotFocus += (s, e) => { if (tb.ForeColor == AppTheme.TextMuted) { tb.Text = ""; tb.ForeColor = AppTheme.TextPrimary; } }; tb.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(tb.Text)) { tb.Text = ph; tb.ForeColor = AppTheme.TextMuted; } }; }
        private string GetTxt(TextBox tb, string ph) => tb.ForeColor == AppTheme.TextMuted ? "" : tb.Text.Trim();
        private void Msg(Label lbl, string text, bool success) { lbl.ForeColor = success ? AppTheme.Success : AppTheme.Danger; lbl.Text = text; }
    }
}