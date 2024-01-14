using Content.Shared.StatusIcon;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.SSDIndicator;

/// <summary>
///     Shows status icon when player in SSD
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class SSDIndicatorComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public bool IsSSD = false;

    public bool IsTeleporting = false;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("icon", customTypeSerializer: typeof(PrototypeIdSerializer<StatusIconPrototype>))]
    public string Icon = "SSDIcon";

    public DateTime SsdStartTime = DateTime.MaxValue;

    public static SoundSpecifier DepartureSound = new SoundPathSpecifier("/Audio/Effects/teleport_departure.ogg");

    public static List<EntProtoId> Portals = new() {"PortalSSDRed", "PortalSSDBlue"};
}
