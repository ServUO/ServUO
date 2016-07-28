using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class PrismaticLenses : Glasses
    {
        public override int LabelNumber { get { return 1152716; } } // Prismatic Lenses
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int BasePhysicalResistance { get { return 18; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 7; } }
        public override int BasePoisonResistance { get { return 17; } }
        public override int BaseEnergyResistance { get { return 6; } }

        [Constructable]
        public PrismaticLenses()
        {
            Hue = 2068;
            WeaponAttributes.HitLowerDefend = 30;
            Attributes.RegenHits = 2;
            Attributes.RegenStam = 3;
            Attributes.WeaponDamage = 25;
        }

        public PrismaticLenses(Serial serial)
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
            int version = reader.ReadInt();
        }
    }

    public class GargishPrismaticLenses : GargishGlasses
    {
        public override int LabelNumber { get { return 1152716; } } // Prismatic Lenses

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int BasePhysicalResistance { get { return 18; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 7; } }
        public override int BasePoisonResistance { get { return 17; } }
        public override int BaseEnergyResistance { get { return 6; } }

        [Constructable]
        public GargishPrismaticLenses()
        {
            Hue = 2068;
            WeaponAttributes.HitLowerDefend = 30;
            Attributes.RegenHits = 2;
            Attributes.RegenStam = 3;
            Attributes.WeaponDamage = 25;
        }

        public GargishPrismaticLenses(Serial serial)
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
            int version = reader.ReadInt();
        }
    }
}