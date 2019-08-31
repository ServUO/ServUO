using System;

namespace Server.Items
{
    public class ElegantCollarOfFortune : ElegantCollar
    {
        public override int LabelNumber { get { return 1159225; } } // elegant collar of fortune

        [Constructable]
        public ElegantCollarOfFortune()
            : base()
        {
            Attributes.Luck = 300;
            Attributes.RegenMana = 1;
        }

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 10; } }
        public override int BaseColdResistance { get { return 10; } }
        public override int BasePoisonResistance { get { return 10; } }
        public override int BaseEnergyResistance { get { return 15; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public ElegantCollarOfFortune(Serial serial)
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
