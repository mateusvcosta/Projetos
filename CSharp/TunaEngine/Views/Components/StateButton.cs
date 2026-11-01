namespace TunaEngine.Views;

public sealed class StateButton : Button
{
    public StateButton()
    {
        var commonStates = new VisualStateGroup { Name = "CommonStates" };
        commonStates.States.Add(new VisualState { Name = "Normal" });
        commonStates.States.Add(new VisualState
        {
            Name = "PointerOver",
            Setters =
            {
                new Setter
                {
                    Property = VisualElement.OpacityProperty,
                    Value = 0.88
                }
            }
        });
        commonStates.States.Add(new VisualState
        {
            Name = "Pressed",
            Setters =
            {
                new Setter
                {
                    Property = VisualElement.OpacityProperty,
                    Value = 0.72
                }
            }
        });
        commonStates.States.Add(new VisualState
        {
            Name = "Focused",
            Setters =
            {
                new Setter
                {
                    Property = Button.BorderColorProperty,
                    Value = ViewTheme.Accent
                },
                new Setter
                {
                    Property = Button.BorderWidthProperty,
                    Value = 2
                }
            }
        });
        commonStates.States.Add(new VisualState
        {
            Name = "Disabled",
            Setters =
            {
                new Setter
                {
                    Property = VisualElement.OpacityProperty,
                    Value = 0.45
                }
            }
        });

        VisualStateManager.SetVisualStateGroups(this, [commonStates]);
    }
}
