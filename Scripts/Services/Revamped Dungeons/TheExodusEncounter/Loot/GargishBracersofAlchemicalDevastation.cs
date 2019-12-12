using System;

namespace Server.Items
{
    public class GargishBracersofAlchemicalDevastation : GargishLeatherArms
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishBracersofAlchemicalDevastation()
        {
            Attributes.RegenMana = 4;
            Attributes.CastRecovery = 3;
            ArmorAttributes.MageArmor = 1;
            WeaponAttributes.HitFireball = 15;
        }

        public GargishBracersofAlchemicalDevastation(Serial serial) : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1153523;
            }
        }//Bracers of Alchemical Devastation [Replica]

        public override int BasePhysicalResistance { get { return 10; } }
        public override int BaseFireResistance { get { return 8; } }
        public override int BaseColdResistance { get { return 8; } }
        public override int BasePoisonResistance { get { return 8; } }
        public override int BaseEnergyResistance { get { return 8; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override bool CanFortify { get { return false; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                xWeaponAttributesDeserializeHelper(reader, this);
            }
        }
    }
}
