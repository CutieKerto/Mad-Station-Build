﻿using System.Linq;
using Content.Client.Chat.Managers;
using Content.Shared._MadStation.Cvars;
using Content.Shared.Chat;
using Content.Shared.Chat.Prototypes;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Client._MadStation.EmotionPanel;

[GenerateTypedNameReferences]
public sealed partial class EmotionPanel : DefaultWindow
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly FavoriteEmotesManager _favoriteEmotes = default!;

    private DateTime _lastEmotionTimeUse = DateTime.Now;
    private float _emotionCooldown = default!;

    private bool _initialized;

    public EmotionPanel()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);
        _cfg.OnValueChanged(MadCvars.EmoteCooldown, f => _emotionCooldown = f, true);
        _favoriteEmotes.OnFavoritesUpdate += OnFavoritesUpdate;
        TabContainer.SetTabTitle(0, "Все эмоции");
        TabContainer.SetTabTitle(1, "Избранное");
    }

    protected override void Opened()
    {
        base.Opened();

        if (_initialized)
        {
            _initialized = true;
            return;
        }

        FillEmoteContainer();
        FillFavoritesContainer();
    }

    private void FillEmoteContainer()
    {
        var emotions = _prototypeManager.EnumeratePrototypes<EmotePrototype>()
            .ToList();

        emotions.RemoveAll(x => x.HideInEmotePanel);
        emotions.Sort((first, second) =>
            string.Compare(first.ChatMessages.First(), second.ChatMessages.First(), StringComparison.Ordinal));

        foreach (var emote in emotions)
        {
            var button = CreateEmoteButton(emote);

            EmotionsContainer.AddChild(button);
        }
    }

    private void OnFavoritesUpdate(object? sender, FavoriteEmotesChangedEventArgs args)
    {
        if (args.Added != null)
        {
            var emoteButton = CreateEmoteButton(args.Added);
            FavoritesContainer.AddChild(emoteButton);
        }
        else if (args.Removed != null)
        {
            var emoteButton =
                FavoritesContainer.Children.FirstOrDefault(
                    x => ((EmotionButton) x).EmotePrototype.ID == args.Removed.ID);
            if (emoteButton == null)
            {
                return;
            }

            FavoritesContainer.RemoveChild(emoteButton);
        }
    }

    private EmotionButton CreateEmoteButton(EmotePrototype emote)
    {
        var button = new EmotionButton(emote, _favoriteEmotes)
        {
            StyleClasses = {"chatFilterOptionButton"},
            HorizontalExpand = true,
            VerticalExpand = true,
            MaxHeight = 45,
            MinWidth = 100
        };

        button.OnPressed += _ => UseEmote(_random.Pick(emote.ChatMessages));
        button.Text = emote.ChatMessages.First();
        return button;
    }

    private void FillFavoritesContainer()
    {
        var favoriteEmotes = _favoriteEmotes.Emotes;

        foreach (var emote in favoriteEmotes)
        {
            var button = CreateEmoteButton(emote);
            FavoritesContainer.AddChild(button);
        }
    }

    private void UseEmote(string emotion)
    {
        var timeSpan = DateTime.Now - _lastEmotionTimeUse;
        var seconds = timeSpan.TotalSeconds;

        if (seconds < _emotionCooldown)
        {
            return;
        }

        _lastEmotionTimeUse = DateTime.Now;
        _chatManager.SendMessage(emotion, ChatSelectChannel.Emotes);
    }
}
