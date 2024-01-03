using Robust.Shared.Configuration;

namespace Content.Shared._MadStation.Cvars;

[CVarDefs]
public sealed class MadCvars
{
    /// <summary>
    /// Cooldown duration in seconds for using emotes.
    /// </summary>
    public static readonly CVarDef<float> EmoteCooldown =
        CVarDef.Create("mad.emote.cooldown", 2f, CVar.SERVER | CVar.ARCHIVE | CVar.REPLICATED);

    /// <summary>
    /// CVar for storing a player's favorite emotes locally 😏.
    /// </summary>
    public static readonly CVarDef<string> FavoriteEmotes =
        CVarDef.Create("mad.emote.favorites", "", CVar.CLIENTONLY | CVar.ARCHIVE);
}
