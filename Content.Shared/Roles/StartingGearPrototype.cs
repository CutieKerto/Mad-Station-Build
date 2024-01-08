using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Robust.Shared.Prototypes;

namespace Content.Shared.Roles
{
    [Prototype("startingGear")]
    public sealed partial class StartingGearPrototype : IPrototype
    {
        [DataField]
        public Dictionary<string, EntProtoId> Equipment = new();

        /// <summary>
        /// if empty, there is no skirt override - instead the uniform provided in equipment is added.
        /// </summary>
        [DataField]
        public EntProtoId? InnerClothingSkirt;

        //MadStation start
        [DataField("femaleTop")]
        public EntProtoId? FemaleDefaultTop = "ClothingUnderwearTopBraAlt";

        [DataField("femaleBottom")]
        public EntProtoId? DefaultFemaleBottom = "ClothingUnderwearPanties";

        [DataField("maleBottom")]
        public EntProtoId? DefaultMaleBottom = "ClothingUnderwearBoxers";
        //MadStation end

        [DataField]
        public EntProtoId? Satchel;

        [DataField]
        public EntProtoId? Duffelbag;

        [DataField]
        public List<EntProtoId> Inhand = new(0);

        [ViewVariables]
        [IdDataField]
        public string ID { get; private set; } = string.Empty;

        public string GetGear(string slot, HumanoidCharacterProfile? profile)
        {
            if (profile != null)
            {
                if (slot == "jumpsuit" && profile.Clothing == ClothingPreference.Jumpskirt && !string.IsNullOrEmpty(InnerClothingSkirt))
                    return InnerClothingSkirt;
                if (slot == "back" && profile.Backpack == BackpackPreference.Satchel && !string.IsNullOrEmpty(Satchel))
                    return Satchel;
                if (slot == "back" && profile.Backpack == BackpackPreference.Duffelbag && !string.IsNullOrEmpty(Duffelbag))
                    return Duffelbag;

                //MadStation start
                if (slot == "underwearTop" && profile.Sex == Sex.Female && !string.IsNullOrEmpty(FemaleDefaultTop))
                {
                    return FemaleDefaultTop;
                }

                if (slot == "underwearBottom")
                {
                    if (profile.Sex == Sex.Female && !string.IsNullOrEmpty(DefaultFemaleBottom))
                    {
                        return DefaultFemaleBottom;
                    }
                    else if (!string.IsNullOrEmpty(DefaultMaleBottom))
                    {
                        return DefaultMaleBottom;
                    }
                }
                //MadStation end
            }

            return Equipment.TryGetValue(slot, out var equipment) ? equipment : string.Empty;
        }
    }
}
