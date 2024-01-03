using Content.Shared.Chat.Prototypes;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;

namespace Content.Client._MadStation.EmotionPanel;

public sealed class EmotionButton : Button
{
    public EmotePrototype EmotePrototype { get; }
    private readonly FavoriteEmotesManager _favoriteEmotesManager = default!;

    public EmotionButton(EmotePrototype emotePrototype, FavoriteEmotesManager favoriteEmotesManager)
    {
        EmotePrototype = emotePrototype;
        _favoriteEmotesManager = favoriteEmotesManager;

        var favoriteCheckBox = new CheckBox
        {
            SetWidth = 20,
            SetHeight = 20,
            HorizontalAlignment = HAlignment.Right,
            VerticalAlignment = VAlignment.Center,
            Pressed = _favoriteEmotesManager.IsFavorite(emotePrototype)
        };

        favoriteCheckBox.OnToggled += OnCheckboxToggled;

        AddChild(favoriteCheckBox);
    }

    private void OnCheckboxToggled(ButtonToggledEventArgs state)
    {
        if (state.Pressed)
        {
            _favoriteEmotesManager.AddToFavorite(EmotePrototype);
        }
        else
        {
            _favoriteEmotesManager.RemoveFromFavorite(EmotePrototype);
        }
    }
}
