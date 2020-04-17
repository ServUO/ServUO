namespace Server.Items
{
    public class BlueDiamondRing : GoldRing
    {
        [Constructable]
        public BlueDiamondRing()
            : base()
        {
            Weight = 1.0;

            BaseRunicTool.ApplyAttributesTo(this, true, 0, Utility.RandomMinMax(2, 4), 0, 100);

            switch (Utility.Random(4))
            {
                case 0:
                    Attributes.LowerManaCost += 10;
                    break;
                case 1:
                    Attributes.CastSpeed += 1;
                    break;
                case 2:
                    Attributes.CastRecovery += 2;
                    break;
                case 3:
                    Attributes.SpellDamage += 5;
                    break;
            }
        }

        public BlueDiamondRing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073458;// blue diamond ring
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