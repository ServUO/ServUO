using System;

namespace Server.Items
{
    public class DailyRaresSpawner
    {
        public static readonly bool Enabled = Config.Get("DailyRares.Enabled", true);

        public static void Initialize()
        {
            if (!Enabled)
            {
                return;
            }

            // rocks
            Map map = Map.Felucca;
            Point3D p = new Point3D(new Point3D(2684, 2060, 28));
            if (map.FindItem<DailyRocks>(p) == null)
            {
                SetItem(new DailyRocks(), p, map);
            }

            map = Map.Trammel;
            p = new Point3D(new Point3D(2684, 2060, 28));
            if (map.FindItem<DailyRocks>(p) == null)
            {
                SetItem(new DailyRocks(), p, map);
            }

            // rock
            map = Map.Felucca;
            p = new Point3D(5511, 3116, -4);
            if (map.FindItem<DailyRock>(p) == null)
            {
                SetItem(new DailyRock(), p, map);
            }

            map = Map.Trammel;
            p = new Point3D(5511, 3116, -4);
            if (map.FindItem<DailyRock>(p) == null)
            {
                SetItem(new DailyRock(), p, map);
            }

            // fruit basket (eat)
            map = Map.Felucca;
            p = new Point3D(2636, 2081, 16);
            if (map.FindItem<FruitBasket>(p) == null)
            {
                SetItem(new FruitBasket(true), p, map);
            }

            map = Map.Trammel;
            p = new Point3D(2636, 2081, 16);
            if (map.FindItem<FruitBasket>(p) == null)
            {
                SetItem(new FruitBasket(true), p, map);
            }

            // fruit basket
            map = Map.Felucca;
            p = new Point3D(286, 986, 6);
            if (map.FindItem<FruitBasket>(p) == null)
            {
                SetItem(new FruitBasket(), p, map);
            }

            map = Map.Trammel;
            p = new Point3D(286, 986, 6);
            if (map.FindItem<FruitBasket>(p) == null)
            {
                SetItem(new FruitBasket(), p, map);
            }

            // closed barrel 1
            map = Map.Felucca;
            p = new Point3D(5191, 587, 0);
            if (map.FindItem<ClosedBarrel>(p) == null)
            {
                SetItem(new ClosedBarrel(), p, map);
            }

            map = Map.Trammel;
            p = new Point3D(5191, 587, 0);
            if (map.FindItem<ClosedBarrel>(p) == null)
            {
                SetItem(new ClosedBarrel(), p, map);
            }

            // closed barrel 2
            map = Map.Felucca;
            p = new Point3D(5301, 592, 0);
            if (map.FindItem<ClosedBarrel>(p) == null)
            {
                SetItem(new ClosedBarrel(), p, map);
            }

            map = Map.Trammel;
            p = new Point3D(5301, 592, 0);
            if (map.FindItem<ClosedBarrel>(p) == null)
            {
                SetItem(new ClosedBarrel(), p, map);
            }

            // Large Candle
            map = Map.Felucca;
            p = new Point3D(5575, 1829, 6);
            if (map.FindItem<CandleLarge>(p) == null)
            {
                CandleLarge candle = new CandleLarge
                {
                    Burning = true
                };
                SetItem(candle, p, map);
            }

            map = Map.Trammel;
            p = new Point3D(5575, 1829, 6);
            if (map.FindItem<CandleLarge>(p) == null)
            {
                CandleLarge candle = new CandleLarge
                {
                    Burning = true
                };
                SetItem(candle, p, map);
            }

            // full jars
            map = Map.Felucca;
            p = new Point3D(3656, 2506, 0);
            if (map.FindItem<DailyFullJars>(p) == null)
            {
                SetItem(new DailyFullJars(), p, map);
            }

            // Hay
            p = new Point3D(5998, 3774, 19);
            if (map.FindItem<DecoHay2>(p) == null)
            {
                SetItem(new DecoHay2(), p, map);
            }

            // Broken Chair
            map = Map.Ilshenar;
            p = new Point3D(148, 946, -29);
            if (map.FindItem<DailyBrokenChair>(p) == null)
            {
                SetItem(new DailyBrokenChair(), p, map);
            }

            // Meat Pie
            map = Map.Malas;
            p = new Point3D(2112, 1311, -44);
            if (map.FindItem<DailyMeatPie>(p) == null)
            {
                SetItem(new DailyMeatPie(), p, map);
            }

            // Daily Seaweed
            map = Map.Felucca;
            p = new Point3D(4548, 2400, -5);
            if (map.FindItem<DailySeaweed>(p) == null)
            {
                SetItem(new DailySeaweed(), p, map);
            }

            map = Map.Trammel;
            p = new Point3D(4548, 2400, -5);
            if (map.FindItem<DailySeaweed>(p) == null)
            {
                SetItem(new DailySeaweed(), p, map);
            }
        }

        public static void SetItem(Item item, Point3D p, Map map)
        {
            item.MoveToWorld(p, map);
            item.LastMoved = DateTime.UtcNow + TimeSpan.FromMinutes(int.MaxValue);
        }
    }

    public class DailyRocks : Item
    {
        [Constructable]
        public DailyRocks()
            : base(0x1367)
        {
        }

        public DailyRocks(Serial serial)
            : base(serial)
        {
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

    public class DailyRock : Item
    {
        [Constructable]
        public DailyRock()
            : base(0x1368)
        {
        }

        public DailyRock(Serial serial)
            : base(serial)
        {
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

    public class DailyFullJars : Item
    {
        [Constructable]
        public DailyFullJars()
            : base(0xE48)
        {
        }

        public DailyFullJars(Serial serial)
            : base(serial)
        {
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

    public class DailyBrokenChair : Item
    {
        [Constructable]
        public DailyBrokenChair()
            : base(0xC19)
        {
        }

        public DailyBrokenChair(Serial serial)
            : base(serial)
        {
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

    public class DailySeaweed : Item
    {
        [Constructable]
        public DailySeaweed()
            : base(0xDBA)
        {
        }

        public DailySeaweed(Serial serial)
            : base(serial)
        {
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

    public class DailyMeatPie : Food
    {
        public override int LabelNumber => 1060141;  // a tasty meat pie

        [Constructable]
        public DailyMeatPie()
            : base(0x1041)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 5;
        }

        public DailyMeatPie(Serial serial)
            : base(serial)
        {
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