using System.Text;
using Content.Shared._MadStation.Cvars;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;

namespace Content.Client._MadStation.EmotionPanel;

public class FavoriteEmotesChangedEventArgs : EventArgs
{
    public EmotePrototype? Added { get; }
    public EmotePrototype? Removed { get; }

    public FavoriteEmotesChangedEventArgs(EmotePrototype? added, EmotePrototype? removed)
    {
        Added = added;
        Removed = removed;
    }
}
public sealed class FavoriteEmotesManager
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private const string CacheTemplate = @"{0}|";
    private const string Separator = "|";

    public IReadOnlyList<EmotePrototype> Emotes => _emotes;

    public EventHandler<FavoriteEmotesChangedEventArgs>? OnFavoritesUpdate;

    private readonly List<EmotePrototype> _emotes = new();

    public void Initialize()
    {
        RemoveInvalidEmotions();
        Cache();
    }

    public void AddToFavorite(EmotePrototype emote)
    {
        var emotes = _cfg.GetCVar(MadCvars.FavoriteEmotes);
        if (emotes.Contains(emote.ID))
        {
            return;
        }

        emotes += string.Format(CacheTemplate, emote.ID);

        _cfg.SetCVar(MadCvars.FavoriteEmotes, emotes);
        _cfg.SaveToFile();
        _emotes.Add(emote);
        OnFavoritesUpdate?.Invoke(this, new FavoriteEmotesChangedEventArgs(emote, null));
    }

    public void RemoveFromFavorite(EmotePrototype emote)
    {
        var emotes = _cfg.GetCVar(MadCvars.FavoriteEmotes);
        emotes = emotes.Replace(string.Format(CacheTemplate, emote.ID), string.Empty);

        _cfg.SetCVar(MadCvars.FavoriteEmotes, emotes);
        _cfg.SaveToFile();
        _emotes.Remove(emote);
        OnFavoritesUpdate?.Invoke(this, new FavoriteEmotesChangedEventArgs(null, emote));
    }

    public bool IsFavorite(EmotePrototype prototype)
    {
        return _emotes.Contains(prototype);
    }

    private void RemoveInvalidEmotions()
    {
        var emotesIds = _cfg.GetCVar(MadCvars.FavoriteEmotes);

        if (string.IsNullOrEmpty(emotesIds))
        {
            return;
        }

        var splitEmotesIds = emotesIds.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
        var emotesToRemove = new HashSet<string>();

        foreach (var emoteId in splitEmotesIds)
        {
            if (!_prototypeManager.TryIndex<EmotePrototype>(emoteId, out _))
            {
                emotesToRemove.Add(emoteId);
            }
        }

        if (emotesToRemove.Count == 0)
        {
            return;
        }

        var builder = new StringBuilder(emotesIds);

        foreach (var emoteId in emotesToRemove)
        {
            builder.Replace(string.Format(CacheTemplate, emoteId), string.Empty);
        }

        _cfg.SetCVar(MadCvars.FavoriteEmotes, emotesIds);
        _cfg.SaveToFile();
    }

    private void Cache()
    {
        var emotesIds = _cfg.GetCVar(MadCvars.FavoriteEmotes);

        if (string.IsNullOrEmpty(emotesIds))
        {
            return;
        }

        var emotesIdsSplit = emotesIds.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

        foreach (var emoteId in emotesIdsSplit)
        {
            var emotePrototype = _prototypeManager.Index<EmotePrototype>(emoteId);
            _emotes.Add(emotePrototype);
        }
    }

}
