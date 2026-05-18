using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Library_Management_System
{
    public class ReportsForm : Form
    {
        // ── private fields ────────────────────────────────────────────────────
        private int adminID;

        // Stat cards
        private Panel cardTotalIssues;
        private Panel cardTotalReturned;
        private Panel cardTotalFine;

        // Tab: Issues by Date
        private TabPage tabByDate;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private Button btnLoadByDate;
        private DataGridView dgvByDate;

        // Tab: Top Books
        private TabPage tabTopBooks;
        private Button btnLoadTopBooks;
        private DataGridView dgvTopBooks;

        // Tab: Top Students
        private TabPage tabTopStudents;
        private Button btnLoadTopStudents;
        private DataGridView dgvTopStudents;

        // ────────────────────────────────────────────────────────────────────
        public ReportsForm(int aid)
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
            this.Text = "Reports & Analytics";
            this.Size = new Size(950, 640);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // ── Header ───────────────────────────────────────────────────────
            var pnlHeader = AppTheme.MakeHeader("📊 Reports & Analytics", "Library usage statistics");
            this.Controls.Add(pnlHeader);

            // ── Stat Cards row ───────────────────────────────────────────────
            var pnlCards = new Panel
            {
                Location = new Point(16, 80),
                Size = new Size(910, 110),
                BackColor = Color.Transparent
            };

            cardTotalIssues = AppTheme.MakeStatCard("—", "Total Issues (All Time)", AppTheme.Teal);
            cardTotalIssues.Location = new Point(0, 0);

            cardTotalReturned = AppTheme.MakeStatCard("—", "Total Returned", AppTheme.Success);
            cardTotalReturned.Location = new Point(190, 0);

            cardTotalFine = AppTheme.MakeStatCard("—", "Fine Collected (৳)", AppTheme.Danger);
            cardTotalFine.Location = new Point(380, 0);

            pnlCards.Controls.Add(cardTotalIssues);
            pnlCards.Controls.Add(cardTotalReturned);
            pnlCards.Controls.Add(cardTotalFine);
            this.Controls.Add(pnlCards);

            // ── TabControl ───────────────────────────────────────────────────
            var tabMain = new TabControl
            {
                Location = new Point(16, 200),
                Size = new Size(910, 398),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Padding = new Point(14, 6)
            };
            AppTheme.StyleTabs(tabMain);

            BuildByDateTab();
            BuildTopBooksTab();
            BuildTopStudentsTab();

            tabMain.Controls.Add(tabByDate);
            tabMain.Controls.Add(tabTopBooks);
            tabMain.Controls.Add(tabTopStudents);

            this.Controls.Add(tabMain);

            this.Load += ReportsForm_Load;
        }

        // ════════════════════════════════════════════════════════════════════
        //  TAB: ISSUES BY DATE
        // ════════════════════════════════════════════════════════════════════
        private void BuildByDateTab()
        {
            tabByDate = new TabPage("📅  Issues by Date");
            tabByDate.BackColor = AppTheme.Surface;

            var lblFrom = new Label { Text = "From:", Font = AppTheme.FontLabel, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(10, 14) };
            tabByDate.Controls.Add(lblFrom);

            dtpFrom = new DateTimePicker
            {
                Location = new Point(56, 10),
                Width = 150,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddMonths(-1)
            };
            tabByDate.Controls.Add(dtpFrom);

            var lblTo = new Label { Text = "To:", Font = AppTheme.FontLabel, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(220, 14) };
            tabByDate.Controls.Add(lblTo);

            dtpTo = new DateTimePicker
            {
                Location = new Point(248, 10),
                Width = 150,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };
            tabByDate.Controls.Add(dtpTo);

            btnLoadByDate = AppTheme.MakePrimaryBtn("📊  Load Report", 150, 34);
            btnLoadByDate.Location = new Point(412, 8);
            btnLoadByDate.Click += BtnLoadByDate_Click;
            tabByDate.Controls.Add(btnLoadByDate);

            dgvByDate = new DataGridView
            {
                Location = new Point(10, 52),
                Size = new Size(876, 302),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            AppTheme.StyleGrid(dgvByDate);
            tabByDate.Controls.Add(dgvByDate);

            tabByDate.Resize += (s, e) =>
            {
                dgvByDate.Size = new Size(tabByDate.ClientSize.Width - 20, tabByDate.ClientSize.Height - 64);
            };
        }

        // ════════════════════════════════════════════════════════════════════
        //  TAB: TOP BOOKS
        // ════════════════════════════════════════════════════════════════════
        private void BuildTopBooksTab()
        {
            tabTopBooks = new TabPage("📚  Top Books");
            tabTopBooks.BackColor = AppTheme.Surface;

            btnLoadTopBooks = AppTheme.MakePrimaryBtn("📚  Load Top Books", 180, 34);
            btnLoadTopBooks.Location = new Point(10, 8);
            btnLoadTopBooks.Click += BtnLoadTopBooks_Click;
            tabTopBooks.Controls.Add(btnLoadTopBooks);

            dgvTopBooks = new DataGridView
            {
                Location = new Point(10, 52),
                Size = new Size(876, 302),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            AppTheme.StyleGrid(dgvTopBooks);
            tabTopBooks.Controls.Add(dgvTopBooks);

            tabTopBooks.Resize += (s, e) =>
            {
                dgvTopBooks.Size = new Size(tabTopBooks.ClientSize.Width - 20, tabTopBooks.ClientSize.Height - 64);
            };
        }

        // ════════════════════════════════════════════════════════════════════
        //  TAB: TOP STUDENTS
        // ════════════════════════════════════════════════════════════════════
        private void BuildTopStudentsTab()
        {
            tabTopStudents = new TabPage("🎓  Top Students");
            tabTopStudents.BackColor = AppTheme.Surface;

            btnLoadTopStudents = AppTheme.MakePrimaryBtn("🎓  Load Top Students", 200, 34);
            btnLoadTopStudents.Location = new Point(10, 8);
            btnLoadTopStudents.Click += BtnLoadTopStudents_Click;
            tabTopStudents.Controls.Add(btnLoadTopStudents);

            dgvTopStudents = new DataGridView
            {
                Location = new Point(10, 52),
                Size = new Size(876, 302),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            AppTheme.StyleGrid(dgvTopStudents);
            tabTopStudents.Controls.Add(dgvTopStudents);

            tabTopStudents.Resize += (s, e) =>
            {
                dgvTopStudents.Size = new Size(tabTopStudents.ClientSize.Width - 20, tabTopStudents.ClientSize.Height - 64);
            };
        }

        // ════════════════════════════════════════════════════════════════════
        //  FORM LOAD
        // ════════════════════════════════════════════════════════════════════
        private void ReportsForm_Load(object sender, EventArgs e)
        {
            LoadStatCards();
        }

        // ════════════════════════════════════════════════════════════════════
        //  STAT CARDS
        // ════════════════════════════════════════════════════════════════════
        private void LoadStatCards()
        {
            try
            {
                object totalIssues   = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM BookIssue");
                object totalReturned = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM BookIssue WHERE Status='Returned'");
                object totalFine     = DatabaseHelper.ExecuteScalar(
                    "SELECT ISNULL(SUM(FineAmount),0) FROM BookIssue WHERE ReturnedDate IS NOT NULL");

                SetStatCardValue(cardTotalIssues,   (totalIssues   ?? 0).ToString());
                SetStatCardValue(cardTotalReturned, (totalReturned ?? 0).ToString());

                decimal fine = (totalFine != null && totalFine != DBNull.Value) ? Convert.ToDecimal(totalFine) : 0m;
                SetStatCardValue(cardTotalFine, fine.ToString("F0"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load summary stats:\n" + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SetStatCardValue(Panel card, string value)
        {
            foreach (Control c in card.Controls)
            {
                if (c is Label lbl && lbl.Font != null && lbl.Font.Size >= 20)
                {
                    lbl.Text = value;
                    return;
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  ISSUES BY DATE
        // ════════════════════════════════════════════════════════════════════
        private void BtnLoadByDate_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = @"SELECT s.EnrollmentNo,
                                      s.FirstName+' '+ISNULL(s.LastName,'') AS Student,
                                      b.BookName, bi.IssuedDate, bi.DueDate,
                                      bi.Status, bi.FineAmount
                               FROM BookIssue bi
                               JOIN Student s ON bi.StudentID=s.StudentID
                               JOIN Books b ON bi.BookID=b.BookID
                               WHERE CAST(bi.IssuedDate AS DATE) BETWEEN @from AND @to
                               ORDER BY bi.IssuedDate DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql,
                    new SqlParameter[] {
                        new SqlParameter("@from", dtpFrom.Value.Date),
                        new SqlParameter("@to",   dtpTo.Value.Date)
                    });

                dgvByDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load report:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  TOP BOOKS
        // ════════════════════════════════════════════════════════════════════
        private void BtnLoadTopBooks_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = @"SELECT TOP 10 b.BookName, b.Author, COUNT(*) AS [Times Borrowed]
                               FROM BookIssue bi
                               JOIN Books b ON bi.BookID=b.BookID
                               GROUP BY b.BookName, b.Author
                               ORDER BY COUNT(*) DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql);
                dgvTopBooks.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load top books:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  TOP STUDENTS
        // ════════════════════════════════════════════════════════════════════
        private void BtnLoadTopStudents_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = @"SELECT TOP 10
                                      s.EnrollmentNo,
                                      s.FirstName+' '+ISNULL(s.LastName,'') AS Student,
                                      s.Department,
                                      COUNT(*) AS [Books Borrowed],
                                      SUM(ISNULL(bi.FineAmount,0)) AS [Total Fine]
                               FROM BookIssue bi
                               JOIN Student s ON bi.StudentID=s.StudentID
                               GROUP BY s.EnrollmentNo, s.FirstName, s.LastName, s.Department
                               ORDER BY COUNT(*) DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql);
                dgvTopStudents.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load top students:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
