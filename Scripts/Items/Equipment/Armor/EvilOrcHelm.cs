using Server.Misc;

namespace Server.Items
{
    public class EvilOrcHelm : OrcHelm
    {
        [Constructable]
        public EvilOrcHelm()
            : base()
        {
            Hue = 0x96E;
            Attributes.BonusStr = 10;
            Attributes.BonusInt = IntOrDexPropertyValue;
            Attributes.BonusDex = IntOrDexPropertyValue;
        }

        public EvilOrcHelm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1062021;// an evil orc helm
        public override bool UseIntOrDexProperty
        {
            get
            {
                if (!(Parent is Mobile))
                    return true;

                return base.UseIntOrDexProperty;
            }
        }
        public override int IntOrDexPropertyValue => -10;
        public override bool OnEquip(Mobile from)
        {
            if (from.RawInt > from.RawDex)
                Attributes.BonusDex = 0;
            else
                Attributes.BonusInt = 0;

            Titles.AwardKarma(from, -22, true);

            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
            {
                Attributes.BonusInt = IntOrDexPropertyValue;
                Attributes.BonusDex = IntOrDexPropertyValue;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
