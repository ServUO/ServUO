using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.VvV
{
    public class VvVGargishStoneChest : GargishStoneChest
    {
        public override int BasePhysicalResistance { get { return 17; } }
        public override int BaseFireResistance { get { return 19; } }
        public override int BaseColdResistance { get { return 18; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 6; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public VvVGargishStoneChest()
        {
            IsVvVItem = true;

            AbsorptionAttributes.EaterEnergy = 15;
            Attributes.BonusStr = 3;
            Attributes.BonusStam = 10;
            Attributes.RegenStam = 3;
        }

        public VvVGargishStoneChest(Serial serial)
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

    public class VvVStuddedChest : StuddedChest
    {
        public override int BasePhysicalResistance { get { return 17; } }
        public override int BaseFireResistance { get { return 19; } }
        public override int BaseColdResistance { get { return 18; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 6; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public VvVStuddedChest()
        {
            IsVvVItem = true;

            AbsorptionAttributes.EaterEnergy = 15;
            Attributes.BonusStr = 3;
            Attributes.BonusStam = 10;
            Attributes.RegenStam = 3;
        }

        public VvVStuddedChest(Serial serial)
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