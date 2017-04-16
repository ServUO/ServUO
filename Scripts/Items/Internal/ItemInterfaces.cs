using System;

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
        int WestID { get; }
        int NorthID { get; }
        void OnFlip();
    }

    public interface IQuality
    {
        ItemQuality Quality { get; set; }
    }

    public interface IResource : IQuality
    {
        CraftResource Resource { get; set; }
    }
}