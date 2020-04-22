using Server.Network;
using System;

namespace Server.Items
{
    public enum SpikeTrapType
    {
        WestWall,
        NorthWall,
        WestFloor,
        NorthFloor
    }

    public class SpikeTrap : BaseTrap
    {
        [Constructable]
        public SpikeTrap()
            : this(SpikeTrapType.WestFloor)
        {
        }

        [Constructable]
        public SpikeTrap(SpikeTrapType type)
            : base(GetBaseID(type))
        {
        }

        public SpikeTrap(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SpikeTrapType Type
        {
            get
            {
                switch (ItemID)
                {
                    case 4360:
                    case 4361:
                    case 4366:
                        return SpikeTrapType.WestWall;
                    case 4379:
                    case 4380:
                    case 4385:
                        return SpikeTrapType.NorthWall;
                    case 4506:
                    case 4507:
                    case 4511:
                        return SpikeTrapType.WestFloor;
                    case 4512:
                    case 4513:
                    case 4517:
                        return SpikeTrapType.NorthFloor;
                }

                return SpikeTrapType.WestWall;
            }
            set
            {
                bool extended = Extended;

                ItemID = (extended ? GetExtendedID(value) : GetBaseID(value));
            }
        }
        public bool Extended
        {
            get
            {
                return (ItemID == GetExtendedID(Type));
            }
            set
            {
                if (value)
                    ItemID = GetExtendedID(Type);
                else
                    ItemID = GetBaseID(Type);
            }
        }
        public override bool PassivelyTriggered => false;
        public override TimeSpan PassiveTriggerDelay => TimeSpan.Zero;
        public override int PassiveTriggerRange => 0;
        public override TimeSpan ResetDelay => TimeSpan.FromSeconds(6.0);
        public static int GetBaseID(SpikeTrapType type)
        {
            switch (type)
            {
                case SpikeTrapType.WestWall:
                    return 4360;
                case SpikeTrapType.NorthWall:
                    return 4379;
                case SpikeTrapType.WestFloor:
                    return 4506;
                case SpikeTrapType.NorthFloor:
                    return 4512;
            }

            return 0;
        }

        public static int GetExtendedID(SpikeTrapType type)
        {
            return GetBaseID(type) + GetExtendedOffset(type);
        }

        public static int GetExtendedOffset(SpikeTrapType type)
        {
            switch (type)
            {
                case SpikeTrapType.WestWall:
                    return 6;
                case SpikeTrapType.NorthWall:
                    return 6;

                case SpikeTrapType.WestFloor:
                    return 5;
                case SpikeTrapType.NorthFloor:
                    return 5;
            }

            return 0;
        }

        public override void OnTrigger(Mobile from)
        {
            if (!from.Alive || from.IsStaff())
                return;

            Effects.SendLocationEffect(Location, Map, GetBaseID(Type) + 1, 18, 3, GetEffectHue(), 0);
            Effects.PlaySound(Location, Map, 0x22C);
            IPooledEnumerable eable = GetMobilesInRange(0);

            foreach (Mobile mob in eable)
            {
                if (mob.Alive && !mob.IsDeadBondedPet)
                    Spells.SpellHelper.Damage(TimeSpan.FromTicks(1), mob, mob, Utility.RandomMinMax(1, 6) * 6);
            }
            eable.Free();

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), OnSpikeExtended);

            from.LocalOverheadMessage(MessageType.Regular, 0x22, 500852); // You stepped onto a spike trap!
        }

        public virtual void OnSpikeExtended()
        {
            Extended = true;
            Timer.DelayCall(TimeSpan.FromSeconds(5.0), OnSpikeRetracted);
        }

        public virtual void OnSpikeRetracted()
        {
            Extended = false;
            Effects.SendLocationEffect(Location, Map, GetExtendedID(Type) - 1, 6, 3, GetEffectHue(), 0);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Extended = false;
        }
    }
}
