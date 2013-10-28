using System;

namespace Server.Items
{
    public abstract class BaseRing : BaseJewel
    {
        public BaseRing(int itemID)
            : base(itemID, Layer.Ring)
        {
        }

        public BaseRing(Serial serial)
            : base(serial)
        {
        }

        public override int BaseGemTypeNumber
        {
            get
            {
                return 1044176;
            }
        }// star sapphire ring
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

    public class GoldRing : BaseRing
    {
        [Constructable]
        public GoldRing()
            : base(0x108a)
        {
            this.Weight = 0.1;
        }

        public GoldRing(Serial serial)
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

    public class SilverRing : BaseRing
    {
        [Constructable]
        public SilverRing()
            : base(0x1F09)
        {
            this.Weight = 0.1;
        }

        public SilverRing(Serial serial)
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