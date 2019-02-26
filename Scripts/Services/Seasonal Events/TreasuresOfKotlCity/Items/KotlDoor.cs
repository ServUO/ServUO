using System;
using Server;
using Server.Network;
using Server.Items;

namespace Server.Engines.TreasuresOfKotlCity
{
    public class KotlDoor : MetalDoor
    {
        public static KotlDoor Instance { get; set; }

        public KotlDoor()
            : base(DoorFacing.EastCW)
        {
            Hue = 2591;
            Locked = true;

            KeyValue = Key.RandomValue();

            if(Instance == null)
                Instance = this;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else if (Locked)
            {
                from.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1157017, from.NetState); // *The door is sealed shut. There is likely a switch that controls it nearby.*
            }
            else
            {
                Use(from);
            }
        }

        public KotlDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if(Instance == null)
                Instance = this;
        }
    }
}