using System;
using Server.Mobiles;

namespace Server.Items
{
    public class LuckyCharm : BaseTalisman, ITokunoDyable
    {
        [Constructable]
        public LuckyCharm()
            : base(0x2F5B)
        {
            this.Hue = 1923;
            this.Attributes.RegenHits = 1;
            this.Attributes.RegenStam = 1;
            this.Attributes.RegenMana = 1;
            this.Attributes.Luck = 150;
        }

        public LuckyCharm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1154725;
            }
        }// Lucky Charm
        public override bool ForceShowName
        {
            get
            {
                return true;
            }
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