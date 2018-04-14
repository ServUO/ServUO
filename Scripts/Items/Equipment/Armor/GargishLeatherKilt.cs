using Server.Engines.Craft;
using System;

namespace Server.Items
{
    [TypeAlias("Server.Items.MaleGargishLeatherKilt")]
    public class GargishLeatherKilt : BaseArmor
    {
        [Constructable]
        public GargishLeatherKilt()
            : this(0)
        {
        }

        [Constructable]
        public GargishLeatherKilt(int hue)
            : base(0x311)
        {
            Layer = Layer.Gloves;
            Weight = 5.0;
            Hue = hue;
        }

        public GargishLeatherKilt(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance { get { return 5; } }
        public override int BaseFireResistance { get { return 6; } }
        public override int BaseColdResistance { get { return 7; } }
        public override int BasePoisonResistance { get { return 6; } }
        public override int BaseEnergyResistance { get { return 6; } }

        public override int InitMinHits { get { return 30; } }
        public override int InitMaxHits { get { return 50; } }

        public override int AosStrReq { get { return 25; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }
        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Leather; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if(this.ItemID != 0x311)
                this.ItemID = 0x311;
        }
    }
}