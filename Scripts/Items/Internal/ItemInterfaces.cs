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
        void OnFlip();
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
}