using System;
using Server;
using Server.Network;
using Server.Mobiles;

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
        [CommandProperty(AccessLevel.GameMaster)]
        public SawTrapType Type
        {
            get
            {
                switch (ItemID)
                {
                    case 0x1103: return SawTrapType.NorthWall;
                    case 0x1116: return SawTrapType.WestWall;
                    case 0x11AC: return SawTrapType.NorthFloor;
                    case 0x11B1: return SawTrapType.WestFloor;
                }

                return SawTrapType.NorthWall;
            }
            set
            {
                ItemID = GetBaseID(value);
            }
        }

        public static int GetBaseID(SawTrapType type)
        {
            switch (type)
            {
                case SawTrapType.NorthWall: return 0x1103;
                case SawTrapType.WestWall: return 0x1116;
                case SawTrapType.NorthFloor: return 0x11AC;
                case SawTrapType.WestFloor: return 0x11B1;
            }

            return 0;
        }

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

        public override bool PassivelyTriggered { get { return false; } }
        public override TimeSpan PassiveTriggerDelay { get { return TimeSpan.Zero; } }
        public override int PassiveTriggerRange { get { return 0; } }
        public override TimeSpan ResetDelay { get { return TimeSpan.FromSeconds(0.0); } }
        public override int MessageHue { get { return 0x5A; } }

        public override void OnTrigger(Mobile from)
        {
            if (!from.Alive || from.AccessLevel > AccessLevel.Player ||
                from is BaseCreature && !((BaseCreature)from).Controlled)
                return;
            //if ( !from.Alive || from.AccessLevel > AccessLevel.Player )
            //	return;

            Effects.SendLocationEffect(Location, Map, GetBaseID(this.Type) + 1, 6, 3, GetEffectHue(), 0);
            Effects.PlaySound(Location, Map, 0x21C);

            Spells.SpellHelper.Damage(TimeSpan.FromTicks(1), from, from, Utility.RandomMinMax(5, 15));

            from.LocalOverheadMessage(MessageType.Regular, 0x22, 500853); // You stepped onto a blade trap!
        }

        public SawTrap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}