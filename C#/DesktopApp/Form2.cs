using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktopApp
{
    public partial class Form2: Form
    {
        private readonly string[] Titles = {"Inventário", "Membros", "Calendário", "Configurações"};
        private readonly string[] Texts =
        {
            "INVENTÁRIO\n\nAqui você pode visualizar e organizar os itens do inventário.",
            "MEMBROS\n\nAqui você pode consultar e gerenciar os membros.",
            "CALENDÁRIO\n\nAqui você pode visualizar seus eventos e compromissos.",
            "CONFIGURAÇÕES\n\nAqui você pode ajustar as configurações do aplicativo."
        };
        private int AppIndex;

    public Form2()
    {
        InitializeComponent();
        SetAppIcon();
        ShowAPP(0);
    }

    private void SetAppIcon()
    {
        var iconPath = Path.Combine(AppContext.BaseDirectory, "images", "icon-cropped.png");
        if (!File.Exists(iconPath))
        {
            return;
        }

        using var bitmap = new Bitmap(iconPath);
        using var icon = Icon.FromHandle(bitmap.GetHicon());
        Icon = (Icon)icon.Clone();
    }

    private void ShowAPP(int index)
    {
            AppIndex = Math.Clamp(index, 0, Titles.Length - 1);
            Header.Text = Titles[AppIndex];
            Footer.Text = $"App {AppIndex + 1} of {Titles.Length}";
            Display.Controls.Clear();

            Display.Controls.Add(new Label
            {
                Text = Texts[AppIndex],
                Dock = DockStyle.Fill,
                AutoSize = false,
                Padding = new Padding(8),
                TextAlign = ContentAlignment.TopLeft,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(94, 234, 212),
                BackColor = Color.FromArgb(15, 23, 42)
            });

            foreach (Control control in Menu.Controls)
            {
                if (control is Button button && button.Tag is int buttonIndex)
                {
                    button.BackColor = buttonIndex == AppIndex
                        ? Color.FromArgb(15, 118, 110)
                        : Color.FromArgb(31, 41, 55);
                    button.ForeColor = buttonIndex == AppIndex
                        ? Color.White
                        : Color.FromArgb(226, 232, 240);
                }
            }
    }

    }
}