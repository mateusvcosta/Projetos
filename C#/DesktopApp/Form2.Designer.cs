using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopApp
{
    partial class Form2
    {
        #region variables
        private System.ComponentModel.IContainer components = null;
        private new FlowLayoutPanel Menu = null!;
        private Panel Display = null!;
        private Label Footer = null!;
        private Label Header = null!;
        private PictureBox appLogo = null!;
        #endregion
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        Menu = new FlowLayoutPanel();
        Display = new Panel();
        Footer = new Label();
        Header = new Label();
        appLogo = new PictureBox();

        // Configure the main window and its starting size.
        Text = string.Empty;
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 560);
        BackColor = Color.FromArgb(15, 23, 42);
        ForeColor = Color.FromArgb(226, 232, 240);

        // Keep the navigation area across the top of the page.
        Menu.Dock = DockStyle.Top;
        Menu.Height = 76;
        Menu.FlowDirection = FlowDirection.LeftToRight;
        Menu.WrapContents = false;
        Menu.AutoScroll = true;
        Menu.Padding = new Padding(20, 16, 14, 10);
        Menu.BackColor = Color.FromArgb(17, 24, 39);

        appLogo.Width = 48;
        appLogo.Height = 44;
        appLogo.SizeMode = PictureBoxSizeMode.Zoom;
        appLogo.Margin = new Padding(0, 0, 12, 0);
        var logoPath = Path.Combine(AppContext.BaseDirectory, "images", "icon-cropped.png");
        if (File.Exists(logoPath))
        {
            using var logoSource = Image.FromFile(logoPath);
            appLogo.Image = new Bitmap(logoSource);
        }
        Menu.Controls.Add(appLogo);

        //MENU BUTTONS
        for (var index = 0; index < Titles.Length; index++)
        {
            var menuButton = new Button
            {
                Text = Titles[index],
                Width = 160,
                Height = 44,
                Tag = index,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 8, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(226, 232, 240),
                BackColor = Color.FromArgb(31, 41, 55),
                Margin = new Padding(0, 0, 8, 0),
                UseVisualStyleBackColor = false,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.FromArgb(51, 65, 85) },
                FlatStyle = FlatStyle.Flat
            };
            menuButton.Click += (_, _) => ShowAPP((int)menuButton.Tag);
            Menu.Controls.Add(menuButton);
        }

        // Display the current section title across the top of the content area.
        Header.Dock = DockStyle.Top;
        Header.Height = 100;
        Header.Font = new Font("Segoe UI", 18, FontStyle.Bold);
        Header.Padding = new Padding(24, 24, 24, 0);
        Header.ForeColor = Color.FromArgb(248, 250, 252);
        Header.BackColor = Color.FromArgb(15, 23, 42);

        // Let the example area fill the remaining space.
        Display.Dock = DockStyle.Fill;
        Display.Padding = new Padding(24);
        Display.BackColor = Color.FromArgb(15, 23, 42);

        // Show the current app page at the bottom of the window.
        Footer.AutoSize = false;
        Footer.Height = 40;
        Footer.Dock = DockStyle.Bottom;
        Footer.Padding = new Padding(0, 8, 24, 8);
        Footer.TextAlign = ContentAlignment.BottomRight;
        Footer.ForeColor = Color.FromArgb(148, 163, 184);
        Footer.BackColor = Color.FromArgb(15, 23, 42);

        // Add each control to Form2 so it becomes visible in the window.
        Controls.Add(Display);
        Controls.Add(Footer);
        Controls.Add(Header);
        Controls.Add(Menu);
    }








    }
}