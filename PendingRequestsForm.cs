using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Library_Management_System
{
    /// <summary>
    /// Librarian Feature 9: View & Process Pending Borrow Requests
    /// </summary>
    public class PendingRequestsForm : Form
    {
        private int librarianID;
        private DataGridView dgvRequests;
        private Button btnApprove, btnReject, btnRefresh;
        private Label lblCount;
        private ComboBox cmbFilter;

        public PendingRequestsForm(int lid)
        {
            librarianID = lid;
            InitializeComponent();
            LoadRequests();
        }

        private void InitializeComponent()
        {
            this.Text            = "Pending Borrow Requests";
            this.Size            = new Size(1000, 620);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            AppTheme.StyleForm(this);

            Panel header = AppTheme.MakeHeader("📩 Borrow Requests", "Approve or reject student borrow requests");
            this.Controls.Add(header);

            // Filter bar
            Panel bar = new Panel { Location = new Point(10, 80), Size = new Size(980, 50), BackColor = AppTheme.CardBg };
            bar.Paint += (s, e) => e.Graphics.DrawRectangle(new System.Drawing.Pen(AppTheme.BorderColor), 0, 0, bar.Width - 1, bar.Height - 1);

            Label lblFilter = new Label { Text = "Show:", Font = AppTheme.FontLabel, AutoSize = true, Location = new Point(10, 15) };
            cmbFilter = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.FontInput,
                Width = 160, Location = new Point(60, 12)
            };
            cmbFilter.Items.AddRange(new object[] { "Pending", "Approved", "Rejected", "All" });
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => LoadRequests();

            lblCount = new Label { Text = "0 requests", Font = AppTheme.FontBody, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(240, 15) };

            btnRefresh = AppTheme.MakePrimaryBtn("↺ Refresh", 100, 32);
            btnRefresh.Location  = new Point(780, 9);
            btnRefresh.BackColor = AppTheme.NavyMid;
            btnRefresh.Click    += (s, e) => LoadRequests();

            bar.Controls.AddRange(new Control[] { lblFilter, cmbFilter, lblCount, btnRefresh });
            this.Controls.Add(bar);

            // Grid
            dgvRequests = new DataGridView { Location = new Point(10, 140), Size = new Size(980, 360) };
            AppTheme.StyleGrid(dgvRequests);
            this.Controls.Add(dgvRequests);

            // Action buttons
            btnApprove = AppTheme.MakeSuccessBtn("✅  Approve Request", 200, 44);
            btnApprove.Location = new Point(570, 516);
            btnApprove.Click   += BtnApprove_Click;

            btnReject = AppTheme.MakeDangerBtn("✖  Reject Request", 200, 44);
            btnReject.Location = new Point(780, 516);
            btnReject.Click   += BtnReject_Click;

            Label note = new Label
            {
                Text = "ℹ️  Approving a request will automatically issue the book to the student.",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(10, 530)
            };

            this.Controls.Add(btnApprove);
            this.Controls.Add(btnReject);
            this.Controls.Add(note);
        }

        private void LoadRequests()
        {
            string filter = cmbFilter.SelectedItem?.ToString() ?? "Pending";
            string where  = filter == "All" ? "" : $" AND br.Status = '{filter}'";

            DataTable dt = DatabaseHelper.ExecuteQuery(
                $@"SELECT br.RequestID,
                          s.EnrollmentNo AS [Enrollment],
                          s.FirstName + ' ' + ISNULL(s.LastName,'') AS [Student Name],
                          b.BookName AS [Book Title],
                          b.Author,
                          b.AvailableQuantity AS [Available Copies],
                          br.RequestDate AS [Requested On],
                          br.Status
                   FROM BorrowRequest br
                   INNER JOIN Student s ON br.StudentID=s.StudentID
                   INNER JOIN Books b   ON br.BookID=b.BookID
                   WHERE 1=1 {where}
                   ORDER BY br.RequestDate DESC");

            dgvRequests.DataSource = dt;
            if (dgvRequests.Columns.Contains("RequestID")) dgvRequests.Columns["RequestID"].Visible = false;

            // Color rows by status
            if (dt != null)
                foreach (DataGridViewRow row in dgvRequests.Rows)
                {
                    if (row.IsNewRow) continue;
                    string s = row.Cells["Status"].Value?.ToString();
                    if (s == "Approved") row.DefaultCellStyle.ForeColor = AppTheme.Success;
                    else if (s == "Rejected") row.DefaultCellStyle.ForeColor = AppTheme.Danger;
                    else row.DefaultCellStyle.ForeColor = AppTheme.Warning;
                }

            lblCount.Text = $"{dt?.Rows.Count ?? 0} request(s)";
        }

        private void BtnApprove_Click(object sender, EventArgs e)
        {
            if (dgvRequests.SelectedRows.Count == 0)
            { MessageBox.Show("Select a request.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string status = dgvRequests.SelectedRows[0].Cells["Status"].Value?.ToString();
            if (status != "Pending")
            { MessageBox.Show("Only pending requests can be approved.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            int avail = Convert.ToInt32(dgvRequests.SelectedRows[0].Cells["Available Copies"].Value);
            if (avail <= 0)
            { MessageBox.Show("No copies available for this book.", "Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            int reqID = Convert.ToInt32(dgvRequests.SelectedRows[0].Cells["RequestID"].Value);

            // Get BookID and StudentID
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT BookID,StudentID FROM BorrowRequest WHERE RequestID=@id",
                new SqlParameter[] { new SqlParameter("@id", reqID) });
            if (dt == null || dt.Rows.Count == 0) return;

            int bookID    = Convert.ToInt32(dt.Rows[0]["BookID"]);
            int studentID = Convert.ToInt32(dt.Rows[0]["StudentID"]);

            if (MessageBox.Show("Approve this request and issue the book?\n\nDue date will be set to 14 days from today.",
                "Confirm Approval", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            // Issue the book
            bool issued = DatabaseHelper.ExecuteNonQuery(
                "INSERT INTO BookIssue (BookID,StudentID,IssuedDate,DueDate,Status,IssuedBy) VALUES (@b,@s,GETDATE(),DATEADD(DAY,14,GETDATE()),'Issued',@lib)",
                new SqlParameter[] {
                    new SqlParameter("@b",   bookID),
                    new SqlParameter("@s",   studentID),
                    new SqlParameter("@lib", librarianID)
                });

            if (issued)
            {
                DatabaseHelper.ExecuteNonQuery("UPDATE Books SET AvailableQuantity=AvailableQuantity-1 WHERE BookID=@b",
                    new SqlParameter[] { new SqlParameter("@b", bookID) });
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE BorrowRequest SET Status='Approved',ProcessedBy=@lib,ProcessedDate=GETDATE() WHERE RequestID=@id",
                    new SqlParameter[] { new SqlParameter("@lib", librarianID), new SqlParameter("@id", reqID) });

                MessageBox.Show("✅ Request approved and book issued successfully!\nDue date: " + DateTime.Now.AddDays(14).ToString("dd-MMM-yyyy"),
                    "Approved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadRequests();
            }
        }

        private void BtnReject_Click(object sender, EventArgs e)
        {
            if (dgvRequests.SelectedRows.Count == 0)
            { MessageBox.Show("Select a request.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string status = dgvRequests.SelectedRows[0].Cells["Status"].Value?.ToString();
            if (status != "Pending")
            { MessageBox.Show("Only pending requests can be rejected.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            int reqID = Convert.ToInt32(dgvRequests.SelectedRows[0].Cells["RequestID"].Value);
            if (MessageBox.Show("Reject this request?", "Confirm Rejection", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            bool ok = DatabaseHelper.ExecuteNonQuery(
                "UPDATE BorrowRequest SET Status='Rejected',ProcessedBy=@lib,ProcessedDate=GETDATE() WHERE RequestID=@id",
                new SqlParameter[] { new SqlParameter("@lib", librarianID), new SqlParameter("@id", reqID) });

            if (ok) { MessageBox.Show("Request rejected.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadRequests(); }
        }
    }
}
