using System;
using System.Drawing;
using System.Windows.Forms;

namespace Library_Management_System
{
    /// <summary>
    /// Central design system — colors, fonts, and helper methods.
    /// Apply to any form: AppTheme.StyleForm(this);
    /// </summary>
    public static class AppTheme
    {
        // ── Brand Colors ───────────────────────────────────────
        public static readonly Color NavyDark    = Color.FromArgb(15,  23,  42);
        public static readonly Color NavyMid     = Color.FromArgb(30,  41,  59);
        public static readonly Color NavyLight   = Color.FromArgb(51,  65,  85);
        public static readonly Color Teal        = Color.FromArgb(20, 184, 166);
        public static readonly Color TealLight   = Color.FromArgb(94, 234, 212);
        public static readonly Color TealDark    = Color.FromArgb(13, 148, 136);
        public static readonly Color Accent      = Color.FromArgb(99, 102, 241);  // Indigo
        public static readonly Color AccentLight = Color.FromArgb(165, 180, 252);
        public static readonly Color Success     = Color.FromArgb(34, 197, 94);
        public static readonly Color Danger      = Color.FromArgb(239, 68,  68);
        public static readonly Color Warning     = Color.FromArgb(234, 179, 8);
        public static readonly Color WarningDark = Color.FromArgb(161, 98,  7);
        public static readonly Color Surface     = Color.FromArgb(248, 250, 252);
        public static readonly Color CardBg      = Color.White;
        public static readonly Color BorderColor = Color.FromArgb(226, 232, 240);
        public static readonly Color TextPrimary = Color.FromArgb(15,  23,  42);
        public static readonly Color TextMuted   = Color.FromArgb(100, 116, 139);

        // ── Fonts ──────────────────────────────────────────────
        public static readonly Font FontTitle   = new Font("Segoe UI", 20f, FontStyle.Bold);
        public static readonly Font FontH2      = new Font("Segoe UI", 14f, FontStyle.Bold);
        public static readonly Font FontH3      = new Font("Segoe UI", 11f, FontStyle.Bold);
        public static readonly Font FontBody    = new Font("Segoe UI",  9.5f);
        public static readonly Font FontSmall   = new Font("Segoe UI",  8.5f);
        public static readonly Font FontBtn     = new Font("Segoe UI", 10f, FontStyle.Bold);
        public static readonly Font FontInput   = new Font("Segoe UI", 10f);
        public static readonly Font FontLabel   = new Font("Segoe UI",  9.5f, FontStyle.Bold);
        public static readonly Font FontCard    = new Font("Segoe UI", 22f, FontStyle.Bold);
        public static readonly Font FontMono    = new Font("Consolas",  9.5f);

        // ── Header Panel ───────────────────────────────────────
        public static Panel MakeHeader(string title, string subtitle = "")
        {
            Panel header = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 70,
                BackColor = NavyDark,
                Padding   = new Padding(20, 0, 20, 0)
            };

            Label lblTitle = new Label
            {
                Text      = title,
                Font      = FontH2,
                ForeColor = TealLight,
                AutoSize  = false,
                Width     = 600,
                Height    = 35,
                Location  = new Point(20, 10),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label lblSub = new Label
            {
                Text      = subtitle,
                Font      = FontSmall,
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize  = false,
                Width     = 600,
                Height    = 22,
                Location  = new Point(22, 43),
                TextAlign = ContentAlignment.MiddleLeft
            };

            header.Controls.Add(lblTitle);
            if (!string.IsNullOrEmpty(subtitle)) header.Controls.Add(lblSub);
            return header;
        }

