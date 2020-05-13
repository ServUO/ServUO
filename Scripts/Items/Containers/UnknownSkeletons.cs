namespace Server.Items
{
    public class UnknownBardSkeleton : BaseContainer
    {
        public override int DefaultGumpID => 0x9;

        [Constructable]
        public UnknownBardSkeleton()
            : base(0xECA + Utility.Random(9))
        {
            Name = "An Unknown Bard's Skeleton";
            Weight = 35.0;

            DropItem(new Gold(Utility.RandomMinMax(200, 400)));
            DropItem(new Doublet(Utility.RandomNondyedHue()));
            DropItem(new JesterHat(Utility.RandomNondyedHue()));
            DropItem(new Bandage(Utility.RandomMinMax(10, 20)));

            switch (Utility.Random(2))
            {
                case 0: DropItem(new Kilt(Utility.RandomNondyedHue())); break;
                case 1: DropItem(new ShortPants(Utility.RandomNondyedHue())); break;
            }

            switch (Utility.Random(3))
            {
                case 0: DropItem(new BeverageBottle(BeverageType.Ale)); break;
                case 1: DropItem(new BeverageBottle(BeverageType.Wine)); break;
                case 2: DropItem(new BeverageBottle(BeverageType.Liquor)); break;
            }

            DropItem(Loot.RandomInstrument());
        }

        public UnknownBardSkeleton(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class UnknownRogueSkeleton : BaseContainer
    {
        public override int DefaultGumpID => 0x9;

        [Constructable]
        public UnknownRogueSkeleton()
            : base(0xECA + Utility.Random(9))
        {
            Name = "An Unknown Rogue's Skeleton";
            Weight = 35.0;

            DropItem(new LeatherChest());
            DropItem(new LeatherGloves());
            DropItem(new LeatherArms());
            DropItem(new Dagger());
            DropItem(new Shovel(50));
            DropItem(new Lockpick(Utility.RandomMinMax(1, 4)));

            if (Utility.RandomBool())
                DropItem(new Torch());
            else
                DropItem(new Lantern());

            if (0.1 >= Utility.RandomDouble())
                DropItem(Loot.RandomRangedWeapon());
            else
                DropItem(Loot.RandomWeapon());

            DropItem(new TreasureMap(Utility.RandomMinMax(3, 5), Map.Felucca));
        }

        public UnknownRogueSkeleton(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class UnknownMageSkeleton : BaseContainer
    {
        public override int DefaultGumpID => 0x9;

        [Constructable]
        public UnknownMageSkeleton()
            : base(0xECA + Utility.Random(9))
        {
            Name = "An Unknown Mage's Skeleton";
            Weight = 35.0;

            DropItem(new Robe(Utility.RandomNondyedHue()));
            DropItem(new Sandals());
            DropItem(Loot.RandomJewelry());

            if (Utility.RandomBool())
                DropItem(new QuarterStaff());
            else
                DropItem(new GnarledStaff());

            Item item;

            for (int i = 0; i < 3; i++)
            {
                item = Loot.RandomReagent();
                item.Amount = Utility.RandomMinMax(15, 20);
                DropItem(item);
            }

            for (int i = 0; i < 3; i++)
            {
                if (0.25 >= Utility.RandomDouble())
                    item = Loot.Construct(Loot.NecromancyScrollTypes);
                else
                    item = Loot.Construct(Loot.MageryScrollTypes);

                item.Amount = Utility.RandomMinMax(1, 2);
                DropItem(item);
            }
        }

        public UnknownMageSkeleton(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}
