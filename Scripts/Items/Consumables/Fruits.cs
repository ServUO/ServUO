namespace Server.Items
{
    public class FruitBasket : Food
    {
        private bool _DailyRare;

        public bool DailyRare
        {
            get { return _DailyRare; }
            set
            {
                _DailyRare = value;

                if (_DailyRare)
                {
                    Movable = false;
                }
            }
        }

        [Constructable]
        public FruitBasket()
            : this(false)
        {
        }

        [Constructable]
        public FruitBasket(bool rare)
            : base(1, 0x993)
        {
            Weight = 2.0;
            FillFactor = 5;
            Stackable = false;

            DailyRare = rare;
        }

        public FruitBasket(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!DailyRare)
            {
                base.OnDoubleClick(from);
                return;
            }

            if (from.InRange(GetWorldLocation(), 1))
            {
                Eat(from);
            }
        }

        public override bool Eat(Mobile from)
        {
            Point3D p = Location;

            if (!base.Eat(from))
            {
                return false;
            }

            Basket basket = new Basket();

            if (Parent == null && DailyRare)
            {
                basket.MoveToWorld(p, from.Map);
            }
            else
            {
                from.AddToBackpack(new Basket());
            }

            return true;
        }

        public override bool TryEat(Mobile from)
        {
            if (!DailyRare)
            {
                return base.TryEat(from);
            }

            if (Deleted || !from.CheckAlive() || !CheckItemUse(from))
                return false;

            return Eat(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(DailyRare);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    DailyRare = reader.ReadBool();
                    break;
            }
        }
    }

    [Flipable(0x171f, 0x1720)]
    public class Banana : Food
    {
        [Constructable]
        public Banana()
            : this(1)
        {
        }

        [Constructable]
        public Banana(int amount)
            : base(amount, 0x171f)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Banana(Serial serial)
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

    [Flipable(0x1721, 0x1722)]
    public class Bananas : Food
    {
        [Constructable]
        public Bananas()
            : this(1)
        {
        }

        [Constructable]
        public Bananas(int amount)
            : base(amount, 0x1721)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Bananas(Serial serial)
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

    public class SplitCoconut : Food
    {
        [Constructable]
        public SplitCoconut()
            : this(1)
        {
        }

        [Constructable]
        public SplitCoconut(int amount)
            : base(amount, 0x1725)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public SplitCoconut(Serial serial)
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

    public class Lemon : Food
    {
        [Constructable]
        public Lemon()
            : this(1)
        {
        }

        [Constructable]
        public Lemon(int amount)
            : base(amount, 0x1728)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Lemon(Serial serial)
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

    public class Lemons : Food
    {
        [Constructable]
        public Lemons()
            : this(1)
        {
        }

        [Constructable]
        public Lemons(int amount)
            : base(amount, 0x1729)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Lemons(Serial serial)
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

    public class Lime : Food
    {
        [Constructable]
        public Lime()
            : this(1)
        {
        }

        [Constructable]
        public Lime(int amount)
            : base(amount, 0x172a)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Lime(Serial serial)
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

    public class Limes : Food
    {
        [Constructable]
        public Limes()
            : this(1)
        {
        }

        [Constructable]
        public Limes(int amount)
            : base(amount, 0x172B)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Limes(Serial serial)
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

    public class Coconut : Food
    {
        [Constructable]
        public Coconut()
            : this(1)
        {
        }

        [Constructable]
        public Coconut(int amount)
            : base(amount, 0x1726)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Coconut(Serial serial)
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

    public class OpenCoconut : Food
    {
        [Constructable]
        public OpenCoconut()
            : this(1)
        {
        }

        [Constructable]
        public OpenCoconut(int amount)
            : base(amount, 0x1723)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public OpenCoconut(Serial serial)
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

    public class Dates : Food
    {
        [Constructable]
        public Dates()
            : this(1)
        {
        }

        [Constructable]
        public Dates(int amount)
            : base(amount, 0x1727)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Dates(Serial serial)
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

    public class Grapes : Food
    {
        [Constructable]
        public Grapes()
            : this(1)
        {
        }

        [Constructable]
        public Grapes(int amount)
            : base(amount, 0x9D1)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Grapes(Serial serial)
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

    public class Peach : Food
    {
        [Constructable]
        public Peach()
            : this(1)
        {
        }

        [Constructable]
        public Peach(int amount)
            : base(amount, 0x9D2)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Peach(Serial serial)
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

    public class Pear : Food
    {
        [Constructable]
        public Pear()
            : this(1)
        {
        }

        [Constructable]
        public Pear(int amount)
            : base(amount, 0x994)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Pear(Serial serial)
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

    public class Apple : Food
    {
        [Constructable]
        public Apple()
            : this(1)
        {
        }

        [Constructable]
        public Apple(int amount)
            : base(amount, 0x9D0)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Apple(Serial serial)
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

    public class Watermelon : Food
    {
        [Constructable]
        public Watermelon()
            : this(1)
        {
        }

        [Constructable]
        public Watermelon(int amount)
            : base(amount, 0xC5C)
        {
            Weight = 5.0;
            FillFactor = 5;
        }

        public Watermelon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SmallWatermelon : Food
    {
        [Constructable]
        public SmallWatermelon()
            : this(1)
        {
        }

        [Constructable]
        public SmallWatermelon(int amount)
            : base(amount, 0xC5D)
        {
            Weight = 5.0;
            FillFactor = 5;
        }

        public SmallWatermelon(Serial serial)
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

    [Flipable(0xc72, 0xc73)]
    public class Squash : Food
    {
        [Constructable]
        public Squash()
            : this(1)
        {
        }

        [Constructable]
        public Squash(int amount)
            : base(amount, 0xc72)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Squash(Serial serial)
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

    [Flipable(0xc79, 0xc7a)]
    public class Cantaloupe : Food
    {
        [Constructable]
        public Cantaloupe()
            : this(1)
        {
        }

        [Constructable]
        public Cantaloupe(int amount)
            : base(amount, 0xc79)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Cantaloupe(Serial serial)
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

    public class Plum : Food
    {
        public override int LabelNumber => 1157208;  // plum

        [Constructable]
        public Plum()
            : this(1)
        {
        }

        [Constructable]
        public Plum(int amount)
            : base(amount, 0x9E86)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Plum(Serial serial)
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