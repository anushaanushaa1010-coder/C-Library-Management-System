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
            this.SuspendLayout();
            // 
            // PendingRequestsForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "PendingRequestsForm";
            this.Load += new System.EventHandler(this.PendingRequestsForm_Load);
            this.ResumeLayout(false);

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

        private void PendingRequestsForm_Load(object sender, EventArgs e)
        {

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
