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
}