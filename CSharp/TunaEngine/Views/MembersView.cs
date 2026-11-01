using System.Net.Http;
using System.Text.Json;
using TunaEngine.Services;
using TunaEngine.Views.Popups;

namespace TunaEngine.Views;

public sealed class MembersView : ContentView
{
    private readonly TunaEngineApiClient _apiClient;
    private readonly Label _status = ViewTheme.TextLabel("A carregar membros...", 13, color: ViewTheme.Muted);
    private readonly Button _retryButton;
    private readonly List<string> _roleNames = [];
    private readonly List<ApiRole> _roles = [];
    private readonly VerticalStackLayout _table = new()
    {
        Spacing = 0,
        HorizontalOptions = LayoutOptions.Fill
    };
    private bool _isInitialized;

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
        if (_isInitialized)
        {
            RefreshRows();
            return;
        }

        _isInitialized = true;
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

        content.Children.Add(new Border
        {
            BackgroundColor = ViewTheme.Surface,
            Stroke = ViewTheme.Stroke,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            Padding = 0,
            Content = _table
        });

        Content = content;
        RefreshRows();
    }

    private void RefreshRows()
    {
        _table.Children.Clear();
        _table.Children.Add(BuildTableHeader());
        for (var index = 0; index < AppSession.Members.Count; index++)
        {
            _table.Children.Add(BuildMemberRow(AppSession.Members[index], index));
        }
    }

    private async Task LoadMembersAsync()
    {
        _status.Text = "A carregar membros...";
        _status.TextColor = ViewTheme.Muted;
        _retryButton.IsVisible = false;

        try
        {
            var members = await _apiClient.GetAsync<List<ApiMember>>("api/members");

            AppSession.Members.Clear();
            AppSession.Members.AddRange(members.Select(member => new TeamMember
            {
                Id = member.Id,
                Name = member.Name,
                Role = member.RoleId ?? "Membro",
                TunaName = member.TunaName ?? "",
                Email = member.Email ?? "",
                Phone = member.Phone ?? "",
                StudentNumber = member.StudentNumber ?? "",
                VoiceType = member.VoiceType ?? member.LegacyVoiceType ?? "",
                Instrument = member.Instrument ?? "",
                Password = member.Password ?? ""
            }));

            _status.Text = members.Count == 0 ? "Não existem membros na base de dados." : "";
            RefreshRows();

            if (members.Count == 0)
            {
                return;
            }

            try
            {
                var roles = await _apiClient.GetAsync<List<ApiRole>>("api/roles");
                _roleNames.Clear();
                _roleNames.AddRange(roles
                    .Select(role => role.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct(StringComparer.CurrentCultureIgnoreCase));
                foreach (var member in AppSession.Members)
                {
                    var role = roles.FirstOrDefault(candidate =>
                        string.Equals(candidate.Id, member.Role, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(candidate.RoleCode, member.Role, StringComparison.OrdinalIgnoreCase));
                    if (role is not null)
                    {
                        member.Role = role.Name;
                    }
                }

                _status.Text = "";
                RefreshRows();
            }
            catch (Exception roleException) when (
                roleException is HttpRequestException or JsonException or TaskCanceledException or InvalidOperationException)
            {
                _status.Text = $"Os membros foram carregados, mas não foi possível carregar os cargos: {roleException.Message}";
                _status.TextColor = ViewTheme.Error;
            }
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
            ViewTheme.TextLabel("Naipe", 12, true, ViewTheme.Muted),
            ViewTheme.TextLabel("Ações", 12, true, ViewTheme.Muted)
        };
        cells[3].HorizontalTextAlignment = TextAlignment.End;

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

        var voiceTypeCell = new Grid
        {
            Padding = new Thickness(0)
        };
        voiceTypeCell.Add(ViewTheme.TextLabel(
            string.IsNullOrWhiteSpace(member.VoiceType) ? "—" : member.VoiceType,
            14,
            color: Color.FromArgb("#CCCCDD")));
        row.Add(voiceTypeCell, 2, 0);

        var actionCell = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 8,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center
        };
        actionCell.Add(MemberAction("pencil.svg", $"Editar {member.Name}", ViewTheme.Muted,
            () => EditMember(member), "MemberEditButton"), 0, 0);
        actionCell.Add(MemberAction("trash.svg", $"Remover {member.Name}", ViewTheme.Error,
            () => RemoveMember(member), "MemberRemoveButton"), 1, 0);
        row.Add(actionCell, 3, 0);

        AddBottomBorder(row);
        return row;
    }

    private static View MemberAction(
        string icon,
        string description,
        Color color,
        Func<Task> callback,
        string automationId)
        => TableAction(icon, description, color, callback, automationId);

    private static View TableAction(
        string icon,
        string description,
        Color color,
        Func<Task> callback,
        string automationId)
    {
        var button = new ImageButton
        {
            AutomationId = automationId,
            Source = ImageSource.FromFile(icon.Replace(".svg", ".png", StringComparison.OrdinalIgnoreCase)),
            BackgroundColor = Colors.Transparent,
            Padding = 8,
            BorderWidth = 0,
            BorderColor = Colors.Transparent,
            CornerRadius = 6,
            HeightRequest = 32,
            WidthRequest = 32,
            Aspect = Aspect.AspectFit,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        SemanticProperties.SetDescription(button, description);
        ViewTheme.BindAction(button, callback);
        return button;
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
                new ColumnDefinition(new GridLength(500, GridUnitType.Star)),
                new ColumnDefinition(new GridLength(200, GridUnitType.Star)),
                new ColumnDefinition(new GridLength(200, GridUnitType.Star)),
                new ColumnDefinition(new GridLength(180, GridUnitType.Star))
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

    private async Task RemoveMember(TeamMember member)
    {
        var shell = Shell.Current;
        if (shell is null)
        {
            return;
        }

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

    private async Task EditMember(TeamMember? member)
    {
        var shell = Shell.Current;
        if (shell is null)
        {
            return;
        }

        if (_roleNames.Count == 0)
        {
            try
            {
                var roles = await _apiClient.GetAsync<List<ApiRole>>("api/roles");
                _roles.Clear();
                _roles.AddRange(roles);
                _roleNames.AddRange(roles
                    .Select(role => role.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct(StringComparer.CurrentCultureIgnoreCase));
            }
            catch (Exception exception) when (
                exception is HttpRequestException or JsonException or TaskCanceledException or InvalidOperationException)
            {
                _status.Text = $"Não foi possível carregar os cargos: {exception.Message}";
                _status.TextColor = ViewTheme.Error;
                return;
            }
        }

        var popup = new AddMemberPopup(member, _roleNames);
        var draft = await ModalOverlay.ShowAsync(
            shell.CurrentPage,
            popup,
            popup.Completion,
            popup.Cancel);
        if (draft is null)
        {
            return;
        }

        member ??= new TeamMember();
        member.Name = draft.Name;
        member.Role = draft.Role;
        member.TunaName = draft.TunaName;
        member.Email = draft.Email;
        member.Phone = draft.Phone;
        member.StudentNumber = draft.StudentNumber;
        member.VoiceType = draft.VoiceType;
        member.Instrument = draft.Instrument;
        member.Password = draft.Password;

        var role = _roles.FirstOrDefault(candidate =>
            string.Equals(candidate.Name, draft.Role, StringComparison.CurrentCultureIgnoreCase)
            || string.Equals(candidate.Id, draft.Role, StringComparison.OrdinalIgnoreCase)
            || string.Equals(candidate.RoleCode, draft.Role, StringComparison.OrdinalIgnoreCase));

        var request = new
        {
            name = member.Name,
            tunaName = member.TunaName,
            email = member.Email,
            password = member.Password,
            phone = member.Phone,
            studentNumber = member.StudentNumber,
            roleId = role?.Id ?? member.Role,
            voiceType = member.VoiceType,
            instrument = member.Instrument
        };

        try
        {
            if (string.IsNullOrWhiteSpace(member.Id))
            {
                var created = await _apiClient.PostAsync<ApiMember>(
                    "api/members", request);
                member.Id = created.Id;
            }
            else
            {
                await _apiClient.PutAsync($"api/members/{member.Id}", request);
            }

            if (!AppSession.Members.Contains(member))
            {
                AppSession.Members.Add(member);
            }

            _status.Text = "";
            _status.TextColor = ViewTheme.Muted;
        }
        catch (Exception exception) when (
            exception is HttpRequestException or JsonException or TaskCanceledException or InvalidOperationException)
        {
            _status.Text = $"Não foi possível guardar o membro: {exception.Message}";
            _status.TextColor = ViewTheme.Error;
            return;
        }

        Render();
    }
}
