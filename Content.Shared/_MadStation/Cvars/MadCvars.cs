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

    /// <summary>
    /// CVar for enabling/disabling SSD teleportation.
    /// </summary>
    public static readonly CVarDef<bool> SsdTeleportationEnabled =
        CVarDef.Create("mad.ssd.teleportationEnabled", true, CVar.SERVERONLY | CVar.ARCHIVE);

    /// <summary>
    /// CVar for setting the maximum SSD time in minutes.
    /// </summary>
    public static readonly CVarDef<float> MaxSsdTime =
        CVarDef.Create("mad.ssd.maxSsdTime",20f, CVar.SERVERONLY | CVar.ARCHIVE);
}
