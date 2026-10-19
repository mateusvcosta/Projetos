using System.Net.Http;
using System.Text.Json;
using TunaEngine.Services;

namespace TunaEngine.Views;

public sealed class MembersView : ContentView
{
    private readonly TunaEngineApiClient _apiClient;
    private readonly Label _status = ViewTheme.TextLabel("A carregar membros...", 13, color: ViewTheme.Muted);
    private readonly Button _retryButton;

    private static readonly Color[] AvatarColors =
    [
        Color.FromArgb("#E8526A"),
        Color.FromArgb("#6B7DB3"),
        Color.FromArgb("#9B6B9B")
    ];

    public MembersView(TunaEngineApiClient apiClient)
    {
        _apiClient = apiClient;
        _retryButton = ViewTheme.Action("Tentar novamente", LoadMembersAsync, primary: false);
        _retryButton.IsVisible = false;
        Render();
        _ = LoadMembersAsync();
    }

    private void Render()
    {
        Content = null;
        var content = new VerticalStackLayout
        {
            Spacing = 0,
            HorizontalOptions = LayoutOptions.Fill
        };

        var header = new Grid
        {
            HeightRequest = 80,
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(300))
            },
            RowDefinitions =
            {
                new RowDefinition(new GridLength(36)),
                new RowDefinition(new GridLength(8)),
                new RowDefinition(new GridLength(17)),
                new RowDefinition(new GridLength(19))
            }
        };

        var title = ViewTheme.TextLabel("Membros", 30, true);
        SemanticProperties.SetHeadingLevel(title, SemanticHeadingLevel.Level1);
        header.Add(title, 0, 0);

        var subtitle = ViewTheme.TextLabel(
            "Gestão da equipa da tuna.",
            14,
            color: ViewTheme.Muted);
        header.Add(subtitle, 0, 2);

        var addButton = ViewTheme.Action("+ Novo membro", () => EditMember(null));
        addButton.WidthRequest = 200;
        addButton.HeightRequest = 44;
        addButton.MinimumHeightRequest = 44;
        addButton.FontSize = 14;
        addButton.Padding = 0;
        addButton.HorizontalOptions = LayoutOptions.Start;
        addButton.VerticalOptions = LayoutOptions.Start;
        header.Add(addButton, 1, 0);
        Grid.SetRowSpan(addButton, 3);
        content.Children.Add(header);
        content.Children.Add(_status);
        content.Children.Add(_retryButton);

        var table = new VerticalStackLayout
        {
            Spacing = 0,
            HorizontalOptions = LayoutOptions.Fill
        };
        table.Children.Add(BuildTableHeader());
        for (var index = 0; index < AppSession.Members.Count; index++)
        {
            table.Children.Add(BuildMemberRow(AppSession.Members[index], index));
        }

        content.Children.Add(new Border
        {
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            Padding = 0,
            Content = table
        });

        Content = content;
    }

    private async Task LoadMembersAsync()
    {
        _status.Text = "A carregar membros...";
        _status.TextColor = ViewTheme.Muted;
        _retryButton.IsVisible = false;

        try
        {
            var memberTask = _apiClient.GetAsync<List<ApiMember>>("api/members");
            var roleTask = _apiClient.GetAsync<List<ApiRole>>("api/roles");
            await Task.WhenAll(memberTask, roleTask);
            var roles = await roleTask;
            var members = await memberTask;

            AppSession.Members.Clear();
            AppSession.Members.AddRange(members.Select(member =>
            {
                var role = roles.FirstOrDefault(candidate =>
                    string.Equals(candidate.Id, member.RoleId, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(candidate.RoleCode, member.RoleId, StringComparison.OrdinalIgnoreCase));
                return new TeamMember
                {
                    Id = member.Id,
                    Name = member.Name,
                    Role = role?.Name ?? member.RoleId ?? "Membro"
                };
            }));

            _status.Text = members.Count == 0 ? "Não existem membros na base de dados." : "";
            Render();
        }
        catch (Exception exception) when (
            exception is HttpRequestException or JsonException or TaskCanceledException or InvalidOperationException)
        {
            _status.Text = $"Não foi possível carregar os membros: {exception.Message}";
            _status.TextColor = ViewTheme.Error;
            _retryButton.IsVisible = true;
        }
    }

    private static Grid BuildTableHeader()
    {
        var header = CreateTableRow(44, ViewTheme.SurfaceDark);
        var cells = new[]
        {
            ViewTheme.TextLabel("Nome", 12, true, ViewTheme.Muted),
            ViewTheme.TextLabel("Cargo", 12, true, ViewTheme.Muted),
            ViewTheme.TextLabel("Ações", 12, true, ViewTheme.Muted)
        };
        cells[2].HorizontalTextAlignment = TextAlignment.End;

        for (var column = 0; column < cells.Length; column++)
        {
            header.Add(cells[column], column, 0);
        }

        AddBottomBorder(header);
        return header;
    }

    private Grid BuildMemberRow(TeamMember member, int index)
    {
        var row = CreateTableRow(60);
        var nameCell = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(36)),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 12
        };
        nameCell.Add(new Microsoft.Maui.Controls.Shapes.Ellipse
        {
            WidthRequest = 36,
            HeightRequest = 36,
            Fill = AvatarColors[index % AvatarColors.Length],
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center
        }, 0, 0);
        nameCell.Add(ViewTheme.TextLabel(member.Name, 14, true), 1, 0);
        row.Add(nameCell, 0, 0);

        var roleCell = new Grid
        {
            Padding = new Thickness(0)
        };
        roleCell.Add(ViewTheme.TextLabel(member.Role, 14, color: Color.FromArgb("#CCCCDD")));
        row.Add(roleCell, 1, 0);

        var actionCell = new Grid
        {
            Padding = new Thickness(0)
        };
        var actionButton = new Button
        {
            Text = "•••",
            FontSize = 18,
            BackgroundColor = Colors.Transparent,
            TextColor = ViewTheme.Muted,
            Padding = 0,
            CornerRadius = 0,
            MinimumWidthRequest = 18,
            MinimumHeightRequest = 32,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center
        };
        SemanticProperties.SetDescription(actionButton, $"Ações para {member.Name}");
        actionButton.Clicked += async (_, _) => await MemberActions(member);
        actionCell.Add(actionButton);
        row.Add(actionCell, 2, 0);

        AddBottomBorder(row);
        return row;
    }

    private static Grid CreateTableRow(double height, Color? background = null)
    {
        var row = new Grid
        {
            HeightRequest = height,
            Padding = new Thickness(20, 0),
            ColumnSpacing = 0,
            BackgroundColor = background ?? Colors.Transparent,
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(700, GridUnitType.Star)),
                new ColumnDefinition(new GridLength(300, GridUnitType.Star)),
                new ColumnDefinition(new GridLength(100, GridUnitType.Star))
            }
        };
        return row;
    }

    private static void AddBottomBorder(Grid row)
    {
        var divider = new BoxView
        {
            HeightRequest = 1,
            Color = ViewTheme.Stroke,
            VerticalOptions = LayoutOptions.End,
            InputTransparent = true
        };
        Grid.SetColumnSpan(divider, row.ColumnDefinitions.Count);
        row.Add(divider);
    }

    private async Task MemberActions(TeamMember member)
    {
        var shell = Shell.Current;
        if (shell is null)
        {
            return;
        }

        var action = await shell.DisplayActionSheetAsync(
            member.Name,
            "Cancelar",
            "Remover",
            "Editar");
        if (action == "Editar")
        {
            await EditMember(member);
        }
        else if (action == "Remover")
        {
            var confirmed = await shell.DisplayAlertAsync(
                "Remover membro",
                $"Remover “{member.Name}”?",
                "Remover",
                "Cancelar");
            if (!confirmed)
            {
                return;
            }

            AppSession.Members.Remove(member);
            Render();
        }
    }

    private async Task EditMember(TeamMember? member)
    {
        var shell = Shell.Current;
        if (shell is null)
        {
            return;
        }

        var name = await shell.DisplayPromptAsync(
            member is null ? "Novo membro" : "Editar membro",
            "Nome:",
            "Continuar",
            "Cancelar",
            initialValue: member?.Name ?? "");
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        var role = await shell.DisplayPromptAsync(
            "Cargo",
            "Cargo na equipa:",
            "Guardar",
            "Cancelar",
            initialValue: member?.Role ?? "Tuno");
        if (string.IsNullOrWhiteSpace(role))
        {
            return;
        }

        member ??= new TeamMember();
        member.Name = name.Trim();
        member.Role = role.Trim();
        if (!AppSession.Members.Contains(member))
        {
            AppSession.Members.Add(member);
        }

        Render();
    }
}
