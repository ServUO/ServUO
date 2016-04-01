using System;

namespace Server.Items
{
    public class SerpentFangSectBadge : Item
    {
        [Constructable]
        public SerpentFangSectBadge()
            : base(0x23C)
        {
            this.LootType = LootType.Blessed;
        }

        public SerpentFangSectBadge(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073139;
            }
        }// A Serpent Fang Sect Badge
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