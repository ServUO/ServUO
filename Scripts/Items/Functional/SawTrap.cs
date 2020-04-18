using Server.Network;
using System;

namespace Server.Items
{
    public enum SawTrapType
    {
        WestWall,
        NorthWall,
        WestFloor,
        NorthFloor
    }

    public class SawTrap : BaseTrap
    {
        [Constructable]
        public SawTrap()
            : this(SawTrapType.NorthFloor)
        {
        }

        [Constructable]
        public SawTrap(SawTrapType type)
            : base(GetBaseID(type))
        {
        }

        public SawTrap(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SawTrapType Type
        {
            get
            {
                switch (ItemID)
                {
                    case 0x1103:
                        return SawTrapType.NorthWall;
                    case 0x1116:
                        return SawTrapType.WestWall;
                    case 0x11AC:
                        return SawTrapType.NorthFloor;
                    case 0x11B1:
                        return SawTrapType.WestFloor;
                }

                return SawTrapType.NorthWall;
            }
            set
            {
                ItemID = GetBaseID(value);
            }
        }
        public override bool PassivelyTriggered => false;
        public override TimeSpan PassiveTriggerDelay => TimeSpan.Zero;
        public override int PassiveTriggerRange => 0;
        public override TimeSpan ResetDelay => TimeSpan.FromSeconds(0.0);
        public static int GetBaseID(SawTrapType type)
        {
            switch (type)
            {
                case SawTrapType.NorthWall:
                    return 0x1103;
                case SawTrapType.WestWall:
                    return 0x1116;
                case SawTrapType.NorthFloor:
                    return 0x11AC;
                case SawTrapType.WestFloor:
                    return 0x11B1;
            }

            return 0;
        }

        public override void OnTrigger(Mobile from)
        {
            if (!from.Alive || from.IsStaff())
                return;

            Effects.SendLocationEffect(Location, Map, GetBaseID(Type) + 1, 6, 3, GetEffectHue(), 0);
            Effects.PlaySound(Location, Map, 0x21C);

            Spells.SpellHelper.Damage(TimeSpan.FromTicks(1), from, from, Utility.RandomMinMax(5, 15));

            from.LocalOverheadMessage(MessageType.Regular, 0x22, 500853); // You stepped onto a blade trap!
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
        }
    }
}