using System;
using Server.Mobiles;
using Server.Engines.Craft;

namespace Server.Items
{
    public interface IAccountRestricted
    {
        string Account { get; set; }
    }

    public interface IVvVItem
    {
        bool IsVvVItem { get; set; }
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

    public interface IResource : IQuality
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
        Exceptional,
    }

    public enum DirectionType
    {
        None = 0,
        South = 1,
        East = 2
    }
}
