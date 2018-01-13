using System;

namespace Server.Items
{
    public class DrSpectorsLenses : Glasses
    {
        public override int LabelNumber { get { return 1156991; } } // Dr. Spector's lenses

        [Constructable]
        public DrSpectorsLenses()
        {
            Attributes.BonusInt = 8;
            Attributes.RegenMana = 4;
            Attributes.SpellDamage = 12;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 10;
        }

        public override int BasePhysicalResistance { get { return 5; } }
        public override int BaseFireResistance { get { return 10; } }
        public override int BaseColdResistance { get { return 14; } }
        public override int BasePoisonResistance { get { return 20; } }
        public override int BaseEnergyResistance { get { return 20; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public DrSpectorsLenses(Serial serial)
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
