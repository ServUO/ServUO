using System;

namespace Server.Items
{
    public class TigerClawSectBadge : Item
    {
        [Constructable]
        public TigerClawSectBadge()
            : base(0x23D)
        {
            this.LootType = LootType.Blessed;
        }

        public TigerClawSectBadge(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073140;
            }
        }// A Tiger Claw Sect Badge
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