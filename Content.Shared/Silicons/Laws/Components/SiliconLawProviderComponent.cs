﻿using Robust.Shared.Prototypes;
using Content.Shared.Random;

namespace Content.Shared.Silicons.Laws.Components;

/// <summary>
/// This is used for an entity which grants laws to a <see cref="SiliconLawBoundComponent"/>
/// </summary>
[RegisterComponent, Access(typeof(SharedSiliconLawSystem))]
public sealed partial class SiliconLawProviderComponent : Component
{
    /// <summary>
    /// The id of the lawset that is being provided.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<SiliconLawsetPrototype> Laws = string.Empty;

	/// <summary>
    /// Weighted random override for lawset provided.
    /// </summary>
    [DataField(required: false)]
    public ProtoId<WeightedRandomPrototype>? LawsWeighted;

    /// <summary>
    /// Lawset created from the prototype id.
    /// Cached when getting laws and only modified during an ion storm event.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public SiliconLawset? Lawset;

}
