using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Library_Management_System
{
    public class CategoryManagement : Form
    {
        // ── private fields ────────────────────────────────────────────────────
        private int adminID;
        private int editingCategoryID = -1;

        // Layout
        private DataGridView dgvCategories;

        // Form fields
        private TextBox txtCategoryName;
        private TextBox txtDescription;
        private Button btnSave;
        private Button btnClear;

        // Grid buttons
        private Button btnEditSelected;
        private Button btnToggleActive;
        private Button btnDeleteCategory;

        // ────────────────────────────────────────────────────────────────────
        public CategoryManagement(int aid)
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
            this.Text = "Category Management";
            this.Size = new Size(750, 560);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // ── Header ───────────────────────────────────────────────────────
            var pnlHeader = AppTheme.MakeHeader("🏷️ Category Management", "Add and manage book categories");
            this.Controls.Add(pnlHeader);

            // ── Content area ─────────────────────────────────────────────────
            var pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12)
            };

            // ── Left side: Add/Edit form (w=320) ─────────────────────────────
            var pnlLeft = new Panel
            {
                Location = new Point(12, 82),
                Size = new Size(320, 440),
                BackColor = AppTheme.CardBg,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblFormTitle = AppTheme.MakeSectionTitle("Add / Edit Category");
            lblFormTitle.Location = new Point(12, 10);
            pnlLeft.Controls.Add(lblFormTitle);

            Label ML(string t) => new Label
            {
                Text = t,
                Font = AppTheme.FontLabel,
                ForeColor = AppTheme.TextPrimary,
                AutoSize = true
            };

            int fx = 12, fy = 44;

            pnlLeft.Controls.Add(ML("Category Name")); pnlLeft.Controls[pnlLeft.Controls.Count - 1].Location = new Point(fx, fy);
            txtCategoryName = AppTheme.MakeInput(280);
            txtCategoryName.Location = new Point(fx, fy + 18);
            pnlLeft.Controls.Add(txtCategoryName);

            fy += 62;
            pnlLeft.Controls.Add(ML("Description")); pnlLeft.Controls[pnlLeft.Controls.Count - 1].Location = new Point(fx, fy);
            txtDescription = new TextBox
            {
                Multiline = true,
                Height = 80,
                Width = 280,
                Font = AppTheme.FontInput,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = AppTheme.TextPrimary,
                Location = new Point(fx, fy + 18)
            };
            pnlLeft.Controls.Add(txtDescription);

            fy += 110;
            btnSave = AppTheme.MakeSuccessBtn("💾  Save", 130, 36);
            btnSave.Location = new Point(fx, fy);
            btnSave.Click += BtnSave_Click;
            pnlLeft.Controls.Add(btnSave);

            btnClear = AppTheme.MakeWarningBtn("✕  Clear", 110, 36);
            btnClear.Location = new Point(fx + 145, fy);
            btnClear.Click += BtnClear_Click;
            pnlLeft.Controls.Add(btnClear);

            this.Controls.Add(pnlLeft);

            // ── Right side: DataGridView (w=380) ─────────────────────────────
            var pnlRight = new Panel
            {
                Location = new Point(344, 82),
                Size = new Size(386, 440),
                BackColor = AppTheme.Surface
            };

            var lblGridTitle = AppTheme.MakeSectionTitle("All Categories");
            lblGridTitle.Location = new Point(0, 8);
            pnlRight.Controls.Add(lblGridTitle);

            dgvCategories = new DataGridView
            {
                Location = new Point(0, 38),
                Size = new Size(386, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            AppTheme.StyleGrid(dgvCategories);
            pnlRight.Controls.Add(dgvCategories);

            // Grid action buttons
            btnEditSelected = AppTheme.MakeWarningBtn("✏  Edit Selected", 150, 34);
            btnEditSelected.Location = new Point(0, 348);
            btnEditSelected.Click += BtnEditSelected_Click;
            pnlRight.Controls.Add(btnEditSelected);

            btnToggleActive = AppTheme.MakePrimaryBtn("⟳  Toggle Active", 150, 34);
            btnToggleActive.Location = new Point(0, 390);
            btnToggleActive.Click += BtnToggleActive_Click;
            pnlRight.Controls.Add(btnToggleActive);

            btnDeleteCategory = AppTheme.MakeDangerBtn("🗑  Delete", 110, 34);
            btnDeleteCategory.Location = new Point(160, 390);
            btnDeleteCategory.Click += BtnDeleteCategory_Click;
            pnlRight.Controls.Add(btnDeleteCategory);

            this.Controls.Add(pnlRight);

            this.Load += CategoryManagement_Load;
        }

        // ════════════════════════════════════════════════════════════════════
        //  FORM LOAD
        // ════════════════════════════════════════════════════════════════════
        private void CategoryManagement_Load(object sender, EventArgs e)
        {
            LoadCategories();
        }

        // ════════════════════════════════════════════════════════════════════
        //  LOAD CATEGORIES
        // ════════════════════════════════════════════════════════════════════
        private void LoadCategories()
        {
            try
            {
                string sql = "SELECT CategoryID, CategoryName, Description, IsActive, CreatedDate " +
                             "FROM Category ORDER BY CategoryName";
                DataTable dt = DatabaseHelper.ExecuteQuery(sql);
                dgvCategories.DataSource = dt;
                if (dgvCategories.Columns.Contains("CategoryID"))
                    dgvCategories.Columns["CategoryID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load categories:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  SAVE (ADD or EDIT)
        // ════════════════════════════════════════════════════════════════════
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Category Name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return;
            }

            try
            {
                if (editingCategoryID < 0)
                {
                    // INSERT
                    string sql = "INSERT INTO Category (CategoryName, Description, IsActive, CreatedDate) " +
                                 "VALUES (@name, @desc, 1, GETDATE())";
                    DatabaseHelper.ExecuteNonQuery(sql,
                        new SqlParameter("@name", txtCategoryName.Text.Trim()),
                        new SqlParameter("@desc", string.IsNullOrWhiteSpace(txtDescription.Text) ? (object)DBNull.Value : txtDescription.Text.Trim()));
                    MessageBox.Show("Category added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // UPDATE
                    string sql = "UPDATE Category SET CategoryName=@name, Description=@desc WHERE CategoryID=@id";
                    DatabaseHelper.ExecuteNonQuery(sql,
                        new SqlParameter("@name", txtCategoryName.Text.Trim()),
                        new SqlParameter("@desc", string.IsNullOrWhiteSpace(txtDescription.Text) ? (object)DBNull.Value : txtDescription.Text.Trim()),
                        new SqlParameter("@id",   editingCategoryID));
                    MessageBox.Show("Category updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ClearForm();
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save category:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  CLEAR
        // ════════════════════════════════════════════════════════════════════
        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtCategoryName.Clear();
            txtDescription.Clear();
            editingCategoryID = -1;
            btnSave.Text = "💾  Save";
        }

        // ════════════════════════════════════════════════════════════════════
        //  EDIT SELECTED
        // ════════════════════════════════════════════════════════════════════
        private void BtnEditSelected_Click(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a category to edit.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow row = dgvCategories.SelectedRows[0];
            editingCategoryID = Convert.ToInt32(row.Cells["CategoryID"].Value);
            txtCategoryName.Text = row.Cells["CategoryName"].Value?.ToString() ?? "";
            txtDescription.Text  = row.Cells["Description"].Value?.ToString() ?? "";
            btnSave.Text = "💾  Update";
            txtCategoryName.Focus();
        }

        // ════════════════════════════════════════════════════════════════════
        //  TOGGLE ACTIVE
        // ════════════════════════════════════════════════════════════════════
        private void BtnToggleActive_Click(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a category.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int id = Convert.ToInt32(dgvCategories.SelectedRows[0].Cells["CategoryID"].Value);
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE Category SET IsActive = CASE WHEN IsActive=1 THEN 0 ELSE 1 END WHERE CategoryID=@id",
                    new SqlParameter("@id", id));
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to toggle status:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  DELETE
        // ════════════════════════════════════════════════════════════════════
        private void BtnDeleteCategory_Click(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a category to delete.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = Convert.ToInt32(dgvCategories.SelectedRows[0].Cells["CategoryID"].Value);
            string name = dgvCategories.SelectedRows[0].Cells["CategoryName"].Value?.ToString() ?? "";

            // Check if any books use this category
            try
            {
                object bookCount = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Books WHERE CategoryID=@id",
                    new SqlParameter("@id", id));

                if (Convert.ToInt32(bookCount) > 0)
                {
                    MessageBox.Show($"Cannot delete '{name}' — it is used by one or more books.\nToggle it inactive instead.",
                        "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to check category usage:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show($"Delete category '{name}'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                DatabaseHelper.ExecuteNonQuery("DELETE FROM Category WHERE CategoryID=@id", new SqlParameter("@id", id));
                MessageBox.Show("Category deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete category:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
