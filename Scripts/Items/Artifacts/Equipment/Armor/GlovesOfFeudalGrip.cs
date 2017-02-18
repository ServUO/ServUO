using System;

namespace Server.Items
{
    public class GlovesOfFeudalGrip : DragonGloves
    {
        public override int LabelNumber { get { return 1157349; } } // gloves of feudal grip
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GlovesOfFeudalGrip()
        {
            this.Attributes.BonusStr = 8;
            this.Attributes.BonusStam = 8;
            this.Attributes.RegenHits = 3;
            this.Attributes.RegenMana = 3;
            this.Attributes.WeaponDamage = 30;
        }

        public GlovesOfFeudalGrip(Serial serial)
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