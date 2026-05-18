using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Library_Management_System
{
    /// <summary>
    /// Student Feature 6: Request to Borrow a Book
    /// </summary>
    public class BorrowRequestForm : Form
    {
        private int studentID;
        private DataGridView dgvBooks;
        private TextBox  txtSearch;
        private ComboBox cmbCategory;
        private Button   btnSearch, btnRequest, btnRefresh;
        private Label    lblStatus;

        public BorrowRequestForm(int sid)
        {
            studentID = sid;
            InitializeComponent();
            LoadCategories();
            LoadAvailableBooks();
        }

        private void InitializeComponent()
        {
            this.Text            = "Request to Borrow a Book";
            this.Size            = new Size(900, 620);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            AppTheme.StyleForm(this);

            // Header
            Panel header = AppTheme.MakeHeader("📖 Borrow Request", "Select a book and send a request to the librarian");
            this.Controls.Add(header);

            // Search bar
            Panel searchPanel = new Panel
            {
                Location  = new Point(10, 80),
                Size      = new Size(880, 55),
                BackColor = AppTheme.CardBg
            };
            searchPanel.Paint += (s, e) =>
                e.Graphics.DrawRectangle(new System.Drawing.Pen(AppTheme.BorderColor), 0, 0, searchPanel.Width - 1, searchPanel.Height - 1);

            txtSearch = AppTheme.MakeInput(300);
            txtSearch.Location    = new Point(10, 12);
            txtSearch.Text = ""; // search book title or author

            cmbCategory = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font  = AppTheme.FontInput,
                Width = 180,
                Location = new Point(320, 12)
            };

            btnSearch = AppTheme.MakePrimaryBtn("🔍 Search", 110, 32);
            btnSearch.Location = new Point(514, 12);
            btnSearch.Click   += (s, e) => LoadAvailableBooks();

            btnRefresh = AppTheme.MakePrimaryBtn("↺ Refresh", 110, 32);
            btnRefresh.Location  = new Point(634, 12);
            btnRefresh.BackColor = AppTheme.NavyMid;
            btnRefresh.Click    += (s, e) => { txtSearch.Clear(); cmbCategory.SelectedIndex = 0; LoadAvailableBooks(); };

            searchPanel.Controls.Add(txtSearch);
            searchPanel.Controls.Add(cmbCategory);
            searchPanel.Controls.Add(btnSearch);
            searchPanel.Controls.Add(btnRefresh);
            this.Controls.Add(searchPanel);

            // Grid
            dgvBooks = new DataGridView
            {
                Location = new Point(10, 145),
                Size     = new Size(880, 370)
            };
            AppTheme.StyleGrid(dgvBooks);
            this.Controls.Add(dgvBooks);

            // Bottom bar
            lblStatus = new Label
            {
                Text      = "Select a book from the list and click 'Send Request'",
                Font      = AppTheme.FontBody,
                ForeColor = AppTheme.TextMuted,
                Location  = new Point(15, 530),
                AutoSize  = true
            };

            btnRequest = AppTheme.MakePrimaryBtn("📩  Send Borrow Request", 230, 42);
            btnRequest.Location  = new Point(650, 522);
            btnRequest.BackColor = AppTheme.Success;
            btnRequest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(21, 128, 61);
            btnRequest.Click    += BtnRequest_Click;

            this.Controls.Add(lblStatus);
            this.Controls.Add(btnRequest);

            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) LoadAvailableBooks(); };
        }

        private void LoadCategories()
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("All Categories");
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT CategoryName FROM Category WHERE IsActive=1 ORDER BY CategoryName");
            if (dt != null)
                foreach (DataRow row in dt.Rows)
                    cmbCategory.Items.Add(row["CategoryName"].ToString());
            cmbCategory.SelectedIndex = 0;
        }

        private void LoadAvailableBooks()
        {
            string search   = txtSearch.Text.Trim();
            string category = cmbCategory.SelectedIndex > 0 ? cmbCategory.SelectedItem.ToString() : "";

            string query = @"
                SELECT b.BookID, b.BookName AS [Book Title], b.Author,
                       ISNULL(c.CategoryName,'General') AS Category,
                       b.AvailableQuantity AS [Available],
                       b.TotalQuantity AS [Total], b.Price AS [Price (৳)]
                FROM Books b
                LEFT JOIN Category c ON b.CategoryID = c.CategoryID
                WHERE b.AvailableQuantity > 0";

            if (!string.IsNullOrEmpty(search))
                query += " AND (b.BookName LIKE @s OR b.Author LIKE @s)";
            if (!string.IsNullOrEmpty(category))
                query += " AND c.CategoryName = @cat";
            query += " ORDER BY b.BookName";

            SqlParameter[] prms = new SqlParameter[] {
                new SqlParameter("@s",   "%" + search + "%"),
                new SqlParameter("@cat", category)
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, prms);
            dgvBooks.DataSource = dt;

            if (dgvBooks.Columns.Contains("BookID"))
                dgvBooks.Columns["BookID"].Visible = false;

            lblStatus.Text = $"{(dt?.Rows.Count ?? 0)} book(s) available";
        }

        private void BtnRequest_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book from the list.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int    bookID   = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["BookID"].Value);
            string bookName = dgvBooks.SelectedRows[0].Cells["Book Title"].Value.ToString();

            // Check if already pending
            object existing = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM BorrowRequest WHERE BookID=@b AND StudentID=@s AND Status='Pending'",
                new SqlParameter[] { new SqlParameter("@b", bookID), new SqlParameter("@s", studentID) });

            if (Convert.ToInt32(existing) > 0)
            {
                MessageBox.Show("You already have a pending request for this book.", "Duplicate Request",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if already borrowed
            object borrowed = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM BookIssue WHERE BookID=@b AND StudentID=@s AND Status='Issued'",
                new SqlParameter[] { new SqlParameter("@b", bookID), new SqlParameter("@s", studentID) });

            if (Convert.ToInt32(borrowed) > 0)
            {
                MessageBox.Show("You have already borrowed this book and haven't returned it yet.", "Already Borrowed",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Send borrow request for:\n\n📖 {bookName}\n\nThe librarian will approve or reject your request.",
                "Confirm Request", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            bool ok = DatabaseHelper.ExecuteNonQuery(
     "INSERT INTO BorrowRequest (BookID, StudentID, RequestDate, Status) VALUES (@b, @s, GETDATE(), 'Pending')",
     new SqlParameter[] { new SqlParameter("@b", bookID), new SqlParameter("@s", studentID) });

            if (ok)
            {
                MessageBox.Show("✅ Borrow request sent successfully!\nThe librarian will process it soon.",
                    "Request Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAvailableBooks();
            }
        }
    }
}
