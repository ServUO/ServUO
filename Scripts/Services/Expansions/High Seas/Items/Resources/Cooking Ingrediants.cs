using Server.Targeting;

namespace Server.Items
{
    public class MentoSeasoning : Item
    {
        public override int LabelNumber => 1116299; // Mento Seasoning

        [Constructable]
        public MentoSeasoning()
            : base(0x996)
        {
            Hue = 2591;
        }

        public MentoSeasoning(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class SamuelsSecretSauce : Item
    {
        public override int LabelNumber => 1116338; // Samuel's Secret Sauce

        [Constructable]
        public SamuelsSecretSauce()
            : base(0x99B)
        {
            Hue = 1461;
        }

        public SamuelsSecretSauce(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class DarkTruffle : Item
    {
        public override int LabelNumber => 1116300; // dark truffle

        [Constructable]
        public DarkTruffle()
            : base(0xD18)
        {
            Hue = 2021;
        }

        public DarkTruffle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class FreshGinger : Item
    {
        public override int LabelNumber => 1031235; // Fresh Ginger

        [Constructable]
        public FreshGinger()
            : base(0x2BE3)
        {
        }

        public FreshGinger(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class FishOilFlask : Item
    {
        public override int LabelNumber => 1150863; // fish oil flask

        [Constructable]
        public FishOilFlask()
            : base(0x1C18)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
                from.Target = new InternalTarget(this);
        }

        private class InternalTarget : Target
        {
            private readonly FishOilFlask m_Flask;

            public InternalTarget(FishOilFlask flask) : base(-1, false, TargetFlags.None)
            {
                m_Flask = flask;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is OracleOfTheSea)
                {
                    if (((OracleOfTheSea)targeted).UsesRemaining >= 5)
                        from.SendMessage("That is already fully charged!");
                    else
                    {
                        ((OracleOfTheSea)targeted).UsesRemaining = 5;
                        from.SendMessage("You charge the oracle with the fish oil.");
                        m_Flask.Delete();
                    }
                }
            }
        }

        public FishOilFlask(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class Salt : Item
    {
        public override int LabelNumber => 1159201; // salt

        [Constructable]
        public Salt()
            : base(0x4C09)
        {
            Hue = 1150;
        }

        public Salt(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class FreshSeasoning : Item
    {
        public override int LabelNumber => 1159200; // fresh seasoning

        [Constructable]
        public FreshSeasoning()
            : base(0x1006)
        {
            Hue = 1150;
        }

        public FreshSeasoning(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
