namespace Server.Items
{
    public class BegLiquorPitcher : Pitcher
    {
        [Constructable]
        public BegLiquorPitcher() : base(BeverageType.Liquor)
        {
            ItemID = Utility.RandomDouble() > .5 ? 8089 : 8090;
        }

        public BegLiquorPitcher(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1075129); // Acquired by begging
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}