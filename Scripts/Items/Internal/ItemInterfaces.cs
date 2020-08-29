using Server.Engines.Craft;
using Server.Mobiles;

namespace Server.Items
{
    public interface IUsesRemaining
    {
        int UsesRemaining { get; set; }
        bool ShowUsesRemaining { get; set; }
    }

    public interface IAccountRestricted
    {
        string Account { get; set; }
    }

    public interface IOwnerRestricted
    {
        Mobile Owner { get; set; }
        string OwnerName { get; set; }
    }

    public interface IFlipable
    {
        void OnFlip(Mobile m);
    }

    public interface IQuality : ICraftable
    {
        ItemQuality Quality { get; set; }
        bool PlayerConstructed { get; }
    }

    public interface IResource
    {
        CraftResource Resource { get; set; }
    }

    public interface IConditionalVisibility
    {
        bool CanBeSeenBy(PlayerMobile m);
    }

    public interface IImbuableEquipement
    {
        int TimesImbued { get; set; }
        bool IsImbued { get; set; }

        int[] BaseResists { get; }
        void OnAfterImbued(Mobile m, int mod, int value);
    }

    public interface ICombatEquipment : IImbuableEquipement
    {
        ItemPower ItemPower { get; set; }
        ReforgedPrefix ReforgedPrefix { get; set; }
        ReforgedSuffix ReforgedSuffix { get; set; }
        bool PlayerConstructed { get; set; }
    }

    public enum ItemQuality
    {
        Low,
        Normal,
        Exceptional
    }

    public enum DirectionType
    {
        None = 0,
        South = 1,
        East = 2,
        North = 3,
        West = 4
    }

    public enum ItemSize
    {
        Small,
        Large,
    }
}
