namespace Server.Items
{
    public class MelisandesFermentedWine : GreaterExplosionPotion
    {
        [Constructable]
        public MelisandesFermentedWine()
        {
            Stackable = false;
            ItemID = 0x99B;
            Hue = Utility.RandomList(0xB, 0xF, 0x48D); // TODO update
        }

        public MelisandesFermentedWine(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072114;// Melisande's Fermented Wine

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1074502); // It looks explosive.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
