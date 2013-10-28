using System;
using Server.Misc;

namespace Server.Items
{
    public class EvilOrcHelm : OrcHelm
    {
        [Constructable]
        public EvilOrcHelm()
            : base()
        {
            this.Hue = 0x96E;
            this.Attributes.BonusStr = 10;
            this.Attributes.BonusInt = this.IntOrDexPropertyValue;
            this.Attributes.BonusDex = this.IntOrDexPropertyValue;
        }

        public EvilOrcHelm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1062021;
            }
        }// an evil orc helm
        public override bool UseIntOrDexProperty
        {
            get
            {
                if (Core.SA && !(this.Parent is Mobile))
                    return true;

                return base.UseIntOrDexProperty;
            }
        }
        public override int IntOrDexPropertyValue
        {
            get
            {
                return -10;
            }
        }
        public override bool OnEquip(Mobile from)
        {
            if (from.RawInt > from.RawDex)
                this.Attributes.BonusDex = 0;
            else
                this.Attributes.BonusInt = 0;
			
            Titles.AwardKarma(from, -22, true);

            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
            {
                this.Attributes.BonusInt = this.IntOrDexPropertyValue;
                this.Attributes.BonusDex = this.IntOrDexPropertyValue;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}