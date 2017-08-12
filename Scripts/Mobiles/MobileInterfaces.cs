using System;

namespace Server.Mobiles
{
    public interface IFreezable
    {
        void OnRequestedAnimation(Mobile from);
    }

    public interface IRepairableMobile : IEntity
    {
        Type RepairResource { get; }
        int Hits { get; set; }
        int HitsMax { get; }
    }
}