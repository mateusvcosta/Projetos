using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using TunaEngine.Views.Popups;
using System.Globalization;

namespace TunaEngine.Views;

public sealed class MembersView : ContentView
{
    private sealed record Member(string Name, string Role);

    private readonly List<Member> _members =
    [
        new Member("João Silva", "Tuno"),
        new Member("Maria Oliveira", "Tuno"),
        new Member("Carlos Santos", "Tuno")
    ];

    private readonly VerticalStackLayout memberRows = new()
    {
        Spacing = 0
    };

    private readonly Label MemberCountLabel = new()
    {
        FontSize = 22,
        FontAttributes = FontAttributes.Bold,
        TextColor = Color.FromArgb("#F3F5F4"),
        HorizontalTextAlignment = TextAlignment.End,
        VerticalTextAlignment = TextAlignment.Center
    };

    public MembersView(bool isWideLayout)
    {
        var titleLabel = new Label
        {
            Text = "Membros",
            FontSize = isWideLayout ? 34 : 29,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#F3F5F4")
        };
        SemanticProperties.SetHeadingLevel(titleLabel, SemanticHeadingLevel.Level1);

        var addMemberButton = new Button
        {
            Text = "Novo membro",
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#D94F65"),
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 6,
            Padding = new Thickness(14, 10),
            AutomationId = "AddMember"
        };
        addMemberButton.Clicked += async (_, _) =>
        {
            var popup = new AddMemberPopup();
            if (await ShowMemberPopupAsync(popup) is { } member)
            {
                _members.Add(new Member(member.Name, member.Role));
                RefreshMemberRows();
            }
        };

        var pageTitleRow = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 12,
            VerticalOptions = LayoutOptions.Center
        };
        pageTitleRow.Add(titleLabel, 0, 0);
        pageTitleRow.Add(addMemberButton, 1, 0);

        Content = new VerticalStackLayout
        {
            Spacing = 20,
            Padding = new Thickness(isWideLayout ? 0 : 4),
            Children =
            {
                pageTitleRow,
                MemberCountLabel,
                CreateTable()
            }
        };

        RefreshMemberRows();
    }

    private Border CreateTable()
    {
        var tableTitle = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 12,
            Padding = new Thickness(12, 8),
            BackgroundColor = Color.FromArgb("#1A262E")
        };
        tableTitle.Add(CreateTableLabel("TUNOS", true), 0, 0);

        var header = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(120))
            },
            ColumnSpacing = 12,
            Padding = new Thickness(12, 12),
            BackgroundColor = Color.FromArgb("#202D35")
        };
        header.Add(CreateTableLabel("NOME", true), 0, 0);
        header.Add(CreateTableLabel("CARGO", true), 1, 0);

        var tableContent = new VerticalStackLayout
        {
            Spacing = 0,
            Children =
            {
                tableTitle,
                header,
                new BoxView
                {
                    HeightRequest = 1,
                    BackgroundColor = Color.FromArgb("#2D3A42")
                },
                memberRows
            }
        };

        return new Border
        {
            BackgroundColor = Color.FromArgb("#1A262E"),
            Stroke = Color.FromArgb("#2D3A42"),
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            Content = tableContent
        };
    }

    private void RefreshMemberRows()
    {
        memberRows.Children.Clear();
        foreach (var member in _members)
        {
            memberRows.Children.Add(CreateMemberRow(member));
            memberRows.Children.Add(new BoxView
            {
                HeightRequest = 1,
                BackgroundColor = Color.FromArgb("#2D3A42")
            });
        }

        MemberCountLabel.Text = $"Total Members: {_members.Count}";
    }

    private static async Task<MemberDraft?> ShowMemberPopupAsync(AddMemberPopup popup)
    {
        var shell = Shell.Current;
        if (shell is null)
        {
            return null;
        }

        await shell.ShowPopupAsync(popup, new PopupOptions
        {
            CanBeDismissedByTappingOutsideOfPopup = false,
            PageOverlayColor = Color.FromArgb("#B0000000"),
            Shape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = 8,
                Fill = Color.FromArgb("#172129"),
                Stroke = Color.FromArgb("#172129"),
                StrokeThickness = 0
            },
            Shadow = null
        });

        return popup.Member;
    }

    private static Grid CreateMemberRow(Member member)
    {
        var row = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(120))
            },
            ColumnSpacing = 12,
            Padding = new Thickness(12, 14)
        };
        row.Add(CreateTableLabel(member.Name), 0, 0);
        row.Add(CreateTableLabel(member.Role), 1, 0);
        return row;
    }

    private static Label CreateTableLabel(string text, bool isHeader = false)
    {
        return new Label
        {
            Text = text,
            FontSize = isHeader ? 11 : 14,
            FontAttributes = isHeader ? FontAttributes.Bold : FontAttributes.None,
            TextColor = isHeader ? Color.FromArgb("#91A0A5") : Color.FromArgb("#F3F5F4"),
            VerticalTextAlignment = TextAlignment.Center
        };
    }
}
    