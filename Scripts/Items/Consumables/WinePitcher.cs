namespace Server.Items
{
    public class BegWinePitcher : Pitcher
    {
        [Constructable]
        public BegWinePitcher() : base(BeverageType.Wine)
        {
            ItemID = Utility.RandomDouble() > .5 ? 8091 : 8092;
        }

        public BegWinePitcher(Serial serial)
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