        // ── Stat Card ──────────────────────────────────────────
        public static Panel MakeStatCard(string value, string label, Color accent)
        {
            Panel card = new Panel
            {
                Width     = 170,
                Height    = 95,
                BackColor = CardBg,
                Cursor    = Cursors.Default
            };
            card.Paint += (s, e) =>
            {
                using (Pen p = new Pen(accent, 3))
                    e.Graphics.DrawLine(p, 0, card.Height - 3, card.Width, card.Height - 3);
                e.Graphics.DrawRectangle(new Pen(BorderColor), 0, 0, card.Width - 1, card.Height - 1);
            };

            Label valLbl = new Label
            {
                Text      = value,
                Font      = FontCard,
                ForeColor = accent,
                AutoSize  = false,
                Bounds    = new Rectangle(0, 12, card.Width, 48),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Label nameLbl = new Label
            {
                Text      = label,
                Font      = FontSmall,
                ForeColor = TextMuted,
                AutoSize  = false,
                Bounds    = new Rectangle(0, 58, card.Width, 26),
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(valLbl);
            card.Controls.Add(nameLbl);
            return card;
        }

        // ── Primary Button ────────────────────────────────────
        public static Button MakePrimaryBtn(string text, int width = 130, int height = 36)
        {
            Button btn = new Button
            {
                Text      = text,
                Width     = width,
                Height    = height,
                FlatStyle = FlatStyle.Flat,
                BackColor = Teal,
                ForeColor = Color.White,
                Font      = FontBtn,
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = TealDark;
            return btn;
        }

        // ── Danger Button ─────────────────────────────────────
        public static Button MakeDangerBtn(string text, int width = 130, int height = 36)
        {
            Button btn = new Button
            {
                Text      = text,
                Width     = width,
                Height    = height,
                FlatStyle = FlatStyle.Flat,
                BackColor = Danger,
                ForeColor = Color.White,
                Font      = FontBtn,
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(185, 28, 28);
            return btn;
        }

        // ── Success Button ────────────────────────────────────
        public static Button MakeSuccessBtn(string text, int width = 130, int height = 36)
        {
            Button btn = new Button
            {
                Text      = text,
                Width     = width,
                Height    = height,
                FlatStyle = FlatStyle.Flat,
                BackColor = Success,
                ForeColor = Color.White,
                Font      = FontBtn,
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(21, 128, 61);
            return btn;
        }

        // ── Warning Button ────────────────────────────────────
        public static Button MakeWarningBtn(string text, int width = 130, int height = 36)
        {
            Button btn = new Button
            {
                Text      = text,
                Width     = width,
                Height    = height,
                FlatStyle = FlatStyle.Flat,
                BackColor = Warning,
                ForeColor = Color.White,
                Font      = FontBtn,
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = WarningDark;
            return btn;
        }

        // ── Styled TextBox ────────────────────────────────────
        public static TextBox MakeInput(int width = 280, bool isPassword = false)
        {
            TextBox tb = new TextBox
            {
                Width        = width,
                Height       = 32,
                Font         = FontInput,
                BorderStyle  = BorderStyle.FixedSingle,
                BackColor    = Color.White,
                ForeColor    = TextPrimary
            };
            if (isPassword) tb.PasswordChar = '●';
            return tb;
        }

        // ── Styled Label ──────────────────────────────────────
        public static Label MakeLabel(string text, bool bold = true)
        {
            return new Label
            {
                Text      = text,
                Font      = bold ? FontLabel : FontBody,
                ForeColor = TextPrimary,
                AutoSize  = true
            };
        }

        // ── Style DataGridView ────────────────────────────────
        public static void StyleGrid(DataGridView dgv)
        {
            dgv.BackgroundColor         = Surface;
            dgv.BorderStyle             = BorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor   = NavyMid;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor   = TealLight;
            dgv.ColumnHeadersDefaultCellStyle.Font        = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment   = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight     = 36;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.DefaultCellStyle.Font   = FontBody;
            dgv.DefaultCellStyle.ForeColor = TextPrimary;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 251, 241);
            dgv.DefaultCellStyle.SelectionForeColor = NavyDark;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgv.RowTemplate.Height      = 32;
            dgv.GridColor               = BorderColor;
            dgv.CellBorderStyle         = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.RowHeadersVisible       = false;
            dgv.AutoSizeColumnsMode     = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode           = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect             = false;
            dgv.ReadOnly                = true;
            dgv.AllowUserToAddRows      = false;
            dgv.AllowUserToDeleteRows   = false;
        }

        // ── Style TabControl ──────────────────────────────────
        public static void StyleTabs(TabControl tc)
        {
            tc.Font       = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            tc.DrawMode   = TabDrawMode.Normal;
            tc.SizeMode   = TabSizeMode.Fixed;
            tc.ItemSize   = new Size(130, 36);
            tc.Appearance = TabAppearance.Normal;
            tc.Padding    = new Point(10, 4);
        }

        // ── Apply to Form ────────────────────────────────────
        public static void StyleForm(Form form)
        {
            form.BackColor   = Surface;
            form.Font        = FontBody;
            form.ForeColor   = TextPrimary;
        }

        // ── Section Title ────────────────────────────────────
        public static Label MakeSectionTitle(string text)
        {
            return new Label
            {
                Text      = text,
                Font      = FontH3,
                ForeColor = NavyDark,
                AutoSize  = true,
                Padding   = new Padding(0, 8, 0, 4)
            };
        }

        // ── Separator Line ────────────────────────────────────
        public static Panel MakeSeparator()
        {
            return new Panel
            {
                Height    = 2,
                Dock      = DockStyle.Top,
                BackColor = Teal
            };
        }

        // ── Sidebar Button ────────────────────────────────────
        public static Button MakeSidebarBtn(string text, int width = 200)
        {
            Button btn = new Button
            {
                Text      = "  " + text,
                Width     = width,
                Height    = 44,
                FlatStyle = FlatStyle.Flat,
                BackColor = NavyMid,
                ForeColor = Color.FromArgb(203, 213, 225),
                Font      = FontBody,
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = NavyLight;
            return btn;
        }
    }
}
