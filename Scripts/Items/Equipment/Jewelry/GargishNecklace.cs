using System;

namespace Server.Items
{
    public class GargishNecklace : BaseArmor
    {
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Chainmail; } }
        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

        public override int BasePhysicalResistance { get { return 1; } }
        public override int BaseFireResistance { get { return 2; } }
        public override int BaseColdResistance { get { return 2; } }
        public override int BasePoisonResistance { get { return 2; } }
        public override int BaseEnergyResistance { get { return 3; } }

        public override int InitMinHits { get { return 30; } }
        public override int InitMaxHits { get { return 40; } }

        [Constructable]
        public GargishNecklace()
            : base(0x4210)
        {
            Layer = Layer.Neck;
        }

        public GargishNecklace(int itemID)
            : base(itemID)
        {
        }

        public GargishNecklace(Serial serial)
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

    public class GargishAmulet : GargishNecklace
    {
        [Constructable]
        public GargishAmulet()
            : base(0x4D0B)
        {
        }

        public GargishAmulet(Serial serial)
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

    public class GargishStoneAmulet : GargishNecklace
    {
        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }
        public override int AosStrReq { get { return 40; } }
        public override int OldStrReq { get { return 20; } }

        [Constructable]
        public GargishStoneAmulet()
            : base(0x4D0A)
        {
            this.Hue = 2500;
        }

        public GargishStoneAmulet(Serial serial)
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