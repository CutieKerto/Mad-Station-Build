﻿using Content.Client._MadStation.EmotionPanel;
using Content.Shared.Chat;
using Content.Shared.Input;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Systems.Chat.Controls;

[Virtual]
public class ChatInputBox : PanelContainer
{
    public readonly ChannelSelectorButton ChannelSelector;
    public readonly HistoryLineEdit Input;
    public readonly ChannelFilterButton FilterButton;
    protected readonly BoxContainer Container;
    protected ChatChannel ActiveChannel { get; private set; } = ChatChannel.Local;

    public ChatInputBox()
    {
        Container = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Horizontal,
            SeparationOverride = 4
        };
        AddChild(Container);

        ChannelSelector = new ChannelSelectorButton
        {
            Name = "ChannelSelector",
            ToggleMode = true,
            StyleClasses = {"chatSelectorOptionButton"},
            MinWidth = 75
        };
        Container.AddChild(ChannelSelector);
        Input = new HistoryLineEdit
        {
            Name = "Input",
            PlaceHolder = Loc.GetString("hud-chatbox-info", ("talk-key", BoundKeyHelper.ShortKeyName(ContentKeyFunctions.FocusChat)), ("cycle-key", BoundKeyHelper.ShortKeyName(ContentKeyFunctions.CycleChatChannelForward))),
            HorizontalExpand = true,
            StyleClasses = {"chatLineEdit"}
        };
        Container.AddChild(Input);

        //MadStation edit start
        var emoteButton = new EmotionPanelToggleButton()
        {
            HorizontalAlignment = HAlignment.Center,
            VerticalAlignment = VAlignment.Center,
            StyleClasses = { "chatFilterOptionButton" }
        };
        Container.AddChild(emoteButton);
        //MadStation edit end

        FilterButton = new ChannelFilterButton
        {
            Name = "FilterButton",
            StyleClasses = {"chatFilterOptionButton"}
        };
        Container.AddChild(FilterButton);
        ChannelSelector.OnChannelSelect += UpdateActiveChannel;
    }

    private void UpdateActiveChannel(ChatSelectChannel selectedChannel)
    {
        ActiveChannel = (ChatChannel) selectedChannel;
    }
}
