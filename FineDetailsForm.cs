using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Library_Management_System
{
    /// <summary>
    /// Student Feature 9: Fine Details
    /// Shows current fines and fine history
    /// </summary>
    public class FineDetailsForm : Form
    {
        private int studentID;
        private DataGridView dgvFines;
        private Label lblTotalFine, lblOverdue, lblInfo;

        public FineDetailsForm(int sid)
        {
            studentID = sid;
            InitializeComponent();
            LoadFines();
        }

        private void InitializeComponent()
        {
            this.Text            = "Fine Details";
            this.Size            = new Size(900, 580);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            AppTheme.StyleForm(this);

            // Header
            Panel header = AppTheme.MakeHeader("💰 Fine Details", "View your overdue fines — ৳5 per day after due date");
            this.Controls.Add(header);

            // Stat cards row
            Panel statsRow = new Panel
            {
                Location  = new Point(10, 82),
                Size      = new Size(880, 105),
                BackColor = AppTheme.Surface
            };

            Panel card1 = AppTheme.MakeStatCard("৳0", "Total Fine Due", AppTheme.Danger);
            card1.Location = new Point(0, 5);
            lblTotalFine   = (Label)card1.Controls[0]; // value label

            Panel card2 = AppTheme.MakeStatCard("0", "Overdue Books", AppTheme.Warning);
            card2.Location = new Point(185, 5);
            lblOverdue     = (Label)card2.Controls[0];

            Panel card3 = AppTheme.MakeStatCard("৳5", "Fine Per Day", AppTheme.Teal);
            card3.Location = new Point(370, 5);

            statsRow.Controls.Add(card1);
            statsRow.Controls.Add(card2);
            statsRow.Controls.Add(card3);
            this.Controls.Add(statsRow);

            // Info notice
            lblInfo = new Label
            {
                Text = "ℹ️  Fine is calculated automatically. Pay fine at the library counter to clear your account.",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = AppTheme.WarningDark,
                Location  = new Point(12, 192),
                AutoSize  = true
            };
            this.Controls.Add(lblInfo);

            // Grid
            dgvFines = new DataGridView
            {
                Location = new Point(10, 215),
                Size     = new Size(880, 300)
            };
            AppTheme.StyleGrid(dgvFines);
            this.Controls.Add(dgvFines);

            // Refresh button
            Button btnRefresh = AppTheme.MakePrimaryBtn("↺ Refresh", 130, 36);
            btnRefresh.Location = new Point(760, 527);
            btnRefresh.Click   += (s, e) => LoadFines();
            this.Controls.Add(btnRefresh);
        }

        private void LoadFines()
        {
            string query = @"
                SELECT
                    bi.IssueID,
                    b.BookName          AS [Book Title],
                    b.Author,
                    bi.IssuedDate       AS [Issued On],
                    bi.DueDate          AS [Due Date],
                    bi.ReturnedDate     AS [Returned On],
                    bi.Status,
                    CASE
                        WHEN bi.ReturnedDate IS NULL AND bi.DueDate < CAST(GETDATE() AS DATE)
                             THEN DATEDIFF(DAY, bi.DueDate, GETDATE())
                        WHEN bi.ReturnedDate IS NOT NULL AND bi.DueDate < CAST(bi.ReturnedDate AS DATE)
                             THEN DATEDIFF(DAY, bi.DueDate, bi.ReturnedDate)
                        ELSE 0
                    END                 AS [Days Late],
                    ISNULL(bi.FineAmount, 0) AS [Fine (৳)]
                FROM BookIssue bi
                INNER JOIN Books b ON bi.BookID = b.BookID
                WHERE bi.StudentID = @sid
                  AND (bi.FineAmount > 0 OR (bi.ReturnedDate IS NULL AND bi.DueDate < CAST(GETDATE() AS DATE)))
                ORDER BY bi.DueDate DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query,
                new SqlParameter[] { new SqlParameter("@sid", studentID) });

            // Auto-update fines for currently overdue books
            UpdateOverdueFines();

            // Re-query after update
            dt = DatabaseHelper.ExecuteQuery(query,
                new SqlParameter[] { new SqlParameter("@sid", studentID) });

            dgvFines.DataSource = dt;
            if (dgvFines.Columns.Contains("IssueID"))
                dgvFines.Columns["IssueID"].Visible = false;

            // Colour overdue rows red
            if (dt != null)
            {
                dgvFines.CellFormatting += (s, e) =>
                {
                    if (e.RowIndex < 0 || e.RowIndex >= dgvFines.Rows.Count) return;
                    DataGridViewRow row = dgvFines.Rows[e.RowIndex];
                    string status = row.Cells["Status"].Value?.ToString() ?? "";
                    if (status == "Overdue" || status == "Issued")
                    {
                        if (row.Cells["Days Late"].Value != null && Convert.ToInt32(row.Cells["Days Late"].Value) > 0)
                            row.DefaultCellStyle.ForeColor = AppTheme.Danger;
                    }
                };
            }

            // Totals
            decimal totalFine = 0;
            int     overdue   = 0;
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    totalFine += Convert.ToDecimal(row["Fine (৳)"]);
                    if (Convert.ToInt32(row["Days Late"]) > 0 && row["Returned On"] == DBNull.Value)
                        overdue++;
                }
            }

            lblTotalFine.Text = "৳" + totalFine.ToString("N0");
            lblOverdue.Text   = overdue.ToString();
        }

        private void UpdateOverdueFines()
        {
            // Mark overdue
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE BookIssue SET Status='Overdue', FineAmount=DATEDIFF(DAY,DueDate,GETDATE())*5 WHERE StudentID=@sid AND ReturnedDate IS NULL AND DueDate < CAST(GETDATE() AS DATE)",
                new SqlParameter[] { new SqlParameter("@sid", studentID) });
        }
    }
}
