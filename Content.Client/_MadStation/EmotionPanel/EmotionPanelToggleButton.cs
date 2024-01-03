using Content.Shared.Mobs.Components;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._MadStation.EmotionPanel;

public sealed class EmotionPanelToggleButton : Button
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    private readonly EmotionPanel? _emotionPanel;

    public EmotionPanelToggleButton()
    {
        IoCManager.InjectDependencies(this);

        AddChild(new TextureRect
        {
            TexturePath = "/Textures/Interface/Nano/smile.svg.96dpi.png",
            HorizontalAlignment = HAlignment.Center,
            VerticalAlignment = VAlignment.Center
        });

        _emotionPanel = new EmotionPanel();

        _emotionPanel.OnOpen += OnWindowOpen;
        _emotionPanel.OnClose += OnWindowClose;

        _playerManager.LocalPlayerAttached += OnLocalPlayerAttached;
        _playerManager.LocalPlayerDetached += OnLocalPlayerDetached;

        ToggleMode = true;
        OnToggled += OnButtonToggled;
    }

    private void OnWindowOpen()
    {
        Pressed = true;
    }

    private void OnWindowClose()
    {
        Pressed = false;
    }

    private void OnLocalPlayerAttached(EntityUid player)
    {
        Visible = _entityManager.HasComponent<MobStateComponent>(player);
    }

    private void OnLocalPlayerDetached(EntityUid obj)
    {
        _emotionPanel?.Close();
        Visible = false;
    }

    private void OnButtonToggled(ButtonToggledEventArgs obj)
    {
        if (_emotionPanel == null)
            return;

        if (_emotionPanel.IsOpen)
        {
            _emotionPanel.Close();
        }
        else
        {
            _emotionPanel.OpenCentered();
        }
    }

    protected override void ExitedTree()
    {
        base.ExitedTree();

        _emotionPanel?.Close();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (_emotionPanel == null) return;

        _emotionPanel.OnOpen -= OnWindowOpen;
        _emotionPanel.OnClose -= OnWindowClose;

    }
}
