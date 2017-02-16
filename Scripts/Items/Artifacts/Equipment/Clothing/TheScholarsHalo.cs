using System;

namespace Server.Items
{
    public class TheScholarsHalo : Bandana, ITokunoDyable
	{
        public override int LabelNumber { get { return 1157354; } } // the scholar's halo
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TheScholarsHalo()
        {
            this.Attributes.BonusMana = 15;
            this.Attributes.RegenMana = 2;
            this.Attributes.SpellDamage = 15;
            this.Attributes.CastSpeed = 1;
            this.Attributes.LowerManaCost = 10;
        }

        public TheScholarsHalo(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}