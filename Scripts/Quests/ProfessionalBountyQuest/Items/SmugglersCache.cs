namespace Server.Items
{
    public class SmugglersCache : Container
    {
        public override int LabelNumber => 1150042;

        public SmugglersCache()
            : base(Utility.RandomList(3644, 3645, 3646, 3647, 3648, 3649, 3650, 3651))
        {
            int toDrop = Utility.RandomMinMax(15, 20);
            for (int i = 0; i < toDrop; i++)
            {
                DropItem(GetRandomBeverage());
            }

            if (0.5 < Utility.RandomDouble())
                DropItem(SmugglersLiquor.GetRandom());

            DropItem(SmugglersLiquor.GetRandom());
        }

        public BeverageType RandomBeverageType()
        {
            switch (Utility.Random(4))
            {
                case 0: return BeverageType.Ale;
                case 1: return BeverageType.Cider;
                case 2: return BeverageType.Liquor;
                case 3: return BeverageType.Wine;
            }

            return BeverageType.Milk;
        }

        public BaseBeverage GetRandomBeverage()
        {
            switch (Utility.Random(2))
            {
                default:
                case 0: return new BeverageBottle(RandomBeverageType());
                case 1: return new Jug(RandomBeverageType());
            }
        }

        public SmugglersCache(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }
    }
}