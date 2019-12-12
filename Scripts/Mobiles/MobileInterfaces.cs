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

    public interface IElementalCreature
    {
        ElementType ElementType { get; }
    }

    public interface IAuraCreature
    {
        void AuraEffect(Mobile victim);
    }

    public enum ElementType
    {
        Physical,
        Fire,
        Cold,
        Poison,
        Energy,
        Chaos,
        Direct
    }
}
