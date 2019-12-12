using System;

namespace Server.Items
{
    [Furniture]
    [Flipable(0x9AA, 0xE7D)]
    public class BoxOfCrystalItems : BaseContainer
    {
        public override int LabelNumber { get { return 1076712; } } // A Box of Crystal Items

        public override int DefaultGumpID { get { return 0x43; } }

        [Constructable]
        public BoxOfCrystalItems()
            : base(0x9AA)
        {
            Weight = 4.0;
            Hue = 1173;

            DropItem(new CrystalAltarDeed());
            DropItem(new CrystalBeggarStatueDeed());
            DropItem(new CrystalBrazierDeed());
            DropItem(new CrystalBullStatueDeed());
            DropItem(new CrystalRunnerStatueDeed());
            DropItem(new CrystalSupplicantStatueDeed());
            DropItem(new CrystalTableDeed());
            DropItem(new CrystalThroneDeed());
        }

        public BoxOfCrystalItems(Serial serial)
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
